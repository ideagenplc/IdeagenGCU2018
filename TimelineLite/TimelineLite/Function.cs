using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TimelineLite.Requests;
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
            return WrapResponse($"{request.TenantId} {request.TimelineId} {request.Title} {request.AuthToken}");
        }
    }
}