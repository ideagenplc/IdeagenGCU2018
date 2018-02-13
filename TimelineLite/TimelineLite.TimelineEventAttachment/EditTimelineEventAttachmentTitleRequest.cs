using TimelineLite.Requests;

namespace TimelineLite.TimelineEventAttachment
{
    public class EditTimelineEventAttachmentTitleRequest : BaseRequest
    {
        public string AttachmentId;
        public string Title;
    }
}