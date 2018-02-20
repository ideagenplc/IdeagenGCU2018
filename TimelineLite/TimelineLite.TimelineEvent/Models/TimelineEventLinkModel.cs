using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    [DynamoDBTable("TimelineEventLinkStore")]
    public class TimelineEventLinkModel : BaseModel
    {
        [DynamoDBGlobalSecondaryIndexRangeKey("TimelineEventId-Index")]
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
    }
}