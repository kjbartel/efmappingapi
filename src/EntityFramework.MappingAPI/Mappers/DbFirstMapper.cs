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
    }
}