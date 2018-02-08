namespace TimelineLite.Requests.TimelineEvents
{
    public class UnlinkTimelineEventToTimelineEventRequest : BaseRequest
    {
        public string TimelineEventId;
        
        public string UnlinkedFromTimelineEventId;
    }
}