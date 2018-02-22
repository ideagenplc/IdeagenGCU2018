using System;
using System.Collections.Generic;
using Timelinelite.Core;

namespace TimelineLite.General.Responses
{
    public class GetAllTimelinesAndEventsResponse
    {
        public GetAllTimelinesAndEventsResponse()
        {
            Timelines = new List<Timeline>();
        }
        public List<Timeline> Timelines { get; set; }
    }

    public class Timeline
    {
        public Timeline()
        {
            TimelineEvents = new List<TimelineEvent>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
        public List<TimelineEvent> TimelineEvents { get; set; }
    }

    public class TimelineEvent
    {
        public TimelineEvent()
        {
            LinkedTimelineEventIds = new List<string>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string EventDateTime { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string Location { get; set; }
        public List<string> LinkedTimelineEventIds { get; set; }
    }
}