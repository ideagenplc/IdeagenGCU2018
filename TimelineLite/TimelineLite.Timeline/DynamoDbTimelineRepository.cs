using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Timelinelite.Core;

namespace TimelineLite.Timeline
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
            var models = Context.FromDocuments<TimelineTimelineEventLinkModel>(search.GetRemainingAsync().Result).ToList();
            if (!models.Any())
                throw new ValidationException($"There's no link between {timelineId} and {eventId}");
            Context.DeleteAsync<TimelineTimelineEventLinkModel>(models).Wait();
        }

        public IEnumerable<TimelineTimelineEventLinkModel> GetLinkedEvents(string timelineId)
        {
            var timelineEventLinkTable = Context.GetTargetTable<TimelineTimelineEventLinkModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineTimelineEventLinkModel.TimelineId), QueryOperator.Equal, timelineId);

            var config = CreateQueryConfiguration(filter);
            var search = timelineEventLinkTable.Query(config);
            var timelineEventLinkedModels =
                Context.FromDocuments<TimelineTimelineEventLinkModel>(search.GetNextSetAsync().Result);
            return timelineEventLinkedModels;
        }

        public TimelineModel GetModel(string id)
        {
            var table = Context.GetTargetTable<TimelineModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineModel.Id), QueryOperator.Equal, id);
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            var model = Context.FromDocuments<TimelineModel>(search.GetRemainingAsync().Result).FirstOrDefault();
            if (model == null)
                throw new ValidationException($"No Timeline found with Id : {id}");
            return model;
        }

        public void SaveModel(TimelineModel model)
        {
            Context.SaveAsync(model).Wait();
        }

        public IEnumerable<TimelineModel> GetModels()
        {
            var table = Context.GetTargetTable<TimelineModel>();
            var filter = CreateBaseQueryFilter();
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            return Context.FromDocuments<TimelineModel>(search.GetRemainingAsync().Result);
        }

        public void DeleteTimeline(TimelineModel model)
        {
            Context.DeleteAsync(model).Wait();
        }
    }
}