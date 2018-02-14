using System;
using System.Linq;
using System.Net.Http;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.XRay.Recorder.Core;
using Newtonsoft.Json;

namespace Timelinelite.Core
{
    public static class RequestHelper
    {
        private static readonly IAmazonSimpleDB _simpleDbClient;
        private static readonly string _tableName = "GCUProject-Config";

        static RequestHelper()
        {
            _simpleDbClient = new AmazonSimpleDBClient(RegionEndpoint.EUWest1);
        }

        public static T ParsePutRequestBody<T>(APIGatewayProxyRequest request) where T : BaseRequest
        {
            if (request.HttpMethod != "PUT")
                throw new HttpRequestException("Request is not a PUT");
            var parsedRequest = JsonConvert.DeserializeObject<T>(request.Body);
            AWSXRayRecorder.Instance.AddAnnotation("Tenant", parsedRequest.TenantId);
            return JsonConvert.DeserializeObject<T>(request.Body).AuthorisePutRequest();
        }

        private static T AuthorisePutRequest<T>(this T request) where T : BaseRequest
        {
            if (string.IsNullOrWhiteSpace(request.AuthToken))
                throw new AuthenticationException("Body: AuthToken has not been set on PUT Request");
            if (string.IsNullOrWhiteSpace(request.TenantId))
                throw new AuthenticationException("Body: TenantId has not been set on PUT Request");
            var authToken = GetAuthToken(request.TenantId);
            if (authToken == request.AuthToken)
                return request;
            throw new AuthenticationException($"Invalid Authorisation Token: {request.AuthToken}");
        }

        public static string AuthoriseGetRequest<T>(this T request) where T : APIGatewayProxyRequest
        {
            if (request.HttpMethod != "GET")
                throw new HttpRequestException("Request is not a GET");
            request.Headers.TryGetValue("TenantId", out var tenant);
            if (string.IsNullOrWhiteSpace(tenant))
                throw new AuthenticationException("Header: TenantId has not been set on GET Request");
            var authToken = GetAuthToken(tenant);
            request.Headers.TryGetValue("AuthToken", out var recievedAuthToken);
            if (string.IsNullOrWhiteSpace(recievedAuthToken))
                throw new AuthenticationException("Header: AuthToken has not been set on GET Request");

            if (authToken == recievedAuthToken)
                return tenant;
            throw new AuthenticationException($"Invalid Authorisation Token: {recievedAuthToken}");
        }

        public static void ValidateString(this string input, string message)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ValidationException(message);
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
                throw new AuthenticationException(
                    $"Error retrieving authentication token. Is {tenantId} a valid tenantId?");
            }
            return authToken;
        }
    }
}