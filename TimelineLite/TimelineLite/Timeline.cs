using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Core;
using TimelineLite.Requests;
using TimelineLite.StorageModels;
using TimelineLite.StorageRepos;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TimelineLite
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

        #endregion

        #region Private Methods

        private static APIGatewayProxyResponse CreateTimeline(APIGatewayProxyRequest request)
        {
            var createTimelineRequest = ParsePutRequestBody<CreateTimelineRequest>(request);

            if (string.IsNullOrWhiteSpace(createTimelineRequest.TimelineId))
                throw new ValidationException("Invalid Timeline Id");
            if (string.IsNullOrWhiteSpace(createTimelineRequest.Title))
                throw new ValidationException("Invalid Timeline Title");

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                createTimelineRequest.TenantId);
            var timeline = new TimelineModel
            {
                Id = createTimelineRequest.TimelineId,
                Title = createTimelineRequest.Title,
                CreationTimeStamp = DateTime.Now.Ticks.ToString(),
                IsDeleted = false
            };
            repo.CreateTimeline(timeline);
            return WrapResponse(
                $"{createTimelineRequest.TenantId} {createTimelineRequest.TimelineId} {createTimelineRequest.Title}");
        }

        private static APIGatewayProxyResponse EditTimelineTitle(APIGatewayProxyRequest request)
        {
            var editTimelineTitleRequest = ParsePutRequestBody<EditTimelineTitleRequest>(request);

            if (string.IsNullOrWhiteSpace(editTimelineTitleRequest.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);
            if (string.IsNullOrWhiteSpace(editTimelineTitleRequest.Title))
                return WrapResponse("Invalid Timeline Title ", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                editTimelineTitleRequest.TenantId);
            var model = repo.GetModel(editTimelineTitleRequest.TimelineId);
            model.Title = editTimelineTitleRequest.Title;
            repo.SaveModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse DeleteTimeline(APIGatewayProxyRequest request)
        {
            var editTimelineTitleRequest = ParsePutRequestBody<DeleteTimelineRequest>(request);

            if (string.IsNullOrWhiteSpace(editTimelineTitleRequest.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                editTimelineTitleRequest.TenantId);
            var model = repo.GetModel(editTimelineTitleRequest.TimelineId);
            model.IsDeleted = true;
            repo.SaveModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse LinkEventToTimeline(APIGatewayProxyRequest request)
        {
            var linkRequest = ParsePutRequestBody<LinkEventToTimelineRequest>(request);

            if (string.IsNullOrWhiteSpace(linkRequest.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);
            if (string.IsNullOrWhiteSpace(linkRequest.EventId))
                return WrapResponse("Invalid Event Id", 400);

            var timelineRepo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                linkRequest.TenantId);
            var linkRepo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                linkRequest.TenantId);


            var model = timelineRepo.GetModel(linkRequest.TimelineId);
            var linkModel = new TimelineTimelineEventLinkModel()
            {
                TimelineEventId = linkRequest.EventId,
                TimelineId = model.Id,
                Id = Guid.NewGuid().ToString()
            };

            linkRepo.CreateLink(linkModel);
            return WrapResponse($"OK");
        }

        private static APIGatewayProxyResponse UnlinkEventFromTimeline(APIGatewayProxyRequest request)
        {
//            var unlinkRequest = ParseRequestBody<LinkEventToTimelineRequest>(request);
//
//            if (string.IsNullOrWhiteSpace(unlinkRequest.TimelineId))
//                return WrapResponse("Invalid Timeline Id", 400);
//            if (string.IsNullOrWhiteSpace(unlinkRequest.EventId))
//                return WrapResponse("Invalid Event Id", 400);
//
//            var linkRepo = new DynamoDbTimelineEventLinkRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
//                unlinkRequest.TenantId);
//
//            linkRepo.DeleteLink(unlinkRequest.TimelineId, unlinkRequest.EventId);
            return WrapResponse($"OK");
        }

        private static APIGatewayProxyResponse GetLinkedEvents(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var timeLineId = request.Headers["TimelineId"];
            var skip = request.Headers["Skip"];
            
            if(string.IsNullOrWhiteSpace(tenantId))
                throw new ValidationException($"Invalid Tenant Id {tenantId}");

            if (string.IsNullOrWhiteSpace(skip))
                skip = "0";
            
//            var getLinkedEventsRequest = ParseRequestBody<GetEventsOnTimelineRequest>(request);
//
//            if (string.IsNullOrWhiteSpace(getLinkedEventsRequest.TimelineId))
//                return WrapResponse("Invalid Timeline Id", 400);
//
           var linkRepo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                tenantId);
            var events = linkRepo.GetLinkedEvents(timeLineId, skip);
            return WrapResponse(events);
        }

        private static APIGatewayProxyResponse GetTimeline(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var timelineId = request.Headers["TimelineId"];

            if (string.IsNullOrWhiteSpace(timelineId))
                return WrapResponse("Invalid Timeline Id", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1),
                tenantId);
            var model = repo.GetModel(timelineId);
            return WrapResponse(JsonConvert.SerializeObject(model));
        }

        #endregion
    }
}