namespace TimelineLite.Requests.TimelineEventAttachments
{
    public class CreateTimelineEventAttachmentRequest : BaseRequest
    {
        public string AttachmentId;
        public string TimelineEventId;
        public string Title;
    }
}