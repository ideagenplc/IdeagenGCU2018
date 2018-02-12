using System;
using System.ComponentModel.DataAnnotations;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using TimelineLite.Requests;
using TimelineLite.Responses;

namespace TimelineLite.TimelineEventAttchment
{
    public class TimelineEventAttachment : LambdaBase
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => CreateAttachment(request));
        }
        
        public APIGatewayProxyResponse GenerateUploadPresignedUrl(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GenerateUploadAttachmentPresignedUrl(request));
        }

        public APIGatewayProxyResponse GenerateGetPresignedUrl(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GenerateGetAttachmentPresignedUrl(request));
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
            var timelineEventAttachmentRequest = RequestHelper.ParsePutRequestBody<CreateTimelineEventAttachmentRequest>(request);
            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            ValidateTimelineEventAttachentTitle(timelineEventAttachmentRequest.Title);
            ValidateTimelineEventId(timelineEventAttachmentRequest.TimelineEventId);

            var timelineEventAttachment = new TimelineEventAttachmentModel
            {
                Id = timelineEventAttachmentRequest.AttachmentId,
                Title = timelineEventAttachmentRequest.Title,
                TimelineEventId = timelineEventAttachmentRequest.TimelineEventId
            };
            GetRepo(timelineEventAttachmentRequest.TenantId).CreateTimlineEventAttachment(timelineEventAttachment);

            return ResponseHelper.WrapResponse($"{JsonConvert.SerializeObject(timelineEventAttachment)}");
        }
        
        private static APIGatewayProxyResponse GenerateUploadAttachmentPresignedUrl(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var attachmentId = request.Headers["AttachmentId"];
            ValidateTimelineEventAttachmentId(attachmentId);

            var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            var presignedUrl = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "stewartw-test-bucket",
                Verb = HttpVerb.PUT,
                Key = $"{tenantId}/{attachmentId}",
                Expires = DateTime.Now.AddMinutes(15)
            });
            return ResponseHelper.WrapResponse(presignedUrl);
        }

        private static APIGatewayProxyResponse GenerateGetAttachmentPresignedUrl(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var attachmentId = request.Headers["AttachmentId"];
            ValidateTimelineEventAttachmentId(attachmentId);

            var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            var presignedUrl = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "stewartw-test-bucket",
                Verb = HttpVerb.GET,
                Key = $"{tenantId}/{attachmentId}",
                Expires = DateTime.Now.AddMinutes(15)
            });
            return ResponseHelper.WrapResponse(presignedUrl);
        }

        private static APIGatewayProxyResponse EditAttachmentTitle(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = RequestHelper.ParsePutRequestBody<EditTimelineEventAttachmentTitleRequest>(request);

            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            ValidateTimelineEventAttachentTitle(timelineEventAttachmentRequest.Title);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            model.Title = timelineEventAttachmentRequest.Title;
            repo.SaveModel(model);
            return ResponseHelper.WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }
        
        private static APIGatewayProxyResponse DeleteAttachment(APIGatewayProxyRequest request)
        {
            var timelineEventAttachmentRequest = RequestHelper.ParsePutRequestBody<DeleteTimelineEventAttachmentRequest>(request);

            ValidateTimelineEventAttachmentId(timelineEventAttachmentRequest.AttachmentId);
            
            var repo = GetRepo(timelineEventAttachmentRequest.TenantId);
            var model = repo.GetModel(timelineEventAttachmentRequest.AttachmentId);
            repo.DeleteModel(model);
            return ResponseHelper.WrapResponse($"Successfully deleted Timeline event attachment: {timelineEventAttachmentRequest.AttachmentId}");
        }

        private static DynamoDbTimelineEventAttachmentRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventAttachmentRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
        
        private static void ValidateTimelineEventAttachmentId(string timelineEventAttachmentId)
        {
            if (string.IsNullOrWhiteSpace(timelineEventAttachmentId))
                throw new ValidationException("Invalid Timeline Attachment Id");
        }
        
        private static void ValidateTimelineEventId(string timelineEventId)
        {
            if (string.IsNullOrWhiteSpace(timelineEventId))
                throw new ValidationException("Invalid Timeline Event Id");
        }
        
        private static void ValidateTimelineEventAttachentTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Invalid Timeline Event Attachment Title");
        }
    }
}