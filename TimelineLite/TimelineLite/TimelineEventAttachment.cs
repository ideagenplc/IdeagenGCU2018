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
    public class TimelineEventAttachment : LambdaBase
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => CreateAttachment(request));
        }
        
        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditTitle(request));
        }
        
        public APIGatewayProxyResponse Delete(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => Delete(request));
        }
        
        private static APIGatewayProxyResponse CreateAttachment(APIGatewayProxyRequest request)
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
        
        private static APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = ParseRequestBody<EditTimelineEventAttachmentTitleRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.Title))
                return WrapResponse("Invalid Timeline Event Attachment Title ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.AttachmentId))
                return WrapResponse("Invalid Timeline Event Attachment Id", 400);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            model.Title = timelineEventAttachmentRequest.Title;
            repo.SaveModel(model);
            return WrapResponse($"{timelineEventAttachmentRequest.TenantId} {timelineEventAttachmentRequest.Title} {timelineEventAttachmentRequest.AttachmentId}");
        }
        
        private static APIGatewayProxyResponse Delete(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = ParseRequestBody<DeleteTimelineEventAttachmentRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventAttachmentRequest.AttachmentId))
                return WrapResponse("Invalid Timeline Event Attachment Id", 400);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            if(model.IsDeleted)
                return WrapResponse("Timeline Event Attachment already deleted", 404);
            model.IsDeleted = true;
            repo.SaveModel(model);
            return WrapResponse($"{timelineEventAttachmentRequest.TenantId} {timelineEventAttachmentRequest.AttachmentId}");
        }

        private static DynamoDbTimelineEventAttachmentRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventAttachmentRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
    }
}