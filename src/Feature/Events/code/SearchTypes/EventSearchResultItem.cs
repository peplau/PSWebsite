using System;
using System.Runtime.Serialization;
using PSWebsite.Foundation.Indexing.SearchTypes;
using Sitecore.ContentSearch;

namespace PSWebsite.Feature.Events.SearchTypes
{
    public class EventSearchResultItem : BaseSearchResultItem
    {
        /// <summary>Gets or sets the EventDate.</summary>
        [IndexField("event_date_tdt")]
        [DataMember]
        public virtual DateTime EventDate { get; set; }

        /// <summary>Gets or sets the EventName.</summary>
        [IndexField("event_name_t")]
        [DataMember]
        public virtual string EventName { get; set; }

        /// <summary>Gets or sets the EventSummary.</summary>
        [IndexField("event_summary_t")]
        [DataMember]
        public virtual string EventSummary { get; set; }

        /// <summary>Gets or sets the EventText.</summary>
        [IndexField("event_text_t")]
        [DataMember]
        public virtual string EventText { get; set; }

        /// <summary>Gets or sets the EventVenue.</summary>
        [IndexField("event_venue_t")]
        [DataMember]
        public virtual string EventVenue { get; set; }
    }
}