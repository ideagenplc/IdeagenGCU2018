using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.XRay;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
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
        protected static APIGatewayProxyResponse Handle(Func<APIGatewayProxyResponse> handler)
        {
            AWSSDKHandler.RegisterXRay<IAmazonDynamoDB>();
            try
            {
                return AWSXRayRecorder.Instance.TraceMethod("Handling", handler.Invoke);
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

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}