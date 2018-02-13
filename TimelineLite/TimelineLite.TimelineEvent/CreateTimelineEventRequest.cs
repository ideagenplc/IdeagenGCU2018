using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class CreateTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        public string Title;
        public string EventDateTime;
        public string Description;
    }
}