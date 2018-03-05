using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Timelinelite.Core;

namespace TimelineLite.TimelineEventAttachment
{
    public class DynamoDbTimelineEventAttachmentRepository : BaseRepository
    {

        public DynamoDbTimelineEventAttachmentRepository(IAmazonDynamoDB client, string tenantId) : base(tenantId, client)
        {
            ;
        }

        public void CreateTimlineEventAttachment(TimelineEventAttachmentModel model)
        {
            model.TenantId = TenantId;
            Context.SaveAsync(model).Wait();
        }

        public TimelineEventAttachmentModel GetModel(string id)
        {
            var table = Context.GetTargetTable<TimelineEventAttachmentModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventAttachmentModel.Id), QueryOperator.Equal, id);
            var config = CreateQueryConfiguration(filter);
            var search = table.Query(config);
            var model = Context.FromDocuments<TimelineEventAttachmentModel>(search.GetRemainingAsync().Result).SingleOrDefault();
            if (model == null)
                throw new ValidationException($"No Timeline Event Attachment found with Id : {id}");
            return model;
        }

        public IEnumerable<TimelineEventAttachmentModel> GetTimelineEventAttachments(string timelineEventId)
        {
            var timelineEventLinkTable = Context.GetTargetTable<TimelineEventAttachmentModel>();
            var filter = CreateBaseQueryFilter();
            filter.AddCondition(nameof(TimelineEventAttachmentModel.TimelineEventId), QueryOperator.Equal, timelineEventId);

            var config = CreateQueryConfiguration(filter);
            var search = timelineEventLinkTable.Query(config);
            var timelineEventLinkedModels = Context.FromDocuments<TimelineEventAttachmentModel>(search.GetNextSetAsync().Result);
            return timelineEventLinkedModels;
        }

        public void SaveModel(TimelineEventAttachmentModel model)
        {
            Context.SaveAsync(model).Wait(); ;
        }
        public void DeleteModel(TimelineEventAttachmentModel model)
        {
            Context.DeleteAsync(model).Wait();
        }
    }
}