using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Core;
using TimelineLite.Logging;
using TimelineLite.Requests;
using TimelineLite.StorageModels;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;
namespace TimelineLite
{
    public abstract class LambdaBase
    {
        private readonly ILog _logger;
        protected LambdaBase(ILog logger)
        {
            _logger = logger;
        }
        protected static APIGatewayProxyResponse Handle(Func<APIGatewayProxyResponse> handler)
        {
            try
            {
                return handler.Invoke();
            }
            catch (GCUException e)
            {
                return WrapResponse(e.Message, 400);
            }
            catch (Exception e)
            {
                return WrapResponse($"Unexpected Exception : {e.Message}", 500);
            }
        }

        protected void Log(string message)
        {
            _logger.Log(message);
        }
    }
}