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
    public class TimelineEvents
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var createTimelineEventRequest = ParseRequestBody<CreateTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(createTimelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(createTimelineEventRequest.Title))
                return WrapResponse("Invalid Timeline Event Title ", 400);
            if (string.IsNullOrWhiteSpace(createTimelineEventRequest.Description))
                return WrapResponse("Invalid Timeline Event Description ", 400);
            if (string.IsNullOrWhiteSpace(createTimelineEventRequest.EventDateTime))
                return WrapResponse("Invalid Timeline Event Date Time ", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            var timeline = new TimelineEventModel
            {
                Id = createTimelineEventRequest.TimelineEventId,
                Title = createTimelineEventRequest.Title,
                EventDateTime = createTimelineEventRequest.EventDateTime,
                Description = createTimelineEventRequest.Description
            };
            repo.CreateTimline(timeline);
            return WrapResponse($"{request.TenantId} {request.TimelineId} {request.Title}");
        }
    }
}