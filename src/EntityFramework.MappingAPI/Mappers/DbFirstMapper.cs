using System;
using System.Collections.Generic;
using System.Linq;
#if EF6
    using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Metadata.Edm;
#endif

namespace EntityFramework.MappingAPI.Mappers
{
    internal class DbFirstMapper : MapperBase
    {
        public DbFirstMapper(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer)
            : base(metadataWorkspace, entityContainer)
        {
        }

        protected override string GetTableName(EntitySet entitySet)
        {
            return entitySet.Name;
        }

        protected override Dictionary<string, EntityType> GetTypeMappingsEf4()
        {
            var entityTypes = MetadataWorkspace.GetItems(DataSpace.CSpace)
                .OfType<EntityType>()
                .ToDictionary(x => x.ToString());

            return entityTypes;
        }

        protected override Dictionary<string, TphData> GetTphData()
        {
#if EF4 || EF5
            throw new NotImplementedException("EF4 and EF5 DbFirst is not supported yet");
#else
            return base.GetTphData();
#endif
        }
    }
}