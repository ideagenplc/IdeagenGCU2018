using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite
{
    public class DynamoDbTimelineEventAttachmentRepository
    {
        private static IAmazonDynamoDB _client;
        private static DynamoDBContext _context;
        private string _tenantId;

        public DynamoDbTimelineEventAttachmentRepository(IAmazonDynamoDB client, string tenantId)
        {
            _client = client;
            _tenantId = tenantId;
            _context = new DynamoDBContext(_client);
        }

        public void CreateTimlineEventAttachment(TimelineEventAttachmentModel model)
        { 
            _context.SaveAsync(model).Wait();
        }

        public TimelineEventAttachmentModel GetModel(string id)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineEventAttachmentModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineEventAttachmentModel.TenantId), ScanOperator.Equal, _tenantId)
            };
            
            return _context.ScanAsync<TimelineEventAttachmentModel>(conditions).GetRemainingAsync().Result.Single();
        }

        public void SaveModel(TimelineEventAttachmentModel model)
        {
            _context.SaveAsync(model);
        }
    
    }
}