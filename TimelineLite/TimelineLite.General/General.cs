using System;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Timelinelite.Core;
using TimelineLite.General.Responses;
using TimelineLite.Timeline;
using TimelineLite.TimelineEvent;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TimelineLite.General
{
    public class General : LambdaBase
    {
        public APIGatewayProxyResponse GetAllTimelinesAndEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetAllTimelinesAndTimelineEvents(request));
        }

        public APIGatewayProxyResponse GetOptions(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetOptionsResponse(request));
        }

        #region Private Methods

        private static APIGatewayProxyResponse GetAllTimelinesAndTimelineEvents(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var timelineRepo = GetTimelineRepository(tenantId);
            var timelineEventRepo = GetTimelineEventRepository(tenantId);
            var timelineModels = timelineRepo.GetModels();
            var response = new GetAllTimelinesAndEventsResponse();
            foreach (var timelineModel in timelineModels)
            {
                response.Timelines.Add(new Responses.Timeline
                {
                    Id = timelineModel.Id,
                    CreationTimeStamp = timelineModel.CreationTimeStamp,
                    IsDeleted = timelineModel.IsDeleted,
                    Title = timelineModel.Title,
                    TimelineEvents = GetTimelineEvents(timelineRepo.GetLinkedEvents(timelineModel.Id).Select(_ => _.TimelineEventId), timelineEventRepo)
                });
            }

            return ResponseHelper.WrapResponse(response);
        }

        private static APIGatewayProxyResponse GetOptionsResponse(APIGatewayProxyRequest request)
        {
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>
                {
                    {"Access-Control-Allow-Origin", "*"},
                    {"Access-Control-Allow-Methods", "GET,PUT,OPTIONS"},
                    {
                        "Access-Control-Allow-Headers",
                        "Content-Type,X-mz-Date,Authorization,X-Api-Key,X-Amz-Security-Token,,AuthToken,authtoken,TenantId,tenantid,timelineid,TimelineId,TimelineEventId,timelineeventid,AttachmentId,attachmentid"
                    },
                    {"Content-Type", "application/json"}
                },
                StatusCode = 200
            };
            return response;
        }

        private static List<Responses.TimelineEvent> GetTimelineEvents(IEnumerable<string> timelineEventIds, DynamoDbTimelineEventRepository eventRepo)
        {
            return timelineEventIds.Select(eventRepo.GetTimelineEventModel)
                .Select(timelineEventModel => new Responses.TimelineEvent
                {
                    Id = timelineEventModel.Id,
                    Title = timelineEventModel.Title,
                    Description = timelineEventModel.Description,
                    EventDateTime = timelineEventModel.EventDateTime,
                    IsDeleted = timelineEventModel.IsDeleted,
                    Location = timelineEventModel.Location,
                    LinkedTimelineEventIds = GetLinkedTimelineEvents(timelineEventModel.Id, eventRepo)
                }).ToList();
        }

        private static List<string> GetLinkedTimelineEvents(string timelineEventId, DynamoDbTimelineEventRepository eventRepo)
        {
            return eventRepo.GetTimelineEventLinks(timelineEventId).Select(_ => _.LinkedToTimelineEventId).ToList();
        }

        private static DynamoDbTimelineRepository GetTimelineRepository(string tenantId)
        {
            return new DynamoDbTimelineRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }

        private static DynamoDbTimelineEventRepository GetTimelineEventRepository(string tenantId)
        {
            return new DynamoDbTimelineEventRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }

        #endregion
    }
}