using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventLinkStore")]
    public class TimelineEventLinkModel : BaseModel
    {
        [DynamoDBGlobalSecondaryIndexRangeKey("TimelineEventId-Index")]
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}