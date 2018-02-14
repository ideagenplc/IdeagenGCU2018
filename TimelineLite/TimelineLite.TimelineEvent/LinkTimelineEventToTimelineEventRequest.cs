using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class LinkTimelineEventToTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        
        public string LinkedToTimelineEventId;
    }
}