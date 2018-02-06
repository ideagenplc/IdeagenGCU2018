using System;
using Amazon.Lambda.APIGatewayEvents;
using TimelineLite.Core;
using static TimelineLite.Responses.ResponseHelper;
namespace TimelineLite
{
    public abstract class LambdaBase
    {
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

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}