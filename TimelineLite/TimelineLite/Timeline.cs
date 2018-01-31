using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
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
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var request = ParseRequestBody<CreateTimelineRequest>(input);

            if (string.IsNullOrWhiteSpace(request.TimelineId))
                return WrapResponse("Invalid Timeline Id", 400);
            if (string.IsNullOrWhiteSpace(request.Title))
                return WrapResponse("Invalid Timeline Title ", 400);

            var repo = new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            var timeline = new TimelineModel
            {
                Id = request.TimelineId,
                Title = request.Title,
                CreationTimeStamp = DateTime.Now.Ticks.ToString(),
                IsDeleted = false
            };
            repo.CreateTimline(timeline);
            return WrapResponse($"{request.TenantId} {request.TimelineId} {request.Title}");
        }
    }
}