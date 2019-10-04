using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VisualizationRecorder {
    public sealed partial class RegisterPage {
        /// <summary>
        /// 弹出错误提示
        /// </summary>
        /// <param name="content"></param>
        public async Task PopErrorDialogAsync(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
        }

        /// <summary>
        /// 更新 RegisterPage 页面 UI 布局
        /// </summary>
        private void UpdateRegisterPageLayout() {
            AccountTextBox.Width = RegisterFormPanel.ActualWidth;
            PasswordBox.Width = RegisterFormPanel.ActualWidth;
            TitleBox.Width = RegisterFormPanel.ActualWidth;
            StackPanelWithImage.Width = RegisterFormPanel.ActualWidth;
            Avatar.Margin = new Thickness((RegisterFormPanel.ActualWidth - Avatar.Width) / 2, 0, 0, 0);
            UploadAvatarTextBlock.Margin = new Thickness((RegisterFormPanel.ActualWidth - UploadAvatarTextBlock.ActualWidth) / 2, 0, 0, 0);
            StackPanleWithRadioButtons.Margin = new Thickness((RegisterFormPanel.ActualWidth - StackPanleWithRadioButtons.ActualWidth) / 2, 0, 0, 0);
            RegisterButton.Margin = new Thickness((RegisterFormPanel.ActualWidth - RegisterButton.ActualWidth) / 2, 0, 0, 0);
        }

    }
}
