using System;
using System.Net;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleDB;
using Newtonsoft.Json;
using TimelineLite.Core;

namespace TimelineLite.Requests
{
    public static class RequestHelper
    {
        private static IAmazonSimpleDB _simpleDbClient;
        static RequestHelper()
        {
            _simpleDbClient = new AmazonSimpleDBClient(RegionEndpoint.EUWest1);
        }
        
        public static T ParseRequestBody<T>(APIGatewayProxyRequest request) where T : BaseRequest
        {
            return JsonConvert.DeserializeObject<T>(request.Body).AuthoriseRequest();
        }

        private static T AuthoriseRequest<T>(this T request) where T : BaseRequest
        {
            return request;
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