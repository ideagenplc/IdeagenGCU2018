using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite
{
    public class DynamoDbTimelineRepository
    {
        private static IAmazonDynamoDB _client;
        private static DynamoDBContext _context;
        private string _tenantId;

        public DynamoDbTimelineRepository(IAmazonDynamoDB client, string tenantId)
        {
            _client = client;
            _tenantId = tenantId;
            _context = new DynamoDBContext(_client);
        }

        public void CreateTimline(TimelineModel model)
        {
            _context.SaveAsync(model).Wait();
        }
        
        public TimelineModel GetModel(string id)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineModel.TenantId), ScanOperator.Equal, _tenantId)
            };
            
            return _context.ScanAsync<TimelineModel>(conditions).GetRemainingAsync().Result.Single();
        }
    
    }
}