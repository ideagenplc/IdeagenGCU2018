using System.Collections.Generic;
using System.Linq;
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

        public void CreateTimeline(TimelineModel model)
        {
            model.TenantId = _tenantId;
            _context.SaveAsync(model).Wait();
        }
        
        public void CreateLink(TimelineTimelineEventLinkModel model)
        {
            model.TenantId = _tenantId;
            _context.SaveAsync(model).Wait();
        }
        
        public void GetLinkedEvents(string timelineId)
        {
        }
        
        public TimelineModel GetModel(string id)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineModel.TenantId), ScanOperator.Equal, _tenantId),
                new ScanCondition(nameof(TimelineModel.IsDeleted), ScanOperator.Equal, false)
            };
            
            return _context.ScanAsync<TimelineModel>(conditions).GetRemainingAsync().Result.Single();
        }

        public void SaveModel(TimelineModel model)
        {
            _context.SaveAsync(model);
        }
    }
}