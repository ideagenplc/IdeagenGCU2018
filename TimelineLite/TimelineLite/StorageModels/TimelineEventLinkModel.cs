using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventLinkStore")]
    public class TimelineEventLinkModel : BaseModel
    {
        public string TimelineId { get; set; }
        public string EventId { get; set; }
    }
}