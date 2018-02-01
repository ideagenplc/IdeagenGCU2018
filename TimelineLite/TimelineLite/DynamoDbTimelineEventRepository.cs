using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite
{
    public class DynamoDbTimelineEventRepository
    {
        private static IAmazonDynamoDB _client;
        private static DynamoDBContext _context;
        private string _tenantId;

        public DynamoDbTimelineEventRepository(IAmazonDynamoDB client, string tenantId)
        {
            _client = client;
            _tenantId = tenantId;
            _context = new DynamoDBContext(_client);
        }

        public void CreateTimlineEvent(TimelineEventModel model)
        {
            _context.SaveAsync(model).Wait();
        }
        
        public TimelineEventModel GetTimelineEventModel(string id)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineEventModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineEventModel.TenantId), ScanOperator.Equal, _tenantId),
                new ScanCondition(nameof(TimelineEventModel.IsDeleted), ScanOperator.Equal, false)
            };
            
            return _context.ScanAsync<TimelineEventModel>(conditions).GetRemainingAsync().Result.Single();
        }

        public void SaveTimelineEventModel(TimelineEventModel model)
        {
            _context.SaveAsync(model).Wait();
        }
        
        public void SaveTimelineEventLinkedModel(TimelineEventLinkModel model)
        {
            _context.SaveAsync(model).Wait();
        }
        
        public TimelineEventLinkModel GetTimelineEventLinkModel(string id, string linkedToTimelineEventId)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineEventLinkModel.TimelineEventId), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineEventLinkModel.LinkedToTimelineEventId), ScanOperator.Equal, linkedToTimelineEventId),
                new ScanCondition(nameof(TimelineEventLinkModel.TenantId), ScanOperator.Equal, _tenantId),
                new ScanCondition(nameof(TimelineEventLinkModel.IsDeleted), ScanOperator.Equal, false)
            };
            
            return _context.ScanAsync<TimelineEventLinkModel>(conditions).GetRemainingAsync().Result.Single();
        }
        
        public IEnumerable<TimelineEventLinkModel> GetTimelineEventLinks(string timelineEventId, string token, out string newPaginationToken)
        {
            var timelineEventLinkTable = _context.GetTargetTable<TimelineEventLinkModel>();
            var search = timelineEventLinkTable.Query(new QueryOperationConfig
            {
                Limit = 2,
                Filter = new QueryFilter(nameof(TimelineEventLinkModel.TimelineEventId), QueryOperator.Equal, timelineEventId),
                PaginationToken = token,
                KeyExpression = new Expression(),
            });
            newPaginationToken = search.PaginationToken;
            var timelineEventLinkedModels = _context.FromDocuments<TimelineEventLinkModel>(search.GetNextSetAsync().Result);
            return timelineEventLinkedModels;
        }
    }
}