namespace TimelineLite.Requests.TimelineEvents
{
    public class CreateTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        public string Title;
        public string EventDateTime;
        public string Description;
    }
}