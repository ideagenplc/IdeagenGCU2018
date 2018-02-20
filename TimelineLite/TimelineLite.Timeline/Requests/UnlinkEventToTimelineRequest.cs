using Timelinelite.Core;

namespace TimelineLite.Timeline
{
    public class UnlinkEventToTimelineRequest : BaseRequest
    {
        public string TimelineId;
        public string EventId;
    }
}