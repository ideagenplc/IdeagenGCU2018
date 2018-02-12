using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SimpleDB;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using TimelineLite.Core;
using TimelineLite.Requests;
using static TimelineLite.Responses.ResponseHelper;
namespace TimelineLite
{
    public abstract class LambdaBase
    {
        protected static APIGatewayProxyResponse Handle(Func<APIGatewayProxyResponse> handler)
        {
            AWSSDKHandler.RegisterXRay<IAmazonDynamoDB>();
            AWSSDKHandler.RegisterXRay<IAmazonSimpleDB>();
            AWSSDKHandler.RegisterXRay<ICoreAmazonS3>();
            try
            {
                return AWSXRayRecorder.Instance.TraceMethod("Handling Request", handler);
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