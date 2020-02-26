using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using VisualizationRecorder.CommonTool;
using Janyee.Utilty;

namespace VisualizationRecorder {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            AppSetting();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void AppSetting() {
            Tool.LocalSetting localSetting = null;
            TimeSpan delay = TimeSpan.FromSeconds(1);
            ThreadPoolTimer timer = ThreadPoolTimer.CreateTimer(
                async (source) => {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile appSettingFile = await localFolder.CreateFileAsync("VisualizationRecorderSetting", CreationCollisionOption.OpenIfExists);
                    var buffer = await FileIO.ReadBufferAsync(appSettingFile);
                    // 如果 Capacity==0，表明文件是新创建的，里面没有内容
                    if (buffer.Capacity == 0) {
                        localSetting = Tool.LocalSetting.LocalSettingInstance;
                        byte[] bytes = localSetting.Serializer();
                        await FileIO.WriteBytesAsync(appSettingFile, bytes);
                    }
                    else {
                        localSetting = buffer.ToArray().Deserializer<Tool.LocalSetting>();
                        /*
                         * 最后一步很关键，从配置文件中提取二进制数据反序列化为LocalSetting，
                         * 然后覆盖掉原有的单例对象，其他文件从 Tool.LocalSetting.LocalSettingInstance
                         * 读取的对象是新覆盖的对象
                         */
                        Tool.LocalSetting.SetNewInstance(localSetting);
                    }
                }, delay);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
