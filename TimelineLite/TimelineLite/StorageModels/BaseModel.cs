using Amazon.DynamoDBv2.DataModel;

namespace TimelineLite.StorageModels
{
    public class BaseModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBHashKey]
        public string TenantId { get; set; }
    }
}