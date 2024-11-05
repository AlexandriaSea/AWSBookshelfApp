// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 3, 2024


using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Windows;


namespace WenjieZhou_COMP306Lab2
{
    public class BookshelfOperation
    {
        private readonly DynamoDBContext _DBContext;

        public BookshelfOperation()
        {
            _DBContext = new DynamoDBContext(Helper.amazonDynamoDBClient);
        }

        // Get user bookshelf by email
        public async Task<Bookshelf> GetUserBookshelf(string email)
        {
            try
            {
                return await _DBContext.LoadAsync<Bookshelf>(email);
            }
            catch (AmazonDynamoDBException dbEx)
            {
                MessageBox.Show($"DynamoDB Error: {dbEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching bookshelf: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Save user bookshelf
        public async Task SaveUserBookshelf(Bookshelf bookshelf)
        {
            try
            {
                await _DBContext.SaveAsync(bookshelf);
            }
            catch (AmazonDynamoDBException dbEx)
            {
                MessageBox.Show($"DynamoDB Error: {dbEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bookshelf: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Update last read book title in user bookshelf
        public async Task UpdateLastReadBookTitle(string email, string lastReadBookTitle)
        {
            try
            {
                var userBookshelf = await GetUserBookshelf(email);
                userBookshelf.LastReadBookTitle = lastReadBookTitle;
                await SaveUserBookshelf(userBookshelf);
            }
            catch (AmazonDynamoDBException dbEx)
            {
                MessageBox.Show($"DynamoDB Error: {dbEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating last read book title: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}