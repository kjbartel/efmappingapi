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
    }
}