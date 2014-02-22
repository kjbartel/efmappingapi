using System;
using EntityFramework.MappingAPI.Mappings;

namespace EntityFramework.MappingAPI
{
    public interface IColumnMapping
    {
        /// <summary>
        /// Table column name
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// Entity property name
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Is column primary key
        /// </summary>
        bool IsPk { get; }

        /// <summary>
        /// Is column nullable
        /// </summary>
        bool Nullable { get; }

        /// <summary>
        /// Column default value
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// 
        /// </summary>
        bool Computed { get; }

        /// <summary>
        /// Column max length
        /// </summary>
        int MaxLength { get; }

        /// <summary>
        /// Data type stored in the column
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// Is table-per-hierarchy discriminator
        /// </summary>
        bool IsDiscriminator { get; set; }

        /// <summary>
        /// Paren table mapping
        /// </summary>
        ITableMapping TableMapping { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsNavigationProperty { get; }

        /// <summary>
        /// Is foreign key
        /// </summary>
        bool IsFk { get; }

        /// <summary>
        /// Foreign keys navigation propery name
        /// </summary>
        string NavigationProperty { get; }

        /// <summary>
        /// Foreign key target column
        /// </summary>
        IColumnMapping FkTargetColumn { get; }
    }
}