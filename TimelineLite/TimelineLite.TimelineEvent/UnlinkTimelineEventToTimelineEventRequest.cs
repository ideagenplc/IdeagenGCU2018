using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class UnlinkTimelineEventToTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        
        public string UnlinkedFromTimelineEventId;
    }
}