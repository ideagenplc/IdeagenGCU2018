using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace TimelineLite
{
    public static class DynamoDbHelper
    {
        public static void PutItem(string tableName, Dictionary<string, AttributeValue> attributes)
        {
            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.PutItemAsync(new PutItemRequest(tableName, attributes));
            }
        }
        
        public static void DeleteItem(string tableName, Dictionary<string, AttributeValue> key)
        {
            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.DeleteItemAsync(new DeleteItemRequest(tableName, key));
            }
        }
        
        public static void UpdateItem(string tableName, Dictionary<string, AttributeValue> key, Dictionary<string, AttributeValueUpdate> attributeUpdates)
        {
            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.UpdateItemAsync(new UpdateItemRequest(tableName, key, attributeUpdates));
            }
        }
        
        public static void GetItem(string tableName, Dictionary<string, AttributeValue> key)
        {
            using (var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1))
            {
                dynamoDbClient.GetItemAsync(new GetItemRequest(tableName, key));
            }
        }
    }
}