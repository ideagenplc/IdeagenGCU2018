using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
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