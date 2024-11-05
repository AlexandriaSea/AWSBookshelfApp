// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 1, 2024


using Amazon.DynamoDBv2.DataModel;


namespace WenjieZhou_COMP306Lab2
{
    [DynamoDBTable("Bookshelf")]
    public class Bookshelf
    {
        [DynamoDBHashKey]
        public string Email { get; set; }

        public string Password { get; set; }

        public List<Book> Books { get; set; }

        public string LastReadBookTitle { get; set; }
    }
}