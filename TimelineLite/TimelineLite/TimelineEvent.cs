using System;
using System.ComponentModel.DataAnnotations;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TimelineLite.Requests;
using TimelineLite.Requests.TimelineEvents;
using TimelineLite.StorageModels;
using TimelineLite.StorageRepos;
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
            return Handle(() => EditTimelineEventTitle(request));
        }
        
        public APIGatewayProxyResponse EditDescription(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditTimelineEventDescription(request));
        }
        
        public APIGatewayProxyResponse EditEventDateTime(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => EditTimelineEventDateTime(request));
        }
        
        public APIGatewayProxyResponse DeleteEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => DeleteTimelineEvent(request));
        }
        
        public APIGatewayProxyResponse LinkEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => LinkTimelineEvents(request));
        }
        
        public APIGatewayProxyResponse UnlinkEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => UnlinkTimelineEvents(request));
        }
        
        public APIGatewayProxyResponse GetLinkedEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetLinkedTimelineEvents(request));
        }
        
        public APIGatewayProxyResponse GetAttachments(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Handle(() => GetLinkedTimelineEvents(request));
        }

        private static APIGatewayProxyResponse CreateTimelineEvent(APIGatewayProxyRequest request)
        {
            Log("Create Timeline Request recieved");
            var timelineEventRequest = ParsePutRequestBody<CreateTimelineEventRequest>(request);

            Log($"Request parsed {timelineEventRequest}");
            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);
            ValidateTimelineEventTitle(timelineEventRequest.Title);
            ValidateTimelineEventDateTime(timelineEventRequest.EventDateTime);
            Log($"Request Validation passed");

            var model = new TimelineEventModel
            {
                Id = timelineEventRequest.TimelineEventId,
                Title = timelineEventRequest.Title,
                EventDateTime = timelineEventRequest.EventDateTime,
                Description = timelineEventRequest.Description
            };
            Log($"Getting repo");
            GetRepo(timelineEventRequest.TenantId).CreateTimlineEvent(model);
            
            Log($"Wrapping response");
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse EditTimelineEventTitle(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<EditTimelineEventTitleRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);
            ValidateTimelineEventTitle(timelineEventRequest.Title);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.Title = timelineEventRequest.Title;
            repo.SaveTimelineEventModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse EditTimelineEventDescription(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<EditTimelineEventDescriptionRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.Description = timelineEventRequest.Desciption;
            repo.SaveTimelineEventModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse EditTimelineEventDateTime(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<EditTimelineEventDateTimeRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);
            ValidateTimelineEventDateTime(timelineEventRequest.EventDateTime);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.EventDateTime = timelineEventRequest.EventDateTime;
            repo.SaveTimelineEventModel(model);
            return WrapResponse($"{JsonConvert.SerializeObject(model)}");
        }

        private static APIGatewayProxyResponse DeleteTimelineEvent(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<DeleteTimelineEventRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            model.IsDeleted = true;
            repo.SaveTimelineEventModel(model);
            return WrapResponse($"Successfully Deleted Timeline Event: {timelineEventRequest.TimelineEventId}");
        }

        private static APIGatewayProxyResponse LinkTimelineEvents(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<LinkTimelineEventToTimelineEventRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.LinkedToTimelineEventId))
                throw new ValidationException("Invalid Linked to Timeline Event Id");

            var repo = GetRepo(timelineEventRequest.TenantId);
            var model = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            var linkedTomodel = repo.GetTimelineEventModel(timelineEventRequest.LinkedToTimelineEventId);
            var timelineEventLinkedModel = new TimelineEventLinkModel
            {
                Id = Guid.NewGuid().ToString(),
                TimelineEventId = model.Id,
                LinkedToTimelineEventId = linkedTomodel.Id
            };
            repo.SaveTimelineEventLinkedModel(timelineEventLinkedModel);

            return WrapResponse($"{JsonConvert.SerializeObject(timelineEventLinkedModel)}");
        }

        private static APIGatewayProxyResponse UnlinkTimelineEvents(APIGatewayProxyRequest request)
        {
            var timelineEventRequest = ParsePutRequestBody<UnlinkTimelineEventToTimelineEventRequest>(request);

            ValidateTimelineEventId(timelineEventRequest.TimelineEventId);
            if (string.IsNullOrWhiteSpace(timelineEventRequest.UnlinkedFromTimelineEventId))
                throw new ValidationException("Invalid Unlinked from Timeline Event Id");

            var repo = GetRepo(timelineEventRequest.TenantId);
            var eventModel = repo.GetTimelineEventModel(timelineEventRequest.TimelineEventId);
            var unlinkedFromEventModel = repo.GetTimelineEventModel(timelineEventRequest.UnlinkedFromTimelineEventId);

            var timelineEventLinkedModel = repo.GetTimelineEventLinkModel(eventModel.Id, unlinkedFromEventModel.Id);
            timelineEventLinkedModel.IsDeleted = true;
            repo.SaveTimelineEventLinkedModel(timelineEventLinkedModel);

            return WrapResponse(
                $"Successfully unlinked Timeline Event: {timelineEventRequest.TimelineEventId} from Timeline Event: {timelineEventRequest.UnlinkedFromTimelineEventId}");
        }

        private static APIGatewayProxyResponse GetLinkedTimelineEvents(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var timelineEventId = request.Headers["TimelineEventId"];
            var skip = request.Headers["Skip"];
            ValidateTimelineEventId(timelineEventId);
            ValidateTimelineEventSkip(skip);

            var repo = GetRepo(tenantId);
            var eventModel = repo.GetTimelineEventModel(timelineEventId);
            var timelineEventLinkedModels = repo.GetTimelineEventLinks(eventModel.Id, int.Parse(skip));
            
            Log($"Skipping: {skip}");
            Log("Returning linked timeline events");
            foreach (var linkedModel in timelineEventLinkedModels)
            {
                Log(linkedModel.ToString());
            }

            return WrapResponse($"{JsonConvert.SerializeObject(timelineEventLinkedModels)}");
        }
        
        
        private static APIGatewayProxyResponse GetTimelineEventAttachments(APIGatewayProxyRequest request)
        {
            var tenantId = request.AuthoriseGetRequest();
            var timelineEventId = request.Headers["TimelineEventId"];
            var skip = request.Headers["Skip"];
            ValidateTimelineEventId(timelineEventId);
            ValidateTimelineEventSkip(skip);

            var repo = GetRepo(tenantId);
            var eventModel = repo.GetTimelineEventModel(timelineEventId);
            var timelineEventLinkedModels = repo.GetTimelineEventLinks(eventModel.Id, int.Parse(skip));
            
            Log($"Skipping: {skip}");
            Log("Returning linked timeline events");
            foreach (var linkedModel in timelineEventLinkedModels)
            {
                Log(linkedModel.ToString());
            }

            return WrapResponse($"{JsonConvert.SerializeObject(timelineEventLinkedModels)}");
        }

        private static DynamoDbTimelineEventRepository GetRepo(string tenantId)
        {
            return new DynamoDbTimelineEventRepository(new AmazonDynamoDBClient(RegionEndpoint.EUWest1), tenantId);
        }

        private static void ValidateTimelineEventId(string timelineEventId)
        {
            if (string.IsNullOrWhiteSpace(timelineEventId))
                throw new ValidationException("Invalid Timeline Id");
        }
        
        private static void ValidateTimelineEventTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Invalid Timeline Event Title");
        }
        
        private static void ValidateTimelineEventDateTime(string dateTime)
        {
            if (string.IsNullOrWhiteSpace(dateTime))
                throw new ValidationException("Invalid Timeline Event DateTime");
        }
        
        private static void ValidateTimelineEventSkip(string skip)
        {
            if (string.IsNullOrWhiteSpace(skip))
                throw new ValidationException("Invalid skip");
        }
    }
}