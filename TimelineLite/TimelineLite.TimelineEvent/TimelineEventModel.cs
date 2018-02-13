using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
{
    [DynamoDBTable("TimelineEventStore")]
    public class TimelineEventModel : BaseModel
    {
        public string Title { get; set; }
        public string EventDateTime { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string Location { get; set; }
    }
}