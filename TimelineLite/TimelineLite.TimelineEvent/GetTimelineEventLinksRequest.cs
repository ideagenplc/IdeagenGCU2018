using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    public class GetTimelineEventLinksRequest : BaseRequest
    {
        public string TimelineEventId;
        public string PaginationToken;
        public int Skip;
    }
}