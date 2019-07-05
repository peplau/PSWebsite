using System.Collections.Generic;
using System.Runtime.Serialization;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace PSWebsite.Foundation.Indexing.SearchTypes
{
    public class BaseSearchResultItem : SearchResultItem
    {
        [IndexField("_templates")]
        [DataMember]
        public virtual string Templates { get; set; }

        [IndexField("_templates")]
        [DataMember]
        public virtual List<ID> TemplateIds { get; set; }
    }
}