
using System;
using System.Linq.Expressions;

namespace EntityFramework.MappingAPI
{
    public interface ITableMapping<T> : ITableMapping
    {
        IColumnMapping Col<T1>(Expression<Func<T, T1>> predicate);
    }

    public interface ITableMapping
    {
        /// <summary>
        /// Entity type full name
        /// </summary>
        //string TypeFullName { get; }

        /// <summary>
        /// Entity type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Table name in database
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Database schema
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Is table-per-hierarchy mapping
        /// </summary>
        bool IsTph { get; }

        /// <summary>
        /// Is table-per-hierarchy base entity
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Column mappings for table
        /// </summary>
        IColumnMapping[] Columns { get; }

        /// <summary>
        /// Parent DbMapping
        /// </summary>
        //IDbMapping DbMapping { get; }

        /// <summary>
        /// Foreign key columns
        /// </summary>
        IColumnMapping[] Fks { get; }

        /// <summary>
        /// Primary key columns
        /// </summary>
        IColumnMapping[] Pks { get; }

        /// <summary>
        /// Gets column mapping by property name
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IColumnMapping this[string property] { get; }
    }
}