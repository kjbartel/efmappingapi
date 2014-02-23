using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.MappingAPI.Exceptions;
using EntityFramework.MappingAPI.Extensions;
using EntityFramework.MappingAPI.Mappings;

#if EF6
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

namespace EntityFramework.MappingAPI.Mappers
{
    internal abstract class MapperBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected EntityContainer EntityContainer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly MetadataWorkspace _metadataWorkspace;

        /// <summary>
        /// Table mappings dictionary where key is entity type full name.
        /// </summary>
        private readonly Dictionary<string, TableMapping> _tableMappings = new Dictionary<string, TableMapping>();

        /// <summary>
        /// Primary keys of tables.
        /// Key is table name (in db).
        /// Values is array of entity property names that are primary keys
        /// </summary>
        private readonly Dictionary<string, string[]> _pks = new Dictionary<string, string[]>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<EdmMember, EdmMember> _fks = new Dictionary<EdmMember, EdmMember>(); 

        private Dictionary<string, TphData> _tphData;

        public Dictionary<string, TphData> TphData
        {
            get
            {
                if (_tphData != null)
                    return _tphData;

                var entitySetMaps = (IEnumerable<object>)_metadataWorkspace
                    .GetItemCollection(DataSpace.CSSpace)[0]
                    .GetPrivateFieldValue("EntitySetMaps");

                _tphData = new Dictionary<string, TphData>();

                foreach (var entitySetMap in entitySetMaps)
                {
                    var props = new List<EdmMember>();
                    var navProps = new List<NavigationProperty>();
                    var discriminators = new Dictionary<string, object>();

                    var typeMappings = (IEnumerable<object>)entitySetMap.GetPrivateFieldValue("TypeMappings");
                    foreach (var typeMapping in typeMappings)
                    {
                        var types = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("Types");
                        var isOfypes = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("IsOfTypes");
                        var mappingFragments = (IEnumerable<object>)typeMapping.GetPrivateFieldValue("MappingFragments");

                        // if isOfType.length > 0, then it is base type of TPH
                        // must merge properties with siblings
                        foreach (EntityType type in isOfypes)
                        {
                            var identity = type.ToString();
                            if (!_tphData.ContainsKey(identity))
                                _tphData[identity] = new TphData();

                            _tphData[identity].NavProperties = navProps.ToArray();
                            _tphData[identity].Properties = props.ToArray();
                            _tphData[identity].Discriminators = discriminators;
                        }

                        foreach (EntityType type in types)
                        {
                            var identity = type.ToString();
                            if (!_tphData.ContainsKey(identity))
                                _tphData[identity] = new TphData();

                            // type.Properties gets properties including inherited properties
                            var tmp = new List<EdmMember>(type.Properties);

                            foreach (var navProp in type.NavigationProperties)
                            {
                                var associationType = navProp.RelationshipType as AssociationType;
                                if (associationType != null)
                                {
                                    // if entity does not contain id property i.e has only reference object for a fk
                                    if (associationType.ReferentialConstraints.Count == 0)
                                    {
                                        tmp.Add(navProp);
                                    }
                                }
                            }

                            _tphData[identity].NavProperties = type.NavigationProperties.ToArray();
                            _tphData[identity].Properties = tmp.ToArray();

                            foreach (var prop in type.Properties)
                            {
                                if (!props.Contains(prop))
                                    props.Add(prop);
                            }

                            foreach (var navProp in type.NavigationProperties)
                            {
                                if (!navProps.Contains(navProp))
                                    navProps.Add(navProp);
                            }

                            foreach (var fragment in mappingFragments)
                            {
                                var conditionProperties = (IEnumerable)fragment.GetPrivateFieldValue("m_conditionProperties");
                                foreach (var conditionalProperty in conditionProperties)
                                {
                                    var columnName = ((EdmProperty)conditionalProperty.GetPrivateFieldValue("Key")).Name;
                                    var value = conditionalProperty.GetPrivateFieldValue("Value").GetPrivateFieldValue("Value");

                                    _tphData[identity].Discriminators[columnName] = value;
                                    discriminators[columnName] = value;
                                }
                            }
                        }
                    }
                }

                return _tphData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataWorkspace"></param>
        /// <param name="entityContainer">Code first or DB first entityContainer</param>
        protected MapperBase(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer)
        {
            _metadataWorkspace = metadataWorkspace;
            EntityContainer = entityContainer;

            var relations = _metadataWorkspace.GetItems(DataSpace.CSpace).OfType<AssociationType>();

            foreach (var associationType in relations)
            {
                foreach (var referentialConstraint in associationType.ReferentialConstraints)
                {
                    for (int i = 0; i < referentialConstraint.ToProperties.Count; ++i)
                    {
                        _fks[referentialConstraint.ToProperties[i]] = referentialConstraint.FromProperties[i];
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitySet"></param>
        /// <returns></returns>
        protected abstract string GetTableName(EntitySet entitySet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        internal TableMapping RegTable(string typeFullName, string tableName, string schema)
        {
            var tableMapping = new TableMapping(typeFullName, tableName, schema);
            _tableMappings.Add(typeFullName, tableMapping);

            return tableMapping;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="edmItem"></param>
        public TableMapping MapTable(string typeFullName, EdmType edmItem)
        {
            var identity = edmItem.FullName;
            if (!TphData.ContainsKey(identity))
            {
                return null;
            }

            // find existing parent storageEntitySet
            // thp derived types does not have storageEntitySet
            EntitySet storageEntitySet;
            EdmType baseEdmType = edmItem;
            while (!EntityContainer.TryGetEntitySetByName(baseEdmType.Name, false, out storageEntitySet))
            {
                if (baseEdmType.BaseType == null)
                {
                    break;
                }
                baseEdmType = baseEdmType.BaseType;
            }

            if (storageEntitySet == null)
            {
                return null;
            }

            var isRoot = baseEdmType == edmItem;
            if (!isRoot)
            {
                var parent = _tableMappings.Values.FirstOrDefault(x => x.EdmType == baseEdmType);
                // parent table has not been mapped yet
                if (parent == null)
                {
                    throw new ParentNotMappedYetException();
                }
            }

            string tableName = GetTableName(storageEntitySet);
            string schema = (string)storageEntitySet.MetadataProperties["Schema"].Value;

            var tableMapping = this.RegTable(typeFullName, tableName, schema);
            tableMapping.IsRoot = isRoot;
            tableMapping.EdmType = edmItem;
            tableMapping.ParentEdmType = tableMapping.IsRoot ? null : baseEdmType;

            _pks[tableName] = storageEntitySet.ElementType.KeyMembers.Select(x => x.Name).ToArray();

            tableMapping.IsTph = TphData[identity].Discriminators.Count > 0;

            string prefix = null;
            int i = 0;

            var propertiesToMap = GetPropertiesToMap(tableMapping, storageEntitySet.ElementType.Properties);
            foreach (var prop in propertiesToMap)
            {
                MapColumn(tableMapping, prop, identity, ref i, ref prefix);
            }

            foreach (var navigationProperty in TphData[identity].NavProperties)
            {
                var associationType = navigationProperty.RelationshipType as AssociationType;
                if (associationType == null || associationType.ReferentialConstraints.Count == 0)
                {
                    continue;
                }

                var to = associationType.ReferentialConstraints[0].ToProperties[0];

                var col = tableMapping.Columns.Cast<ColumnMapping>()
                    .FirstOrDefault(x => x.EdmMember == to);

                if (col != null)
                {
                    col.NavigationProperty = navigationProperty.Name;
                }
            }

            foreach (var discriminator in TphData[identity].Discriminators)
            {
                var column = tableMapping.AddColumn(discriminator.Key, discriminator.Key);
                column.DefaultValue = discriminator.Value;
                column.IsDiscriminator = true;
            }

            return tableMapping;
        }

        /// <summary>
        /// Gets properties that are ment for given type.
        /// TPH columns are ordered by hierarchy and type name. 
        /// First columns are from base class. Derived types, which name starts with 'A', columns are before type, which name starts with 'B' etc.
        /// So the logic is to include all properties inherited from base types and exclude all already bound properties from siblings.
        /// </summary>
        /// <param name="tableMapping"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private IEnumerable<EdmProperty> GetPropertiesToMap(TableMapping tableMapping, IEnumerable<EdmProperty> properties)
        {
            if (tableMapping.IsRoot)
            {
                return properties;
            }

            var parentEdmType = tableMapping.ParentEdmType;

            var include = new List<EdmProperty>();
            var exclude = new List<EdmProperty>();

            while (true)
            {
                var parent = _tableMappings.Values.FirstOrDefault(x => x.EdmType == parentEdmType);
                if (parent == null)
                {
                    break;
                }

                include.AddRange(parent.Columns.Cast<ColumnMapping>().Select(x => x.EdmProperty));

                exclude.AddRange(_tableMappings.Values.Where(x => x.ParentEdmType == parentEdmType)
                    .SelectMany(x => x.Columns)
                    .Cast<ColumnMapping>()
                    .Select(x => x.EdmProperty));

                parentEdmType = parent.ParentEdmType;
            }

            return properties.Where(edmProperty => include.Contains(edmProperty) || !exclude.Contains(edmProperty)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableMapping"></param>
        /// <param name="prop"></param>
        /// <param name="i"></param>
        /// <param name="prefix"></param>
        private void MapColumn(TableMapping tableMapping, EdmProperty prop, string identity, ref int i, ref string prefix)
        {
            //var identity = tableMapping.EdmType.FullName;
            var entityMembers = TphData[identity].Properties;
            var columnName = prop.Name;

            if (entityMembers.Length <= i)
            {
                return;
            }
            EdmMember edmMember = entityMembers[i];

            // check if is complex type
            if (string.IsNullOrEmpty(prefix) && edmMember.TypeUsage.EdmType.GetType() == typeof(ComplexType))
            {
                prefix = edmMember.Name;
                ++i;
            }

            string propName;

            if (prefix != null)
            {
                if (columnName.StartsWith(prefix + "_"))
                {
                    propName = columnName.Replace('_', '.');
                }
                else
                {
                    prefix = null;
                    propName = edmMember.Name;
                    ++i;
                }
            }
            else
            {
                propName = entityMembers[i++].Name;
            }

            var propInfo = tableMapping.Type.GetProperty(propName, '.');
            if (propInfo == null)
            {
                // entity does not contain such property
                return;
            }

            ColumnMapping colMapping;
            try
            {
                colMapping = tableMapping.AddColumn(propName, columnName);
                colMapping.EdmProperty = prop;
                colMapping.EdmMember = edmMember;
                colMapping.IsNavigationProperty = edmMember is NavigationProperty;
                colMapping.IsFk = _fks.ContainsKey(edmMember);
                if (colMapping.IsFk)
                {
                    colMapping.FkTargetEdmMember = _fks[edmMember];
                    tableMapping.AddFk(colMapping);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Failed to map propName {0} to column {1} on table {2} ({3})",
                    propName,
                    columnName,
                    tableMapping.TableName,
                    tableMapping.TypeFullName);

                throw new Exception(errorMessage, ex);
            }

            if (_pks[tableMapping.TableName].Contains(propName))
            {
                colMapping.IsPk = true;
                tableMapping.AddPk(colMapping);
            }

            foreach (var facet in prop.TypeUsage.Facets)
            {
                switch (facet.Name)
                {
                    case "Nullable":
                        colMapping.Nullable = (bool)facet.Value;
                        break;
                    case "DefaultValue":
                        colMapping.DefaultValue = facet.Value;
                        break;
                    case "StoreGeneratedPattern":
                        colMapping.IsIdentity = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Identity;
                        colMapping.Computed = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Computed;
                        break;
                    case "MaxLength":

                        try
                        {
                            colMapping.MaxLength = (int)facet.Value;
                        }
                        catch (Exception)
                        {
                            var stringVal = facet.Value.ToString();
                            if (stringVal == "Max")
                                colMapping.MaxLength = int.MaxValue;
                            else
                                throw;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BindForeignKeys()
        {
            var fks = _tableMappings.Values.SelectMany(x => x.Fks);
            var pks = _tableMappings.Values.SelectMany(x => x.Pks);

            // can't use ToDictionary, because tph tables share mappings
            var pkDict = new Dictionary<EdmMember, ColumnMapping>();
            foreach (ColumnMapping columnMapping in pks)
            {
                pkDict[columnMapping.EdmMember] = columnMapping;
            }

            foreach (ColumnMapping fkCol in fks)
            {
                fkCol.FkTargetColumn = pkDict[fkCol.FkTargetEdmMember];
            }
        }
    }
}