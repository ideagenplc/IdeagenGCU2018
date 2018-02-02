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
        private static readonly IAmazonSimpleDB _simpleDbClient;
        private static readonly string _tableName = "GCUProject-Config";
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
            var authToken = GetAuthToken(request);
            
            if (authToken == request.AuthToken)
            return request;
            else
            {
                throw new AuthenticationException($"Invalid Authorisation Token: {request.AuthToken}");
            }
        }

        private static string GetAuthToken<T>(T request) where T : BaseRequest
        {
            string authToken;
            try
            {
                var simpleDbrequest = new GetAttributesRequest(_tableName, request.TenantId);
                var response = _simpleDbClient.GetAttributesAsync(simpleDbrequest).Result;
                authToken = response.Attributes.Single(x => x.Name == "Auth_Token").Value;
            }
            catch (Exception)
            {
                throw new AuthenticationException($"Error retrieving authentication token. Is {request.TenantId} a valid tenantId?");
            }
            return authToken;
        }
    }
}