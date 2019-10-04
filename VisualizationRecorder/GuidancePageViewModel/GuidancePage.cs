using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VisualizationRecorder {
    using Debug = System.Diagnostics.Debug;

    public sealed partial class GuidancePage {
        public void OpenLoginProgressRing() {
            // 移除“登录”和“注册”两个按钮
            StackPanelWithButtons.Children.Remove(LoginButton);
            StackPanelWithButtons.Children.Remove(RegisterButton);
            // 添加进度条
            StackPanelWithButtons.Children.Add(new ProgressRing() {
                FontSize = 25,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                IsActive = true,
                Margin = new Thickness(25)
            });
        }

        public void CloseLoginProgressRing() {
            // 移除进度条
            StackPanelWithButtons.Children.RemoveAt(0);
            // 添加“登录”和“注册”两个按钮
            Button loginButton = new Button() {
                Content = "登录",
                Margin = new Thickness(0, 0, 20, 0)
            };
            loginButton.Click += Login_Click;
            Button registerButton = new Button() {
                Content = "注册",
                Margin = new Thickness(20, 0, 0, 0)
            };
            registerButton.Click += Register_Click;

            StackPanelWithButtons.Children.Add(loginButton);
            StackPanelWithButtons.Children.Add(registerButton);
        }
    }
}
