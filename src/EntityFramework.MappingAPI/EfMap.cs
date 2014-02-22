using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.MappingAPI.Mappings;

namespace EntityFramework.MappingAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class EfMap
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, IDbMapping> Mappings = new Dictionary<Type, IDbMapping>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ITableMapping Get<T>(DbContext context)
        {
            return Get(context)[typeof(T)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDbMapping Get(DbContext context)
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