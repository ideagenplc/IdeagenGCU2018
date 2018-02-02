//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Amazon.DynamoDBv2;
//using Amazon.DynamoDBv2.DataModel;
//using Amazon.DynamoDBv2.DocumentModel;
//using Amazon.DynamoDBv2.Model;
//using TimelineLite.StorageModels;
//
//namespace TimelineLite
//{
//    public class DynamoDbTimelineEventLinkRepository
//    {
//        private static IAmazonDynamoDB _client;
//        private static DynamoDBContext _context;
//        private string _tenantId;
//
//        public DynamoDbTimelineEventLinkRepository(IAmazonDynamoDB client, string tenantId)
//        {
//            _client = client;
//            _tenantId = tenantId;
//            _context = new DynamoDBContext(_client);
//        }
//
//        public void CreateLink(TimelineEventLinkModel model)
//        {
//            model.TenantId = _tenantId;
//            _context.SaveAsync(model).Wait();
//        }
//
//        public TimelineEventLinkModel GetLink(string timelineId, string eventId)
//        {
//            var conditions = new List<ScanCondition>
//            {
//                new ScanCondition(nameof(TimelineEventLinkModel.TenantId), ScanOperator.Equal, _tenantId),
//                new ScanCondition(nameof(TimelineEventLinkModel.EventId), ScanOperator.Equal, eventId),
//                new ScanCondition(nameof(TimelineEventLinkModel.TimelineId), ScanOperator.Equal, timelineId)
//            };
//
//            return _context.ScanAsync<TimelineEventLinkModel>(conditions).GetRemainingAsync().Result.Single();
//        }
//        
//        public List<TimelineEventLinkModel> GetLinks(string timelineId, string paginationToken)
//        {
//            var conditions = new List<ScanCondition>
//            {
//                new ScanCondition(nameof(TimelineEventLinkModel.TenantId), ScanOperator.Equal, _tenantId),
//                new ScanCondition(nameof(TimelineEventLinkModel.TimelineId), ScanOperator.Equal, timelineId)
//            };
//            return new List<TimelineEventLinkModel>();
//        }
//
//        public void DeleteLink(string timelineId, string eventId)
//        {
//            var conditions = new List<ScanCondition>
//            {
//                new ScanCondition(nameof(TimelineEventLinkModel.TenantId), ScanOperator.Equal, _tenantId),
//                new ScanCondition(nameof(TimelineEventLinkModel.EventId), ScanOperator.Equal, eventId),
//                new ScanCondition(nameof(TimelineEventLinkModel.TimelineId), ScanOperator.Equal, timelineId)
//            };
//            var model = _context.ScanAsync<TimelineEventLinkModel>(conditions).GetRemainingAsync().Result.Single();
//            _context.DeleteAsync<TimelineEventLinkModel>(model);
//        }
//
//        public void SaveModel(TimelineEventLinkModel model)
//        {
//            _context.SaveAsync(model);
//        }
//    }
//}