using Amazon.DynamoDBv2.DataModel;
using Timelinelite.Core;

namespace TimelineLite.Timeline
{
    [DynamoDBTable("TimelineStore")]
    public class TimelineModel : BaseModel
    {
        
        public string Title { get; set; }
        public string CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}