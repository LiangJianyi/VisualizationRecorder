using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationRecorder.SqlDbHelper {
    using Debug = System.Diagnostics.Debug;

    enum SqlDbHelperType {
        AzureSqlDbHelper,
        LocalSqlDbHelper
    }

    class SqlDbHelper {
        protected SqlConnectionStringBuilder Builder { get; set; }

        protected SqlDbHelper(SqlConnectionStringBuilder builder) => this.Builder = builder;

        public static SqlDbHelper GetSqlDbHelper(SqlDbHelperType sqlDbHelperType) {
            switch (sqlDbHelperType) {
                case SqlDbHelperType.AzureSqlDbHelper:
                    return new AzureSqlDbHelper();
                case SqlDbHelperType.LocalSqlDbHelper:
                    return new LocalSqlDbHelper();
                default:
                    throw new Exception("SqlDbHelperType error.");
            }
        }

        /// <summary>
        /// 注册账户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileBytes">PersonData文件字节流</param>
        public async Task<int> RegisterUserAsync(Configuration configuration) {
            try {
                using (SqlConnection connection = new SqlConnection(Builder.ConnectionString)) {
                    string commandText = "INSERT INTO dbo.VisualizationRecorderUser (UserName,Password,PersonData) " +
                                         "VALUES(@username, @pwd, @personData)";
                    using (SqlCommand cmd = new SqlCommand(commandText, connection)) {
                        // Create sql parameter @username
                        IDataParameter parameter_username = cmd.CreateParameter();
                        parameter_username.ParameterName = "username";
                        parameter_username.DbType = DbType.String;
                        parameter_username.Value = configuration.UserName;

                        // Create sql parameter @pwd
                        IDataParameter parameter_password = cmd.CreateParameter();
                        parameter_password.ParameterName = "pwd";
                        parameter_password.DbType = DbType.String;
                        parameter_password.Value = configuration.Password;

                        // Create sql parameter @bytes
                        IDataParameter parameter_bytes = cmd.CreateParameter();
                        parameter_bytes.ParameterName = "personData";
                        parameter_bytes.DbType = DbType.Binary;
                        parameter_bytes.Value = await Configuration.SerializeToBytesAsync(configuration);

                        cmd.Parameters.Add(parameter_username);
                        cmd.Parameters.Add(parameter_password);
                        cmd.Parameters.Add(parameter_bytes);

                        await cmd.Connection.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException e) {
                Debug.WriteLine(e.ToString());
            }
            return -1;
        }

        /// <summary>
        /// 登录账户
        /// </summary>
        /// <param name="configuration">接收应用程序的配置</param>
        /// <returns>登陆成功返回 true，否则返回 false</returns>
        /// <remarks>
        /// 该方法会修改 Configuration.Avatar 和 Configuration.RecordFile
        /// </remarks>
        public async Task<Configuration> LoginAsync(string username, string password) {
            using (SqlConnection connect = new SqlConnection(Builder.ConnectionString)) {
                const string SQL = "SELECT TRIM(UserName), TRIM(Password), PersonData FROM dbo.VisualizationRecorderUser " +
                                   "WHERE UserName = @name AND Password = @pwd";
                SqlCommand command = new SqlCommand(SQL, connect);
                // Create sql parameter @name
                IDataParameter parameter_username = command.CreateParameter();
                parameter_username.ParameterName = "name";
                parameter_username.DbType = DbType.String;
                parameter_username.Value = username;

                // Create sql parameter @pwd
                IDataParameter parameter_password = command.CreateParameter();
                parameter_password.ParameterName = "pwd";
                parameter_password.DbType = DbType.String;
                parameter_password.Value = password;

                command.Parameters.Add(parameter_username);
                command.Parameters.Add(parameter_password);

                await connect.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess)) {
                    while (reader.Read()) {
                        /*
                         * 使用临时变量 binary 的原因是如果调用两次 GetSqlBinary(2) 会抛出异常，
                         * 下面的代码会抛出 System.InvalidOperationException: 'Invalid attempt to read from column ordinal '2'.  
                         *  With CommandBehavior.SequentialAccess, you may only read from column ordinal '3' or greater.'
                         * 
                         * if (reader.GetSqlBinary(2).IsNull == false) {
                         *      byte[] bytes = reader.GetSqlBinary(2).Value;
                         *      return await Configuration.DeserializeObjectAsync(bytes);
                         * }
                         */
                        System.Data.SqlTypes.SqlBinary binary = reader.GetSqlBinary(2);
                        if (binary.IsNull == false) {
                            byte[] bytes = binary.Value;
                            // 从数据库拉取已经序列化的 Configuration
                            return await Configuration.DeserializeObjectAsync(bytes);
                        }
                        else {
                            return new Configuration(
                                username: reader.GetSqlString(0).Value,
                                password: reader.GetSqlString(1).Value
                            );
                        }
                    }
                }
            }
            return null;
        }
    }
}
