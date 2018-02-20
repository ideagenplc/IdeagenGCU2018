using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace Timelinelite.Core
{
    public static class ResponseHelper
    {
        public static APIGatewayProxyResponse WrapResponse(object body, int statuscode = 200)
        {
            var response = new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body,
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.None,
                        Formatting = Formatting.Indented
                    }),
                StatusCode = statuscode
            };
            return response;
        }
    }
}