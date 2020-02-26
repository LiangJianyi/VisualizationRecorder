using VisualizationRecorder.SqlDbHelper;
using VisualizationRecorder.CommonTool;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace VisualizationRecorder {
    public sealed partial class RegisterPage : Page {
        private SqlDbHelper.SqlDbHelper _sqlDbHelper = SqlDbHelper.SqlDbHelper.GetSqlDbHelper(SqlDbHelperType.LocalSqlDbHelper);

        private ApplicationTheme _theme;
        private StorageFile _file;

        public RegisterPage() {
            this.InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            UpdateRegisterPageLayout();
        }

        private async void RegisterButton_ClickAsync(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(AccountTextBox.Text)) {
                await PopErrorDialogAsync("用户名不能为空！");
            }
            else if (string.IsNullOrWhiteSpace(AccountTextBox.Text)) {
                await PopErrorDialogAsync("用户名不能有空白字符！");
            }
            else if (AccountTextBox.Text.Length > 128) {
                await PopErrorDialogAsync("用户名长度不能超过128个字符！");
            }
            else if (string.IsNullOrEmpty(PasswordBox.Password)) {
                await PopErrorDialogAsync("密码不能为空！");
            }
            else if (string.IsNullOrWhiteSpace(PasswordBox.Password)) {
                await PopErrorDialogAsync("密码不能有空白字符！");
            }
            else if (PasswordBox.Password.Length > 256) {
                await PopErrorDialogAsync("密码长度不能超过256个字符！");
            }
            else {
                Configuration configuration = new Configuration(
                        username: AccountTextBox.Text,
                        password: PasswordBox.Password,
                        title: TitleBox.Text,
                        theme: this._theme,
                        avatar: _file
                );
                if (await this._sqlDbHelper.RegisterUserAsync(configuration) > 0) {
                    // 注册成功后要用 configuration 再登陆一次
                    configuration = await this._sqlDbHelper.LoginAsync(configuration.UserName, configuration.Password);
                    if (configuration != null) {
                        Frame rootFrame = Window.Current.Content as Frame;
                        rootFrame.Navigate(typeof(MainPage), configuration);
                    }
                }
                else {
                    await PopErrorDialogAsync("注册发生错误！");
                }
            }
        }

        private async void Avatar_PointerReleasedAsync(object sender, PointerRoutedEventArgs e) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            _file = await picker.PickSingleFileAsync();
            if (_file != null) {
                Avatar.Source = await Tool.StorageFileToBitmapImageAsync(_file, (int)Avatar.Width, (int)Avatar.Height);
            }
        }

        private void LightRadioButton_Checked(object sender, RoutedEventArgs e) {
            this._theme = ApplicationTheme.Light;
        }

        private void DarkRadioButton_Checked(object sender, RoutedEventArgs e) {
            this._theme = ApplicationTheme.Dark;
        }
    }
}
