using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var authToken = GetAuthToken(request.TenantId);
            
            if (authToken == request.AuthToken)
            return request;
            else
            {
                throw new AuthenticationException($"Invalid Authorisation Token: {request.AuthToken}");
            }
        }
        
        public static string AuthoriseGetRequest<T>(this T request) where T : APIGatewayProxyRequest
        {
            if (request.HttpMethod != "GET")
                throw new HttpRequestException("Request is not a GET");
            var tenant = request.Headers["GCU-TenantId"];
            if (string.IsNullOrWhiteSpace(tenant))
                throw new AuthenticationException("Header: GCU-TenantId hs not been set on GET Request");
            var authToken = GetAuthToken(tenant);
            var recievedAuthToken = request.Headers["GCU-AuthToken"];
            if (string.IsNullOrWhiteSpace(recievedAuthToken))
                throw new AuthenticationException("Header: GCU-AuthToken hs not been set on GET Request");
            
            if (authToken == recievedAuthToken)
                return tenant;
            else
            {
                throw new AuthenticationException($"Invalid Authorisation Token: {recievedAuthToken}");
            }
        }

        private static string GetAuthToken(string tenantId)
        {
            string authToken;
            try
            {
                var simpleDbrequest = new GetAttributesRequest(_tableName, tenantId);
                var response = _simpleDbClient.GetAttributesAsync(simpleDbrequest).Result;
                authToken = response.Attributes.Single(x => x.Name == "Auth_Token").Value;
            }
            catch (Exception)
            {
                throw new AuthenticationException($"Error retrieving authentication token. Is {tenantId} a valid tenantId?");
            }
            return authToken;
        }
    }
}