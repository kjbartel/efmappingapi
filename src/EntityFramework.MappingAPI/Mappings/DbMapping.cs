using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using EntityFramework.MappingAPI.Extensions;
using EntityFramework.MappingAPI.Mappers;

#if EF6
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

namespace EntityFramework.MappingAPI.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    internal class DbMapping : IDbMapping
    {
        private readonly Dictionary<string, ITableMapping> _tableMappings = new Dictionary<string, ITableMapping>();
        private readonly string _contextTypeName;

        private readonly MetadataWorkspace _metadataWorkspace;

        private Dictionary<string, EntityType> GetTypeMappingsEf4()
        {
            var entityTypes = _metadataWorkspace.GetItems(DataSpace.CSpace).OfType<EntityType>();

            var clrTypes = _metadataWorkspace.GetItems(DataSpace.OSpace)
                .Select(x => x.ToString())
                .ToDictionary(x => x.Substring(x.LastIndexOf('.') + 1));

            // can be matched by name because classes with same name from different namespaces can not be used
            // http://entityframework.codeplex.com/workitem/714

            var typeMappings = new Dictionary<string, EntityType>();
            foreach (var entityType in entityTypes)
            {
                if (entityType.Name == "EdmMetadata")
                {
                    continue;
                }

                var key = clrTypes[entityType.Name];
                typeMappings[key] = entityType;
            }

            return typeMappings;
        }

        private Dictionary<string, EntityType> GetTypeMappingsEf6()
        {
            return _metadataWorkspace.GetItems(DataSpace.OCSpace)
                    .Select(x => new { identity = x.ToString().Split(':'), edmItem = x.GetPrivateFieldValue("EdmItem") as EntityType })
                    .Where(x => x.edmItem != null)
                    .ToDictionary(x => x.identity[0], x => x.edmItem);
        }

        public Dictionary<string, EntityType> TypeMappings
        {
            get
            {
#if EF6
                return GetTypeMappingsEf6();
#else
                return GetTypeMappingsEf4();
#endif
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public DbMapping(DbContext context)
        {
            _contextTypeName = context.GetType().FullName;

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            _metadataWorkspace = objectContext.MetadataWorkspace;

            MapperBase mapper;

            EntityContainer entityContainer;
            if (_metadataWorkspace.TryGetEntityContainer("CodeFirstDatabase", true, DataSpace.SSpace, out entityContainer))
            {
                mapper = new CodeFirstMapper(_metadataWorkspace, entityContainer);
            }
            else
            {
                entityContainer = _metadataWorkspace.GetEntityContainer("DbFirstModelStoreContainer", DataSpace.SSpace);
                mapper = new DbFirstMapper(_metadataWorkspace, entityContainer);
            }

            // todo - order tables by hierarchy
            // if dbsets are defined in context. root table is not taken as first table.

            foreach (var kvp in TypeMappings)
            {
                var tableMapping = mapper.MapTable(kvp.Key, kvp.Value);
                if (tableMapping == null)
                {
                    continue;
                }

                tableMapping.DbMapping = this;
                _tableMappings.Add(kvp.Key, tableMapping);
            }

            mapper.BindForeignKeys();
        }

        /// <summary>
        /// Tables in database
        /// </summary>
        public ITableMapping[] Tables { get { return _tableMappings.Values.ToArray(); } }

        /// <summary>
        /// Get table mapping by entity type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ITableMapping this[Type type]
        {
            get { return this[type.FullName]; }
        }

        /// <summary>
        /// Get table mapping by entity type full name
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public ITableMapping this[string typeFullName]
        {
            get
            {
                if (!_tableMappings.ContainsKey(typeFullName))
                    throw new Exception("Type '" + typeFullName + "' is not found in context '" + _contextTypeName + "'");

                return _tableMappings[typeFullName];
            }
        }
    }
}
