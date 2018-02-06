using System;
using System.ComponentModel.DataAnnotations;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Logging;
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
            return Handle(() => EditAttachmentTitle(request));
        }
        
        public APIGatewayProxyResponse Delete(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => DeleteAttachment(request));
        }
        
        private static APIGatewayProxyResponse CreateAttachment(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = ParsePutRequestBody<CreateTimelineEventAttachmentRequest>(request);
            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            ValidateTimelineEventAttachentTitle(timelineEventAttachmentRequest.AttachmentId);
            ValidateTimelineEventId(timelineEventAttachmentRequest.AttachmentId);

            var timelineEventAttachment = new TimelineEventAttachmentModel
            {
                Id = timelineEventAttachmentRequest.AttachmentId,
                Title = timelineEventAttachmentRequest.Title,
                TimelineEventId = timelineEventAttachmentRequest.TimelineEventId
            };
            GetRepo(timelineEventAttachmentRequest.TenantId).CreateTimlineEventAttachment(timelineEventAttachment);

            return WrapResponse($"{JsonConvert.SerializeObject(timelineEventAttachment)}");
        }
        
        private static APIGatewayProxyResponse EditAttachmentTitle(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = ParsePutRequestBody<EditTimelineEventAttachmentTitleRequest>(request);

            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            ValidateTimelineEventAttachentTitle(timelineEventAttachmentRequest.AttachmentId);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            model.Title = timelineEventAttachmentRequest.Title;
            repo.SaveModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }
        
        private static APIGatewayProxyResponse DeleteAttachment(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = ParsePutRequestBody<DeleteTimelineEventAttachmentRequest>(request);

            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            if(model.IsDeleted)
                return WrapResponse($"Cannot find attachment with Id {timelineEventAttachmentRequest.AttachmentId}", 404);
            model.IsDeleted = true;
            repo.SaveModel(model);
            return WrapResponse($"Successfully deleted Timeline event attachment: {timelineEventAttachmentRequest.AttachmentId}");
        }

        private static DynamoDbTimelineEventAttachmentRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventAttachmentRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
        
        private static void ValidateTimelineEventAttachmentId(string timelineEventAttachmentId)
        {
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentId))
                throw new ValidationException("Invalid Timeline Id");
        }
        
        private static void ValidateTimelineEventId(string timelineEventId)
        {
            if (string.IsNullOrWhiteSpace(timelineEventId))
                throw new ValidationException("Invalid Timeline Id");
        }
        
        private static void ValidateTimelineEventAttachentTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Invalid Timeline Event Attachment Title");
        }
    }
}