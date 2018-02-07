using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TimelineLite.StorageModels;

namespace TimelineLite.StorageRepos
{
    public class DynamoDbTimelineRepository : BaseRepository
    {

        public DynamoDbTimelineRepository(IAmazonDynamoDB client, string tenantId) : base(tenantId, client)
        {
        }

        public void CreateTimeline(TimelineModel model)
        {
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }
        
        public void CreateLink(TimelineTimelineEventLinkModel model)
        {
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }
        
        public void DeleteLink(string timelineId, string eventId)
        {
            var table = Context.GetTargetTable<TimelineTimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineTimelineEventLinkModel.TimelineId), QueryOperator.Equal, timelineId);
            filter.AddCondition(nameof(TimelineTimelineEventLinkModel.TimelineEventId), ScanOperator.Equal, eventId);
            
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            var model = Context.FromDocuments<TimelineTimelineEventLinkModel>(search.GetRemainingAsync().Result).Single();
            
            Context.DeleteAsync<TimelineTimelineEventLinkModel>(model).Wait();
        }
        
        public IEnumerable<TimelineTimelineEventLinkModel> GetLinkedEvents(string timelineId, string skip = "0")
        {
            var pageToken = skip != "0" ? CreatePaginationToken(skip) : "{}";
            
            var timelineEventLinkTable = Context.GetTargetTable<TimelineTimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineTimelineEventLinkModel.TimelineId), QueryOperator.Equal, timelineId);
            
            var config = CreateQueryConfiguration(filter, pageToken: pageToken);
            var search = timelineEventLinkTable.Query(config);
            var timelineEventLinkedModels = Context.FromDocuments<TimelineTimelineEventLinkModel>(search.GetNextSetAsync().Result);
            pageToken = search.PaginationToken;
            return timelineEventLinkedModels;
        }
        
        public TimelineModel GetModel(string id)
        {
            var table = Context.GetTargetTable<TimelineModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineModel.Id), QueryOperator.Equal, id);
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(TimelineModel.Id), ScanOperator.Equal, id),
                new ScanCondition(nameof(TimelineModel.TenantId), ScanOperator.Equal, TenantId),
                new ScanCondition(nameof(TimelineModel.IsDeleted), ScanOperator.Equal, false)
            };
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            return Context.FromDocuments<TimelineModel>(search.GetRemainingAsync().Result).Single();
        }

        public void SaveModel(TimelineModel model)
        {
            Context.SaveAsync(model).Wait();
        }
    }
}