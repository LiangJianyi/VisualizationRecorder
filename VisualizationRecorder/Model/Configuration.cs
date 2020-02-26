using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Storage;
using Windows.UI.Xaml;
using VisualizationRecorder.CommonTool;
using System.Threading.Tasks;

namespace VisualizationRecorder {

    /// <summary>
    /// 表示用户的配置
    /// </summary>
    class Configuration {
        public string UserName { get; set; }
        public string Password { get; set; }
        public ApplicationTheme Theme { get; set; }
        public string Title { get; set; }
        public StorageFile Avatar { get; set; }
        public StorageFile RecordFile { get; set; }

        public Configuration(string username, string password, string title = "", ApplicationTheme theme = ApplicationTheme.Light, StorageFile avatar = null, StorageFile record = null) {
            this.UserName = username;
            this.Password = password;
            this.Title = title;
            this.Theme = theme;
            if (avatar != null) {
                this.Avatar = avatar;
            }
            if (record != null) {
                this.RecordFile = record;
            }
        }

        /// <summary>
        /// 转换可序列化版本用于序列化
        /// </summary>
        /// <returns></returns>
        private async Task<SerializationConfiguration> AsSerializationConfigurationAsync() =>
            new SerializationConfiguration() {
                UserName = this.UserName,
                Password = this.Password,
                Title = this.Title,
                Theme = this.Theme,
                Avatar = this.Avatar == null ? null : await this.Avatar.ToBytesAsync(),
                RecordFile = this.RecordFile == null ? null : await this.RecordFile.ToBytesAsync()
            };

        /// <summary>
        /// 序列化为字节数组
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task<byte[]> SerializeToBytesAsync(Configuration configuration) {
            SerializationConfiguration serializationConfiguration = await configuration.AsSerializationConfigurationAsync();
            //内存实例
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, serializationConfiguration);//序列化对象，写入内存流中  
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// 将字节数组反序列化为 Configuration
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static async Task<Configuration> DeserializeObjectAsync(byte[] bytes) {
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            SerializationConfiguration obj = formatter.Deserialize(ms) as SerializationConfiguration;   //把内存流反序列成对象  
            ms.Close();
            return await obj.AsConfigurationAsync();
        }

        public override string ToString() {
            return $"Title: {this.Title}\n" +
                   $"Theme: {this.Theme}\n" +
                   $"Avatar file name: {this.Avatar?.Name}\n" +
                   $"Record file name: {this.RecordFile?.Name}";
        }
    }

    /// <summary>
    /// Configuration 的可序列化版本
    /// </summary>
    [Serializable]
    class SerializationConfiguration {
        public string UserName { get; set; }
        public string Password { get; set; }
        public ApplicationTheme Theme { get; set; }
        public string Title { get; set; }
        public byte[] Avatar { get; set; }
        public byte[] RecordFile { get; set; }

        /// <summary>
        /// 转换为 Configuration
        /// </summary>
        /// <returns></returns>
        public async Task<Configuration> AsConfigurationAsync() {
            return new Configuration(
                username: this.UserName,
                password: this.Password,
                title: this.Title,
                theme: this.Theme,
                avatar: this.Avatar == null ? null : await this.Avatar.AsStorageFile("Avatar.png"),
                record: this.RecordFile == null ? null : await this.RecordFile.AsStorageFile("RecordFile.txt")
            );
        }
    }
}
