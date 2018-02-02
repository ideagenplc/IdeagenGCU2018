using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TimelineLite.Requests.TimelineEvents;
using TimelineLite.StorageModels;
using static TimelineLite.Requests.RequestHelper;
using static TimelineLite.Responses.ResponseHelper;

namespace TimelineLite
{
    public class TimelineEvent : LambdaBase
    {
        public APIGatewayProxyResponse Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => CreateTimelineEvent(request));
        }
        
        public APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditTitle(request));
        }
        
        public APIGatewayProxyResponse EditDescription(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditDescription(request));
        }
        
        public APIGatewayProxyResponse EditEventDateTime(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditEventDateTime(request));
        }
        
        public APIGatewayProxyResponse DeleteEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => DeleteEvent(request));
        }
        
        public APIGatewayProxyResponse LinkEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => LinkEvents(request));
        }
        
        public APIGatewayProxyResponse UnlinkEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => UnlinkEvents(request));
        }
        
        public APIGatewayProxyResponse GetLinkedTimelineEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetLinkedTimelineEvents(request));
        }

        private static APIGatewayProxyResponse CreateTimelineEvent(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<CreateTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Title))
                return WrapResponse("Invalid Timeline Event Title ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Description))
                return WrapResponse("Invalid Timeline Event Description ", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.EventDateTime))
                return WrapResponse("Invalid Timeline Event Date Time ", 400);

            var timelineEvent = new TimelineEventModel
            {
                Id = timelineEventRequest.TimelineEventId,
                Title = timelineEventRequest.Title,
                EventDateTime = timelineEventRequest.EventDateTime,
                Description = timelineEventRequest.Description
            };
            GetRepo(timelineEventRequest.TenantId).CreateTimlineEvent(timelineEvent);
            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Title} {timelineEventRequest.Description} {timelineEventRequest.EventDateTime}");
        }

        private static APIGatewayProxyResponse EditTitle(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventTitleRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Title))
                return WrapResponse("Invalid Timeline Event Title ", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.Title = timelineEventRequest.Title;
            repo.SaveTimelineEventModel(model);
            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Title}");
        }

        private static APIGatewayProxyResponse EditDescription(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventDescriptionRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.Desciption))
                return WrapResponse("Invalid Timeline Event Description", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.Description = timelineEventRequest.Desciption;
            repo.SaveTimelineEventModel(model);
            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.Desciption}");
        }

        private static APIGatewayProxyResponse EditEventDateTime(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<EditTimelineEventDateTimeRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.EventDateTime))
                return WrapResponse("Invalid Timeline Event Date Time", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.EventDateTime = timelineEventRequest.EventDateTime;
            repo.SaveTimelineEventModel(model);
            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.EventDateTime}");
        }

        private static APIGatewayProxyResponse DeleteEvent(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<DeleteTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.IsDeleted = true;
            repo.SaveTimelineEventModel(model);
            return WrapResponse($"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId}");
        }

        private static APIGatewayProxyResponse LinkEvents(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<LinkTimelineEventToTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.LinkedToTimelineEventId))
                return WrapResponse("Invalid Linked to Timeline Event Id", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            var linkedTomodel = repo.GetTimelineEventModel(timelineEventRequest.LinkedToTimelineEventId);

            repo.SaveTimelineEventLinkedModel(new TimelineEventLinkModel
            {
                Id = Guid.NewGuid().ToString(),
                TimelineEventId = model.Id,
                LinkedToTimelineEventId = linkedTomodel.Id
            });

            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.LinkedToTimelineEventId}");
        }

        private static APIGatewayProxyResponse UnlinkEvents(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<UnlinkTimelineEventToTimelineEventRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.UnlinkedFromTimelineEventId))
                return WrapResponse("Invalid Unlinked from Timeline Event Id", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var eventModel = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            var unlinkedFromEventModel = repo.GetTimelineEventModel(timelineEventRequest.UnlinkedFromTimelineEventId);

            var timelineEventLinkedModel = repo.GetTimelineEventLinkModel(eventModel.Id, unlinkedFromEventModel.Id);
            timelineEventLinkedModel.IsDeleted = true;
            repo.SaveTimelineEventLinkedModel(timelineEventLinkedModel);

            return WrapResponse(
                $"{timelineEventRequest.TenantId} {timelineEventRequest.TimelineEventId} {timelineEventRequest.UnlinkedFromTimelineEventId}");
        }

        private static APIGatewayProxyResponse GetLinkedTimelineEvents(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParseRequestBody<GetTimelineEventLinksRequest>(request);

            if (string.IsNullOrWhiteSpace(timelineEventRequest.TimelineEventId))
                return WrapResponse("Invalid Timeline Event Id", 400);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var eventModel = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            var timelineEventLinkedModel =
                repo.GetTimelineEventLinks(timelineEventRequest.TimelineEventId, timelineEventRequest.skip);

            Console.WriteLine($"Skipping: {timelineEventRequest.skip}");
            Console.WriteLine("Returning linked timeline events");
            foreach (var linkedModel in timelineEventLinkedModel)
            {
                Console.WriteLine(linkedModel);
            }

            return WrapResponse(timelineEventLinkedModel);
        }

        private static DynamoDbTimelineEventRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }
    }
}