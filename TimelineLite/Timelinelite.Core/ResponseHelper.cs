using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace Timelinelite.Core
{
    public static class ResponseHelper
    {
        public static APIGatewayProxyResponse WrapResponse(object body, int statuscode = 200)
        {
            var response = new APIGatewayProxyResponse();
            response.Body = JsonConvert.SerializeObject(body);
            response.StatusCode = statuscode;
            return response;
        }
    }
}