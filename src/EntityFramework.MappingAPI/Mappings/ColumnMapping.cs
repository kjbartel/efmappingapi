using System;

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
    internal class ColumnMapping : IColumnMapping
    {
        /// <summary>
        /// Table column name
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Entity property name
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Is column primary key
        /// </summary>
        public bool IsPk { get; internal set; }

        /// <summary>
        /// Is column nullable
        /// </summary>
        public bool Nullable { get; internal set; }

        /// <summary>
        /// Column default value
        /// </summary>
        public object DefaultValue { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Computed { get; internal set; }

        /// <summary>
        /// Column max length
        /// </summary>
        public int MaxLength { get; internal set; }

        /// <summary>
        /// Data type stored in the column
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Is table-per-hierarchy discriminator
        /// </summary>
        public bool IsDiscriminator { get; set; }

        /// <summary>
        /// Paren table mapping
        /// </summary>
        public ITableMapping TableMapping { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNavigationProperty { get; internal set; }

        /// <summary>
        /// Is foreign key
        /// </summary>
        public bool IsFk { get; internal set; }

        /// <summary>
        /// Foreign keys navigation propery name
        /// </summary>
        public string NavigationProperty { get; internal set; }

        /// <summary>
        /// Foreign key target column
        /// </summary>
        public IColumnMapping FkTargetColumn { get; internal set; }

        /// <summary>
        /// Edm property from storage entity set (SSpace).
        /// This propery is needed to know which properties are already mapped to TPH entity.
        /// </summary>
        internal EdmProperty EdmProperty { get; set; }

        /// <summary>
        /// Edm member from CSpace.
        /// Stored for linking foreign keys.
        /// </summary>
        internal EdmMember EdmMember { get; set; }

        /// <summary>
        /// Edm member from CSpace.
        /// Stored for linking foreign keys.
        /// </summary>
        internal EdmMember FkTargetEdmMember { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        internal ColumnMapping(string property, string columnName)
        {
            ColumnName = columnName;
            PropertyName = property;
        }
    }
}