using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace VisualizationRecorder.SqlDbHelper {
    using Debug = System.Diagnostics.Debug;

    class AzureSqlDbHelper : SqlDbHelper {
        private static readonly string _dataSource = "jianyi.database.chinacloudapi.cn";
        private static readonly string _userID = "jianyi";
        private static readonly string _password = "{FYbteTx4hNU@7Z83+u)2t@QrtQ9^E8EYFBWv67mGbeifsk[BUWxhBL6GzA]Z$r(";
        private static readonly string _initialCatalog = "JianyiAzureDataBase";

        public AzureSqlDbHelper() : base(new SqlConnectionStringBuilder {
            DataSource = _dataSource,
            UserID = _userID,
            Password = _password,
            InitialCatalog = _initialCatalog
        }) { }
    }
}
