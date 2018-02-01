using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventLinkStore")]
    public class TimelineEventLinkModel : BaseModel
    {
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}