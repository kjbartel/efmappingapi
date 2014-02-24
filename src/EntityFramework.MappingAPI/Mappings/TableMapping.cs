using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#if EF6
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

namespace EntityFramework.MappingAPI.Mappings
{
    internal class TableMapping<T> : TableMapping, ITableMapping<T>
    {
        public IColumnMapping Col<T1>(Expression<Func<T, T1>> predicate)
        {
            var predicateString = predicate.ToString();
            var i = predicateString.IndexOf('.');
            var propName = predicateString.Substring(i + 1);
            return base[propName];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class TableMapping : ITableMapping
    {
        private readonly Dictionary<string, IColumnMapping> _columnMappings = new Dictionary<string, IColumnMapping>();
        private readonly List<IColumnMapping> _fks = new List<IColumnMapping>();
        private readonly List<IColumnMapping> _pks = new List<IColumnMapping>();

        /// <summary>
        /// Entity type full name
        /// </summary>
        public string TypeFullName { get; internal set; }

        /// <summary>
        /// Entity type
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// Table name in database
        /// </summary>
        public string TableName { get; internal set; }

        /// <summary>
        /// Database schema
        /// </summary>
        public string Schema { get; internal set; }

        /// <summary>
        /// Is table-per-hierarchy mapping
        /// </summary>
        public bool IsTph { get; internal set; }

        /// <summary>
        /// Is table-per-hierarchy base entity
        /// </summary>
        public bool IsRoot { get; internal set; }

        /// <summary>
        /// Column mappings for table
        /// </summary>
        public IColumnMapping[] Columns
        {
            get { return _columnMappings.Values.ToArray(); }
        }

        /// <summary>
        /// Foreign key columns
        /// </summary>
        public IColumnMapping[] Fks
        {
            get { return _fks.ToArray(); }
        }

        /// <summary>
        /// Primary key columns
        /// </summary>
        public IColumnMapping[] Pks
        {
            get { return _pks.ToArray(); }
        }

        /// <summary>
        /// Gets column mapping by property name
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IColumnMapping this[string property]
        {
            get { return _columnMappings[property]; }
        }

        /// <summary>
        /// Parent DbMapping
        /// </summary>
        //public IDbMapping DbMapping { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        internal EdmType ParentEdmType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal EdmType EdmType { get; set; }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        internal TableMapping(string typeFullName, string tableName, string schema)
        {
            TypeFullName = typeFullName;
            TableName = tableName;
            Schema = schema;

            Type = TryGetRefObjectType();
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public ColumnMapping AddColumn(string property, string columnName)
        {
            var cmap = new ColumnMapping(property, columnName) {TableMapping = this};
            _columnMappings.Add(property, cmap);

            return cmap;
        }

        public void AddFk(ColumnMapping colMapping)
        {
            _fks.Add(colMapping);
        }

        public void AddPk(ColumnMapping colMapping)
        {
            _pks.Add(colMapping);
        }
    }
}