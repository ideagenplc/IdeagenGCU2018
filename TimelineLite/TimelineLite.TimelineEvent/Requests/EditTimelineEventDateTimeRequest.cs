using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class EditTimelineEventDateTimeRequest : BaseRequest
    {
        public string TimelineEventId;
        public string EventDateTime;
    }
}