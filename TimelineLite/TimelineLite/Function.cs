using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Requests;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TimelineLite
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse CreateTimeline(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var tenantId = "TestTenant";
            var authToken = "TestAuthToken";
            var body = input.Body;
            
            var t = input.Body;
            Console.WriteLine(input.Body);
            var request = RequestHelper.ParseRequestBody<CreateTimelineRequest>(input);
            Console.WriteLine(body);
            var response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            response.Body = $"{body} {request.TimelineId}";
            return response;
        }
    }
}