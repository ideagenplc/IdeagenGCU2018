using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.XRay.Recorder.Core;
using TimelineLite.Core;
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

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}