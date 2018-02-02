using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite
{
    public class DynamoDbTimelineEventRepository : BaseRepository
    {

        public DynamoDbTimelineEventRepository(IAmazonDynamoDB client, string tenantId) : base(tenantId, client)
        {
        }

        public void CreateTimlineEvent(TimelineEventModel model)
        {
            Console.WriteLine($"Creating timeline event for Id:{model.Id}");
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }
        
        public TimelineEventModel GetTimelineEventModel(string id)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineEventModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineEventModel.TenantId), ScanOperator.Equal, TenantId),
                new ScanCondition(nameof(TimelineEventModel.IsDeleted), ScanOperator.Equal, false)
            };
            
            return Context.ScanAsync<TimelineEventModel>(conditions).GetRemainingAsync().Result.Single();
        }

        public void SaveTimelineEventModel(TimelineEventModel model)
        {
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }
        
        public void SaveTimelineEventLinkedModel(TimelineEventLinkModel model)
        {
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }
        
        public TimelineEventLinkModel GetTimelineEventLinkModel(string id, string linkedToTimelineEventId)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineEventLinkModel.TimelineEventId), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineEventLinkModel.LinkedToTimelineEventId), ScanOperator.Equal, linkedToTimelineEventId),
                new ScanCondition(nameof(TimelineEventLinkModel.TenantId), ScanOperator.Equal, TenantId),
                new ScanCondition(nameof(TimelineEventLinkModel.IsDeleted), ScanOperator.Equal, false)
            };
            
            return Context.ScanAsync<TimelineEventLinkModel>(conditions).GetRemainingAsync().Result.Single();
        }
        
        public IEnumerable<TimelineEventLinkModel> GetTimelineEventLinks(string timelineEventId, int skip)
        {
            var pageToken = CreatePaginationToken(skip);
            
            var timelineEventLinkTable = Context.GetTargetTable<TimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventLinkModel.TimelineEventId), QueryOperator.Equal, timelineEventId);
            
            var config = CreateQueryConfiguration(filter, pageToken: pageToken);
            var search = timelineEventLinkTable.Query(config);
            var timelineEventLinkedModels = Context.FromDocuments<TimelineEventLinkModel>(search.GetNextSetAsync().Result);
            pageToken = search.PaginationToken;
           
            return timelineEventLinkedModels;
        }


    }
}