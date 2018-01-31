using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace TimelineLite
{
    public class TimelineEvents
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse CreateTimelineEvent(APIGatewayProxyRequest input, ILambdaContext context)
        {
            // AmazonDynamoDBClient dynamoDbClient = new AmazonDynamoDBClient();

            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.PutItemAsync()
            }
            
            var tenantId = "TestTenant";
            var authToken = "TestAuthToken";
            var body = input.Body;
            
            var t = input.Body;
            Console.WriteLine(input.Body);
            var test = JsonConvert.DeserializeObject<TestClass>(body);
            Console.WriteLine(body);
            var response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            response.Body = $"{body} {test.TimelineId}";
            return response;
        }
    }
}