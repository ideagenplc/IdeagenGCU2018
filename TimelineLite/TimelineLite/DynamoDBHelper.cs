using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace TimelineLite
{
    public class DynamoDbHelper
    {
        public void PutItem(string tableName, Dictionary<string, AttributeValue> attributes)
        {
            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.PutItemAsync(new PutItemRequest(tableName, attributes));
            }
        }
    }
}