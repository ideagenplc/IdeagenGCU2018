using TimelineLite.Requests;

namespace TimelineLite.TimelineEventAttchment
{
    public class EditTimelineEventAttachmentTitleRequest : BaseRequest
    {
        public string AttachmentId;
        public string Title;
    }
}