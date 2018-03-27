using System;
using System.Collections.Generic;
using Amazon.Runtime.Internal;
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
            Attachments = new List<Attachment>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string EventDateTime { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string Location { get; set; }
        public List<string> LinkedTimelineEventIds { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }
    }
}