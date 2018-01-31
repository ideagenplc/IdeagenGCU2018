using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Requests;
using TimelineLite.StorageModels;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TimelineLite
{
    public class Timeline
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var createTimelineRequest = ParseRequestBody<CreateTimelineRequest>(request);

            if (string.IsNullOrWhiteSpace(createTimelineRequest.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);
            if (string.IsNullOrWhiteSpace(createTimelineRequest.Title))
                return WrapResponse("Invalid Timeline Title ", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), createTimelineRequest.TenantId);
            var timeline = new TimelineModel
            {
                Id = createTimelineRequest.TimelineId,
                Title = createTimelineRequest.Title,
                CreationTimeStamp = DateTime.Now.Ticks.ToString(),
                IsDeleted = false
            };
            repo.CreateTimline(timeline);
            return WrapResponse($"{createTimelineRequest.TenantId} {createTimelineRequest.TimelineId} {createTimelineRequest.Title}");
        }
        
        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var editTimelineTitleRequest = ParseRequestBody<CreateTimelineRequest>(request);

            if (string.IsNullOrWhiteSpace(editTimelineTitleRequest.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);
            if (string.IsNullOrWhiteSpace(editTimelineTitleRequest.Title))
                return WrapResponse("Invalid Timeline Title ", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), editTimelineTitleRequest.TenantId);
            var model = repo.GetModel(editTimelineTitleRequest.TimelineId);
            
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }
    }
}