using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEvent
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
            var table = Context.GetTargetTable<TimelineEventModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventModel.Id), QueryOperator.Equal, id);
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            var model = Context.FromDocuments<TimelineEventModel>(search.GetRemainingAsync().Result).SingleOrDefault();
            if (model == null)
                throw new ValidationException($"No Timeline Event found with Id : {id}");
            return model;
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

        public void DeleteLink(string timelineId, string linkedToTimelineEventId)
        {
            var table = Context.GetTargetTable<TimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventLinkModel.TimelineEventId), QueryOperator.Equal, timelineId);
            filter.AddCondition(nameof(TimelineEventLinkModel.LinkedToTimelineEventId), ScanOperator.Equal, linkedToTimelineEventId);

            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            var model = Context.FromDocuments<TimelineEventLinkModel>(search.GetRemainingAsync().Result)
                .SingleOrDefault();
            if (model == null)
                throw new ValidationException($"There's no link between {timelineId} and {linkedToTimelineEventId}");
            Context.DeleteAsync<TimelineEventLinkModel>(model).Wait();
        }

        public IEnumerable<TimelineEventLinkModel> GetTimelineEventLinks(string timelineEventId)
        {
            var timelineEventLinkTable = Context.GetTargetTable<TimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventLinkModel.TimelineEventId), QueryOperator.Equal, timelineEventId);

            var config = CreateQueryConfiguration(filter);
            var search = timelineEventLinkTable.Query(config);
            var timelineEventLinkedModels =
                Context.FromDocuments<TimelineEventLinkModel>(search.GetNextSetAsync().Result);
            return timelineEventLinkedModels;
        }
    }
}