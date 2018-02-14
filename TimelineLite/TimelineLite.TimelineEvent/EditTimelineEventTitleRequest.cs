using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class EditTimelineEventTitleRequest : BaseRequest
    {
        public string TimelineEventId;
        public string Title;
    }
}