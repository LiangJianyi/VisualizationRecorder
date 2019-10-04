﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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
    }
}
