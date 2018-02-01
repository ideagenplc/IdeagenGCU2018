namespace TimelineLite.Requests.TimelineEvents
{
    public class GetTimelineEventLinksRequest : BaseRequest
    {
        public string TimelineEventId;
        public string PaginationToken;
    }
}