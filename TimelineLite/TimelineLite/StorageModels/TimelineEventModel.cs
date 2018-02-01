using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventStore")]
    public class TimelineEventModel : BaseModel
    {
        [DynamoDBHashKey]
        public string Title { get; set; }
        public string EventDateTime { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}