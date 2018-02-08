namespace TimelineLite.Requests.TimelineEvents
{
    public class LinkTimelineEventToTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        
        public string LinkedToTimelineEventId;
    }
}