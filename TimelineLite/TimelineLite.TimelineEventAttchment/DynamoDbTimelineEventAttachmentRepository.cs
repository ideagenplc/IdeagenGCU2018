using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace TimelineLite.TimelineEventAttchment
{
    public class DynamoDbTimelineEventAttachmentRepository
    {
        private static DynamoDBContext _context;
        private readonly string _tenantId;

        public DynamoDbTimelineEventAttachmentRepository(IAmazonDynamoDB client, string tenantId)
        {
            _tenantId = tenantId;
            _context = new DynamoDBContext(client);
        }

        public void CreateTimlineEventAttachment(TimelineEventAttachmentModel model)
        { 
            model.TenantId = _tenantId;
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
            _context.SaveAsync(model).Wait();;
        }
    
    }
}