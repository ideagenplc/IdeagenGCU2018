using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineEventAttachmentStore")]
    public class TimelineEventAttachmentModel : BaseModel
    {
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}