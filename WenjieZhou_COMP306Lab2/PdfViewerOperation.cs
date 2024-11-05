// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 3, 2024


using Amazon.DynamoDBv2.DataModel;
using Amazon.S3.Model;
using Amazon.S3;
using System.IO;
using System.Configuration;
using Amazon.DynamoDBv2;
using System.Windows;


namespace WenjieZhou_COMP306Lab2
{
    public class PdfViewerOperation
    {
        private readonly DynamoDBContext _DBContext;
        private readonly string _bucketName = ConfigurationManager.AppSettings["BucketName"];

        public PdfViewerOperation()
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

        // Load book from S3 bucket
        public async Task<Stream> LoadBook(string objectKey)
        {
            try
            {
                // Get book from S3 bucket by bucket name and object key
                var response = await Helper.s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey
                });

                // Copy book stream to memory stream
                MemoryStream documentStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(documentStream);
                documentStream.Position = 0;

                return documentStream;
            }
            catch (AmazonS3Exception e)
            {
                MessageBox.Show($"S3 Error: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading book: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Get last read page number of a book
        public async Task<int?> GetLastReadPageNumber(string email, string pdfFile)
        {
            try
            {
                // Get the book from user bookshelf by email and file name
                var bookshelf = await GetUserBookshelf(email);
                var book = bookshelf.Books.FirstOrDefault(b => b.File == pdfFile);

                // Return last read page number
                return book?.LastReadPageNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching last read page number: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Update bookmark of a book
        public async Task UpdateBookmark(string email, string file, int pageNumber)
        {
            try
            {
                // Get the book from user bookshelf by email and file name
                var userBookshelf = await GetUserBookshelf(email);
                var book = userBookshelf.Books.FirstOrDefault(b => b.File == file);

                // Update last read page number and timestamp of the book
                if (book != null)
                {
                    book.LastReadPageNumber = pageNumber;
                    book.LastReadPageTimestamp = DateTime.Now;
                    await SaveUserBookshelf(userBookshelf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating bookmark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}