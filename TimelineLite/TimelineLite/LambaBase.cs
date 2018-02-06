using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.XRay;
using Amazon.XRay.Recorder.Core;
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
            AWSXRayRecorder.Instance.BeginSubsegment("Handling Request");
            try
            {
                return handler.Invoke();
            }
            catch (GCUException e)
            {
                AWSXRayRecorder.Instance.AddException(e);
                return WrapResponse(e.Message, 400);
            }
            catch (Exception e)
            {
                AWSXRayRecorder.Instance.AddException(e);
                return WrapResponse($"Unexpected Exception : {e.Message}", 500);
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSubsegment();
            }
        }

        protected void Log(string message)
        {
        }
    }
}