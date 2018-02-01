using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Requests.TimelineEvents;
using TimelineLite.StorageModels;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;

namespace TimelineLite
{
    public class TimelineEvent
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventRequest = ParseRequestBody<CreateTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Title))
                return WrapResponse("Invalid Timeline Event Title ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Description))
                return WrapResponse("Invalid Timeline Event Description ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.EventDateTime))
                return WrapResponse("Invalid Timeline Event Date Time ", 400);

            var timelineEvent = new TimelineEventModel
            {
                Id = timelineEventRequest.TimelineEventId,
                Title = timelineEventRequest.Title,
                EventDateTime = timelineEventRequest.EventDateTime,
                Description = timelineEventRequest.Description
            };
            GetRepo(timelineEventRequest.TenantId).CreateTimlineEvent(timelineEvent);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Title} {timelineEventRequest.Description} {timelineEventRequest.EventDateTime}");
        }
        
        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventTitleRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Title))
                return WrapResponse("Invalid Timeline Event Title ", 400);

            GetRepo(timelineEventRequest.TenantId).EditTimelineEventTitle(timelineEventRequest.TimelineEventId, timelineEventRequest.Title);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Title}");
        }
        
        public APIGatewayProxyResponse EditDescription(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventDescriptionRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Desciption))
                return WrapResponse("Invalid Timeline Event Description", 400);

            GetRepo(timelineEventRequest.TenantId).EditTimelineEventDescription(timelineEventRequest.TimelineEventId, timelineEventRequest.Desciption);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Desciption}");
        }
        
        public APIGatewayProxyResponse EditEventDateTime(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventDateTimeRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.EventDateTime))
                return WrapResponse("Invalid Timeline Event Date Time", 400);

            GetRepo(timelineEventRequest.TenantId).EditTimelineEventDateTime(timelineEventRequest.TimelineEventId, timelineEventRequest.EventDateTime);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.EventDateTime}");
        }
        
        public APIGatewayProxyResponse DeleteEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var timelineEventRequest = ParseRequestBody<DeleteTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);

            GetRepo(timelineEventRequest.TenantId).DeleteTimelineEvent(timelineEventRequest.TimelineEventId);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId}");
        }

        private static DynamoDbTimelineEventRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
    }
}