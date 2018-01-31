using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleDB;
using Newtonsoft.Json;
using TimelineLite.Core;

namespace TimelineLite.Requests
{
    public static class RequestHelper
    {
        
        static RequestHelper()
        {
            
        }
        public static T ParseRequestBody<T>(APIGatewayProxyRequest request) where T : BaseRequest
        {
            return JsonConvert.DeserializeObject<T>(request.Body);
        }

        public static AuthorisationDetails GetAuthorisationDetails(this BaseRequest request)
        {
            var details = new AuthorisationDetails
            {
                TenantId = request.TenantId,
                AuthToken = request.AuthToken
            };
            return details;
        }
    }
}