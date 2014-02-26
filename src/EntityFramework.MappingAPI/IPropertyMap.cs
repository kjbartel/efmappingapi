using System;
using EntityFramework.MappingAPI.Mappings;

namespace EntityFramework.MappingAPI
{
    public interface IPropertyMap
    {
        /// <summary>
        /// Table column name property is mapped to
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
        bool IsRequired { get; }

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
        /// Paren entity mapping
        /// </summary>
        IEntityMap EntityMap { get; }

        #region nav property

        /// <summary>
        /// Is navigation property
        /// </summary>
        bool IsNavigationProperty { get; }

        /// <summary>
        /// Foreign key property name which is used for navigation property.
        /// </summary>
        string ForeignKeyPropertyName { get; }

        /// <summary>
        /// Foreign key property.
        /// Available only for navigation properties.
        /// </summary>
        IPropertyMap ForeignKey { get; }

        #endregion

        #region fk property

        /// <summary>
        /// Is foreign key
        /// </summary>
        bool IsFk { get; }

        /// <summary>
        /// Foreign keys navigation propery name
        /// </summary>
        string NavigationPropertyName { get; }

        /// <summary>
        /// Foreign key target column
        /// </summary>
        IPropertyMap FkTargetColumn { get; }

        /// <summary>
        /// Foreign key navigation property
        /// </summary>
        IPropertyMap NavigationProperty { get; }

        #endregion
    }
}