using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    [DynamoDBTable("TimelineEventAttachLinkStore")]
    public class TimelineEventAttachmentLinkModel : BaseModel
    {
        [DynamoDBGlobalSecondaryIndexRangeKey("TimelineEventId-Index")]
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}