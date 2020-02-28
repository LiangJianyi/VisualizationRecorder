using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Janyee.Utilty;
using System.Runtime.InteropServices.WindowsRuntime;

namespace VisualizationRecorder.CommonTool {
    static class Tool {
        /// <summary>
        /// 从流中提取照片设置传递进去的 Image 控件
        /// </summary>
        /// <param name="imageControl">要设置照片的控件</param>
        /// <param name="file">文件流</param>
        /// <param name="decodePixelWidth">照片的宽度</param>
        /// <param name="decodePixelHeight">照片的高度</param>
        public static async Task LoadImageFromStreamAsync(Image imageControl, StorageFile file, int decodePixelWidth, int decodePixelHeight) {
            System.Diagnostics.Debug.WriteLine("Invoking LoadImageFromStreamAsync...");
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };
                await bitmapImage.SetSourceAsync(fileStream);
                imageControl.Source = bitmapImage;
            }
        }

        /// <summary>
        /// 给 Configuration.Avatar 设置默认头像
        /// </summary>
        /// <param name="res">用于设置 Avatar 属性的 Configuration 实例</param>
        public static async Task GetDefaultAvatarAsync(Configuration res) {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assetsFolder = await installFolder.GetFolderAsync("Assets");
            res.Avatar = await assetsFolder.GetFileAsync("avatar_icon.png");
        }

        /// <summary>
        /// 提取传递进去的 Configuration 中的图像用于 Image 控件
        /// </summary>
        /// <param name="image">Image 控件</param>
        /// <param name="configuration">配置类</param>
        /// <param name="decodePixelWidth">照片宽度</param>
        /// <param name="decodePixelHeight">照片高度</param>
        /// <returns></returns>
        public static async Task GetAvatarAsync(Image image, Configuration configuration, int decodePixelWidth, int decodePixelHeight) {
            using (IRandomAccessStream fileStream = await configuration.Avatar.OpenAsync(FileAccessMode.Read)) {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };

                await bitmapImage.SetSourceAsync(fileStream);
                image.Source = bitmapImage;
            }
        }

        /// <summary>
        /// 将字节数组转换为 StorageFile
        /// </summary>
        /// <param name="byteArray">接收一个字节数组</param>
        /// <param name="fileName">要创建的 StorageFile 名称</param>
        /// <returns></returns>
        /// <remarks>https://social.msdn.microsoft.com/Forums/en-US/3c70c644-df5d-419f-9d19-55a9414c36dd/uwp-how-to-covert-back-byte-array-to-storage-file-c?forum=wpdevelop</remarks>
        public static async Task<StorageFile> AsStorageFile(this byte[] byteArray, string fileName) {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, byteArray);
            return file;
        }

        /// <summary>
        /// 将文件转换为字节数组
        /// </summary>
        /// <param name="file">接收一个文件</param>
        /// <returns></returns>
        /// <remarks>http://windowsapptutorials.com/tips/convert-storage-file-to-byte-array-in-universal-windows-apps/</remarks>
        public static async Task<byte[]> ToBytesAsync(this StorageFile file) {
            byte[] fileBytes = null;
            if (file == null) {
                throw new ArgumentNullException("file is null reference.");
            }
            else {
                using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync()) {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream)) {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                }
                return fileBytes;
            }
        }

        /// <summary>
        /// 从流中提取照片设置传递进去的 Image 控件
        /// </summary>
        /// <param name="imageControl">要设置照片的控件</param>
        /// <param name="file">接收一个文件</param>
        /// <param name="decodePixelWidth">照片的宽度</param>
        /// <param name="decodePixelHeight">照片的高度</param>
        /// <remarks>
        /// 如果字节解码失败，会抛出一个异常：System.Exception: 'The component cannot be found. (Exception from HRESULT: 0x88982F50)'
        /// 解码失败通常由两种原因：
        /// 1、字节码原本是由非图像数据转换而成；
        /// 2、字节码由一张遭遇损坏的图像文件转换而成；
        /// </remarks>
        public static async Task<BitmapImage> StorageFileToBitmapImageAsync(StorageFile file, int decodePixelWidth, int decodePixelHeight) {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }

        /// <summary>
        /// 表示应用的本地配置
        /// </summary>
        [Serializable]
        public class LocalSetting {
            private static LocalSetting _localSettingInstance;
            public static LocalSetting LocalSettingInstance {
                get {
                    if (_localSettingInstance == null) {
                        _localSettingInstance = new LocalSetting();
                    }
                    return _localSettingInstance;
                }
                private set {
                    _localSettingInstance = value;
                }
            }
            public static StorageFile AppSettingFile { get; set; }

            public static void SetNewInstance(LocalSetting localSetting) => LocalSettingInstance = localSetting;

            public static void InitialLocalSetting() {
                if (_localSettingInstance == null) {
                    _localSettingInstance = new LocalSetting();
                }
            }

            public static void SaveSettingFile() {
                byte[] bytes = LocalSettingInstance.Serializer();
                Windows.Foundation.IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                    async (source) => {
                        await FileIO.WriteBytesAsync(AppSettingFile, bytes);
                    }
                );
            }

            public DateMode DateMode { get; set; }
            public SaveMode SaveMode { get; set; }
            public Theme Theme { get; set; }

            private LocalSetting() {
                Windows.Foundation.IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                    async (source) => {
                        AppSettingFile = await ApplicationData.
                                               Current.
                                               LocalFolder.
                                               CreateFileAsync("VisualizationRecorderSetting", CreationCollisionOption.OpenIfExists);
                        IBuffer buffer = await FileIO.ReadBufferAsync(AppSettingFile);
                        if (buffer.Capacity == 0) {
                            SaveSettingFile();
                        }
                        else {
                            /*
                             * 最后一步很关键，从配置文件中提取二进制数据反序列化为LocalSetting，
                             * 然后覆盖掉原有的单例对象，其他文件从 Tool.LocalSetting.LocalSettingInstance
                             * 读取的对象是新覆盖的对象
                             */
                            SetNewInstance(buffer.ToArray().Deserializer<LocalSetting>());
                        }
                    }
            );

                this.DateMode = DateMode.DateWithWhiteSpace;
                this.SaveMode = SaveMode.OrginalFile;
                // 获取系统当前主题颜色
                // 不使用 Application.RequestedTheme 是因为
                // 在 App 构造函数初始化阶段执行 Application.RequestedTheme
                // 会导致 System.AccessViolationException 异常
                var systemTheme = new Windows.UI.ViewManagement.UISettings();
                var uiTheme = systemTheme.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).ToString();
                this.Theme = uiTheme == "#FF000000" ? Theme.Dark : Theme.Light;
            }
        }
    }

    /// <summary>
    /// 日期格式
    /// </summary>
    [Serializable]
    enum DateMode {
        DateWithWhiteSpace,
        DateWithSlash
    }

    /// <summary>
    /// 文件保存模式
    /// </summary>
    [Serializable]
    enum SaveMode {
        NewFile,
        OrginalFile
    }
    /// <summary>
    /// 应用主题颜色
    /// </summary>
    [Serializable]
    enum Theme {
        Light,
        Dark
    }
}
