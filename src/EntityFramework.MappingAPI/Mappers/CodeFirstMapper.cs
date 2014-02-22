#if EF6
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

namespace EntityFramework.MappingAPI.Mappers
{
    internal class CodeFirstMapper : MapperBase
    {
        public CodeFirstMapper(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer) 
            : base(metadataWorkspace, entityContainer)
        {
        }

        protected override string GetTableName(EntitySet entitySet)
        {
            // return entitySet.Table;

            return (string)entitySet.MetadataProperties["Table"].Value;
        }
    }
}
