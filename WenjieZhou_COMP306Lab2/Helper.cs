// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 1, 2024


using Amazon.DynamoDBv2;
using Amazon.S3;
using System.Configuration;


namespace WenjieZhou_COMP306Lab2
{
    public static class Helper
    {
        public readonly static AmazonDynamoDBClient amazonDynamoDBClient;
        public readonly static IAmazonS3 s3Client;

        static Helper()
        {
            amazonDynamoDBClient = GetAmazonDynamoDBClient();
            s3Client = GetAmazonS3Client();
        }

        private static AmazonDynamoDBClient GetAmazonDynamoDBClient()
        {
            string awsAccessId = ConfigurationManager.AppSettings["accessKeyId"];
            string awsSecretKey = ConfigurationManager.AppSettings["secretAccessKey"];
            return new AmazonDynamoDBClient(awsAccessId, awsSecretKey, Amazon.RegionEndpoint.CACentral1);
        }

        private static IAmazonS3 GetAmazonS3Client()
        {
            string awsAccessId = ConfigurationManager.AppSettings["accessKeyId"];
            string awsSecretKey = ConfigurationManager.AppSettings["secretAccessKey"];
            return new AmazonS3Client(awsAccessId, awsSecretKey, Amazon.RegionEndpoint.CACentral1);
        }
    }
}