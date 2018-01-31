using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace TimelineLite.Requests
{
    public static class RequestHelper
    {
        public static T ParseRequestBody<T>(APIGatewayProxyRequest request) where T : BaseRequest
        {
            return JsonConvert.DeserializeObject<T>(request.Body);
        }
    }
}