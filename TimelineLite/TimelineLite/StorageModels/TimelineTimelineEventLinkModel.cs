using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventLinkStore")]
    public class TimelineTimelineEventLinkModel : BaseModel
    {
        public string TimelineEventId { get; set; }
        [DynamoDBGlobalSecondaryIndexRangeKey("TimelineId-Index")]
        public string TimelineId { get; set; }
        public bool IsDeleted { get; set; }
    }
}