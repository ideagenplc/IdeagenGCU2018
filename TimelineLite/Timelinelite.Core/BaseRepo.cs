using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite.StorageRepos
{
    public abstract class BaseRepository
    {
        protected readonly string TenantId;
        protected readonly DynamoDBContext Context;

        protected BaseRepository(string tenantId, IAmazonDynamoDB client)
        {
            TenantId = tenantId;
            Context = new DynamoDBContext(client);
        }
        protected static QueryOperationConfig CreateQueryConfiguration(QueryFilter filter)
        {
            var queryOperationConfig = new QueryOperationConfig
            {
                Filter = filter
            };
            return queryOperationConfig;
        }
        
        protected QueryFilter CreateBaseQueryFilter()
        {
            var filter = new QueryFilter(nameof(BaseModel.TenantId), QueryOperator.Equal, TenantId);
            return filter;
        }
        
        // Skip is the Id of the last value returned in the previous query;
        protected string CreatePaginationToken(object skip)
        {
            var pageToken = $"{{\"Id\":{{\"S\":\"{skip}\"}},\"TenantId\":{{\"S\":\"{TenantId}\"}}}}";
            return pageToken;
        }
    }
}