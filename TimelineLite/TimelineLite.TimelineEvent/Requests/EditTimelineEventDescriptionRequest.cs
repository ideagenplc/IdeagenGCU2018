using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class EditTimelineEventDescriptionRequest : BaseRequest
    {
        public string TimelineEventId;
        public string Description;
    }
}