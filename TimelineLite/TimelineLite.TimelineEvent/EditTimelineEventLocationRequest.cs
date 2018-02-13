using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class EditTimelineEventLocationRequest : BaseRequest
    {
        public string TimelineEventId;
        public string Location;
    }
}