using System;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Newtonsoft.Json;
using TimelineLite.Core;

namespace TimelineLite.Requests
{
    public static class RequestHelper
    {
        private static IAmazonSimpleDB _simpleDbClient;
        private static string _tableName = "GCUProject-Config";
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
            var simpleDbrequest = new GetAttributesRequest(_tableName, request.TenantId);
            var response = _simpleDbClient.GetAttributesAsync(simpleDbrequest).Result;
            var authToken = response.Attributes.Single(x => x.Name == "Auth_Token").Value;
            if (authToken == request.AuthToken)
                return request;
            throw new Exception("Invalid Authorisation Token");
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