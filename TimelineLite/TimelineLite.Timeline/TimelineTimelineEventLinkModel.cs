using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.Timeline
{
    [DynamoDBTable("TimelineTimelineEventLinkStore")]
    public class TimelineTimelineEventLinkModel : BaseModel
    {
        public string TimelineEventId { get; set; }
        [DynamoDBGlobalSecondaryIndexRangeKey("TimelineId-Index")]
        public string TimelineId { get; set; }
        public bool IsDeleted { get; set; }
    }
}