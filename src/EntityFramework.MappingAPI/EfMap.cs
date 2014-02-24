using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.MappingAPI.Mappings;

namespace EntityFramework.MappingAPI
{
    /// <summary>
    /// 
    /// </summary>
    internal class EfMap
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, DbMapping> Mappings = new Dictionary<Type, DbMapping>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ITableMapping<T> Get<T>(DbContext context)
        {
            return (ITableMapping<T>)Get(context)[typeof(T)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ITableMapping Get(DbContext context, Type type)
        {
            return Get(context)[type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ITableMapping Get(DbContext context, string typeFullName)
        {
            return Get(context)[typeFullName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbMapping Get(DbContext context)
        {
            var contextType = context.GetType();
            if (Mappings.ContainsKey(contextType))
            {
                return Mappings[contextType];
            }

            var mapping = new DbMapping(context);
            //var mapping = Map(context);

            Mappings[contextType] = mapping;
            return mapping;
        }
    }
}