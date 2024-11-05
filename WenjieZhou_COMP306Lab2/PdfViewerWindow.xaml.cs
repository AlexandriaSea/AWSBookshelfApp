// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 1, 2024


using System.Configuration;
using System.Windows;


namespace WenjieZhou_COMP306Lab2
{
    public partial class PdfViewerWindow : Window
    {
        private readonly Window _bookshelfWindow;
        private readonly PdfViewerOperation _pdfViewerOperation;
        private readonly string _pdfFile;
        private readonly string _userEmail;

        public PdfViewerWindow(Window bookshelfWindow, string pdfFile, string userEmail)
        {
            InitializeComponent();

            _bookshelfWindow = bookshelfWindow;
            _pdfViewerOperation = new PdfViewerOperation();
            _pdfFile = pdfFile;
            _userEmail = userEmail;

            // Syncfusion license key
            string syncfusionLicenseKey = ConfigurationManager.AppSettings["syncfusionLicenseKey"];
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);

            LoadPdf();

            // Subscribe to the Closed event to save the last read page number
            this.Closed += PdfViewerWindow_Closed;
        }

        // Load PDF file into the viewer
        private async void LoadPdf()
        {
            try
            {
                // Load PDF file and display it in the viewer
                var pdfStream = await _pdfViewerOperation.LoadBook(_pdfFile);
                pdfViewer.Load(pdfStream);

                // Get the last read page number from the bookmark
                var lastReadPageNumber = await _pdfViewerOperation.GetLastReadPageNumber(_userEmail, _pdfFile);

                // Navigate to the last read page
                if (lastReadPageNumber.HasValue)
                {
                    pdfViewer.GoToPageAtIndex(lastReadPageNumber.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Get the last read page number and update the bookmark
        private async Task BookmarkProgress()
        {
            try
            {
                int lastReadPageNumber = pdfViewer.CurrentPage;
                await _pdfViewerOperation.UpdateBookmark(_userEmail, _pdfFile, lastReadPageNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update bookmark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handle the Closed event to execute bookmark operation
        private async void PdfViewerWindow_Closed(object sender, EventArgs e)
        {
            await BookmarkProgress();

            _bookshelfWindow.Show();
        }

        // Bookmark button click event handler
        private async void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            await BookmarkProgress();
            MessageBox.Show("Bookmark saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Exit button click event handler
        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            await BookmarkProgress();

            _bookshelfWindow.Show();
            this.Close();
        }
    }
}