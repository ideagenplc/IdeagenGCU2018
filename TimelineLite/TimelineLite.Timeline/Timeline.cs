using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Timelinelite.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TimelineLite.Timeline
{
    public class Timeline : LambdaBase
    {
        #region Functions

        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => CreateTimeline(request));
        }

        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditTimelineTitle(request));
        }

        public APIGatewayProxyResponse Delete(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => DeleteTimeline(request));
        }

        public APIGatewayProxyResponse LinkEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => LinkEventToTimeline(request));
        }

        public APIGatewayProxyResponse UnlinkEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => UnlinkEventFromTimeline(request));
        }

        public APIGatewayProxyResponse GetEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetLinkedEvents(request));
        }

        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetTimeline(request));
        }

        public APIGatewayProxyResponse GetAll(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetAllTimelines(request));
        }

        #endregion

        #region Private Methods

        private static APIGatewayProxyResponse CreateTimeline(APIGatewayProxyRequest request)
        {
            var createTimelineRequest = RequestHelper.ParsePutRequestBody<CreateTimelineRequest>(request);

            createTimelineRequest.TimelineId.ValidateString("Invalid Timeline Id");
            createTimelineRequest.Title.ValidateString("Invalid Timeline Title");

            var repo = GetRepository(createTimelineRequest);

            var timeline = new TimelineModel
            {
                Id = createTimelineRequest.TimelineId,
                Title = createTimelineRequest.Title,
                CreationTimeStamp = DateTime.Now.Ticks.ToString(),
                IsDeleted = false
            };

            repo.CreateTimeline(timeline);
            return ResponseHelper.WrapResponse($"{timeline}");
        }

        private static APIGatewayProxyResponse EditTimelineTitle(APIGatewayProxyRequest request)
        {
            var editTimelineTitleRequest = RequestHelper.ParsePutRequestBody<EditTimelineTitleRequest>(request);

            editTimelineTitleRequest.TimelineId.ValidateString("Invalid Timeline Id");
            editTimelineTitleRequest.Title.ValidateString("Invalid Timeline Title");

            var repo = GetRepository(editTimelineTitleRequest);

            var model = repo.GetModel(editTimelineTitleRequest.TimelineId);
            model.Title = editTimelineTitleRequest.Title;
            repo.SaveModel(model);

            return ResponseHelper.WrapResponse($"{model}");
        }

        private static APIGatewayProxyResponse DeleteTimeline(APIGatewayProxyRequest request)
        {
            var deleteTimelineRequest = RequestHelper.ParsePutRequestBody<DeleteTimelineRequest>(request);
            
            deleteTimelineRequest.TimelineId.ValidateString("Invalid Timeline Id");

            var repo = GetRepository(deleteTimelineRequest);

            var model = repo.GetModel(deleteTimelineRequest.TimelineId);
            repo.DeleteTimeline(model);

            return ResponseHelper.WrapResponse($"{model}");
        }

        private static APIGatewayProxyResponse LinkEventToTimeline(APIGatewayProxyRequest request)
        {
            var linkRequest = RequestHelper.ParsePutRequestBody<LinkEventToTimelineRequest>(request);
            
            linkRequest.TimelineId.ValidateString("Invalid Timeline Id");
            linkRequest.EventId.ValidateString("Invalid Event Id");

            var timelineRepo = GetRepository(linkRequest);

            var model = timelineRepo.GetModel(linkRequest.TimelineId);
            var linkModel = new TimelineTimelineEventLinkModel()
            {
                TimelineEventId = linkRequest.EventId,
                TimelineId = model.Id,
                Id = Guid.NewGuid().ToString()
            };

            timelineRepo.CreateLink(linkModel);
            return ResponseHelper.WrapResponse($"OK");
        }

        private static APIGatewayProxyResponse UnlinkEventFromTimeline(APIGatewayProxyRequest request)
        {
            var unlinkRequest = RequestHelper.ParsePutRequestBody<LinkEventToTimelineRequest>(request);

            unlinkRequest.TimelineId.ValidateString("Invalid Timeline Id");
            unlinkRequest.EventId.ValidateString("Invalid Event Id");

            var linkRepo = GetRepository(unlinkRequest);

            linkRepo.DeleteLink(unlinkRequest.TimelineId, unlinkRequest.EventId);
            return ResponseHelper.WrapResponse($"OK");
        }

        private static APIGatewayProxyResponse GetLinkedEvents(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            
            request.Headers.TryGetValue("TimelineId", out var timelineId);
            timelineId.ValidateString("Invalid Timeline Id");

            var linkRepo = GetRepository(tenantId);

            var events = linkRepo.GetLinkedEvents(timelineId);
            return ResponseHelper.WrapResponse(events);
        }

        private static APIGatewayProxyResponse GetTimeline(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            request.Headers.TryGetValue("TimelineId", out var timelineId);

            timelineId.ValidateString("Invalid Timeline Id");
            var repo = GetRepository(tenantId);
            
            var model = repo.GetModel(timelineId);
            return ResponseHelper.WrapResponse(model);
        }

        private static APIGatewayProxyResponse GetAllTimelines(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
            var models = repo.GetModels();
            return ResponseHelper.WrapResponse(models);
        }
        
        private static DynamoDbTimelineRepository GetRepository(BaseRequest request)
        {
            return GetRepository(request.TenantId);
        }

        private static DynamoDbTimelineRepository GetRepository(string tenantId)
        {
            return new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }

        #endregion
    }
}