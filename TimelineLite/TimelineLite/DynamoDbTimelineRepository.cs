using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TimelineLite.StorageModels;

namespace TimelineLite
{
    public class DynamoDbTimelineRepository
    {
        private static IAmazonDynamoDB _client;
        private static DynamoDBContext _context;

        public DynamoDbTimelineRepository(IAmazonDynamoDB client)
        {
            _client = client;
            _context = new DynamoDBContext(_client);
        }

        public void CreateTimline(TimelineModel model)
        {
            _context.SaveAsync(model).Wait();
        }
    }
}