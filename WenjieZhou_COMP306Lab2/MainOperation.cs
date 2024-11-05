// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 3, 2024


using Amazon.DynamoDBv2.DocumentModel;
using System.Configuration;
using Amazon.DynamoDBv2;
using System.Windows;


namespace WenjieZhou_COMP306Lab2
{
    public class MainOperation
    {
        private readonly string _tableName = ConfigurationManager.AppSettings["TableName"];

        public MainOperation()
        {
        }

        // Hnalde the login operation
        public async Task<bool> Login(string email, string password)
        {
            try
            {
                // Load the table from DynamoDB and get the document by email
                var table = Table.LoadTable(Helper.amazonDynamoDBClient, _tableName);
                var document = await table.GetItemAsync(email);

                if (document != null)
                {
                    // Compare the stored password with the input password and return the result
                    string storedPassword = document["Password"];
                    return string.Equals(storedPassword, password, StringComparison.Ordinal);
                }
                return false;
            }
            catch (AmazonDynamoDBException dbEx)
            {
                MessageBox.Show($"DynamoDB Error: {dbEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}