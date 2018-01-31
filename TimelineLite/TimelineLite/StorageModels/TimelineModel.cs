using System.Runtime.InteropServices.ComTypes;
using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    [DynamoDBTable("TimelineStore")]
    public class TimelineModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Title { get; set; }
        public string CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}