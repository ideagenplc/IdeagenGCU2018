using System;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SimpleDB;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;

namespace Timelinelite.Core
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
                return ResponseHelper.WrapResponse(e.Message, 400);
            }
            catch (Exception e)
            {
                return ResponseHelper.WrapResponse($"Unexpected Exception : {e.Message}", 500);
            }
        }

        protected static APIGatewayProxyResponse HandleOptions(Func<APIGatewayProxyResponse> handler)
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
                return ResponseHelper.WrapResponse(e.Message, 400);
            }
            catch (Exception e)
            {
                return ResponseHelper.WrapResponse($"Unexpected Exception : {e.Message}", 500);
            }
        }

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }
        
        
    }
}