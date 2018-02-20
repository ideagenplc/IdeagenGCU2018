using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEventAttachment
{
    [DynamoDBTable("AttachmentStore")]
    public class TimelineEventAttachmentModel : BaseModel
    {
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}