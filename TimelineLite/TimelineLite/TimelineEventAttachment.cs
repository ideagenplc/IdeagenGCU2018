using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TimelineLite.Requests.TimelineEventAttachments;
using TimelineLite.StorageModels;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;

namespace TimelineLite
{
    public class TimelineEventAttachment
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventAttachmentRequest = ParseRequestBody<CreateTimelineEventAttachmentRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.Title))
                return WrapResponse("Invalid Timeline Event Attachment Title ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.AttachmentId))
                return WrapResponse("Invalid Timeline Event Attachment Id ", 400);

            var timelineEventAttachment = new TimelineEventAttachmentModel
            {
                Id = timelineEventAttachmentRequest.AttachmentId,
                Title = timelineEventAttachmentRequest.Title,
                TimelineEventId = timelineEventAttachmentRequest.TimelineEventId
            };
            GetRepo(timelineEventAttachmentRequest.TenantId).CreateTimlineEventAttachment(timelineEventAttachment);
            return WrapResponse($"{timelineEventAttachmentRequest.TenantId} {timelineEventAttachmentRequest.TimelineEventId} {timelineEventAttachmentRequest.Title} {timelineEventAttachmentRequest.AttachmentId}");
        }
        
        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventAttachmentRequest = ParseRequestBody<CreateTimelineEventAttachmentRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.Title))
                return WrapResponse("Invalid Timeline Event Attachment Title ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.AttachmentId))
                return WrapResponse("Invalid Timeline Event Attachment Id ", 400);

            var timelineEventAttachment = new TimelineEventAttachmentModel
            {
                Id = timelineEventAttachmentRequest.AttachmentId,
                Title = timelineEventAttachmentRequest.Title,
                TimelineEventId = timelineEventAttachmentRequest.TimelineEventId
            };
            GetRepo(timelineEventAttachmentRequest.TenantId).CreateTimlineEventAttachment(timelineEventAttachment);
            return WrapResponse($"{timelineEventAttachmentRequest.TenantId} {timelineEventAttachmentRequest.TimelineEventId} {timelineEventAttachmentRequest.Title} {timelineEventAttachmentRequest.AttachmentId}");
        }

        private static DynamoDbTimelineEventAttachmentRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventAttachmentRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
    }
}