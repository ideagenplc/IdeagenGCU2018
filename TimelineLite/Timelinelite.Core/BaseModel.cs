using Amazon.DynamoDBv2.DataModel;

namespace Timelinelite.Core
{
    public class BaseModel
    {
        [DynamoDBRangeKey]
        public string Id { get; set; }
        [DynamoDBHashKey]
        public string TenantId { get; set; }
    }
}