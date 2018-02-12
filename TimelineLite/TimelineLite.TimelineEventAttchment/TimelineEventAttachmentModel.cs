using Amazon.DynamoDBv2.DataModel;
using TimelineLite.StorageModels;

namespace TimelineLite.TimelineEventAttchment
{
    [DynamoDBTable("TimelineEventAttachmentStore")]
    public class TimelineEventAttachmentModel : BaseModel
    {
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}