// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 1, 2024


using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace WenjieZhou_COMP306Lab2
{
    public partial class BookshelfWindow : Window
    {
        private readonly BookshelfOperation _bookshelfOperation;
        private readonly string _userEmail;

        public BookshelfWindow(string email)
        {
            InitializeComponent();

            _bookshelfOperation = new BookshelfOperation();
            _userEmail = email;

            LoadBookshelf();
        }

        // Load user bookshelf from DynamoDB
        private async void LoadBookshelf()
        {
            // Display welcome message with user email from MainWindow
            welcomeTextBlock.Text = $"Hi, {_userEmail}";

            // Clear existing books in the stack panel
            BooksStackPanel.Children.Clear();

            // Get user bookshelf and sort books by last read book title in descending order
            var bookshelf = await _bookshelfOperation.GetUserBookshelf(_userEmail);
            var sortedBooks = bookshelf.Books.OrderByDescending(b => b.Title == bookshelf.LastReadBookTitle).ToList();

            // Display books in the stack panel with border and text block
            foreach (var book in sortedBooks)
            {
                var bookBox = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.CadetBlue,
                    BorderThickness = new Thickness(2),
                    Cursor = Cursors.Hand
                };

                var textBlock = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                var titleTextBlock = new TextBlock
                {
                    Text = book.Title,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };

                var authorTextBlock = new TextBlock
                {
                    Text = book.Author,
                    FontSize = 14
                };

                textBlock.Children.Add(titleTextBlock);
                textBlock.Children.Add(authorTextBlock);

                bookBox.Child = textBlock;

                // Open PDF viewer on double click of the book box
                bookBox.MouseLeftButtonDown += async (s, e) =>
                {
                    if (e.ClickCount == 2)
                    {
                        await OpenPdfViewer(book.File, book.Title);
                    }
                };

                BooksStackPanel.Children.Add(bookBox);
            }
        }

        // Open PDF viewer with the selected book
        private async Task OpenPdfViewer(string pdfFile, string bookTitle)
        {
            // Immediately update last read book title in user bookshelf when opening the PDF viewer of a book
            await _bookshelfOperation.UpdateLastReadBookTitle(_userEmail, bookTitle);

            var pdfViewerWindow = new PdfViewerWindow(this, pdfFile, _userEmail);

            pdfViewerWindow.Show();
            this.Hide();

            // Reload bookshelf with updated last read book title
            LoadBookshelf();
        }

        // Lougout button event handler
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}