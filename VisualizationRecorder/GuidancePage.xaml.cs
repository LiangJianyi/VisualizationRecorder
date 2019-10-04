using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VisualizationRecorder.SqlDbHelper;

namespace VisualizationRecorder {
    using Debug = System.Diagnostics.Debug;

    public sealed partial class GuidancePage : Page {
        private SqlDbHelper.SqlDbHelper _sqlDbHelper = SqlDbHelper.SqlDbHelper.GetSqlDbHelper(SqlDbHelperType.LocalSqlDbHelper);

        public GuidancePage() {
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e) {
            this.OpenLoginProgressRing();

            if (string.IsNullOrEmpty(AccountTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能为空");
            }
            else if (string.IsNullOrWhiteSpace(AccountTextBox.Text) ||
                     string.IsNullOrWhiteSpace(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能包含空格");
            }
            else {
                Configuration configuration = await _sqlDbHelper.LoginAsync(AccountTextBox.Text, PasswordBox.Password);
                if (configuration != null) {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage), configuration);
                }
                else {
                    PopErrorDialogAsync("账号或密码错误");
                }
            }
        }

        private static async void PopErrorDialogAsync(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            AccountTextBox.Width = TitleBox.ActualWidth;
            PasswordBox.Width = TitleBox.ActualWidth;
            Debug.WriteLine(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(RegisterPage));
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
    }
}
