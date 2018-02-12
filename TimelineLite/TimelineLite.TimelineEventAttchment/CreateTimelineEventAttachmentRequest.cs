using TimelineLite.Requests;

namespace TimelineLite.TimelineEventAttchment
{
    public class CreateTimelineEventAttachmentRequest : BaseRequest
    {
        public string AttachmentId;
        public string TimelineEventId;
        public string Title;
    }
}