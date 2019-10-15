using CF.Models;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class WebFunctionNameMap : EntityTypeConfiguration<WebFunctionName>
    {
        public WebFunctionNameMap()
        {
            HasKey(wfn => new { wfn.WebFunctionId, wfn.Language });
        }
    }
}
