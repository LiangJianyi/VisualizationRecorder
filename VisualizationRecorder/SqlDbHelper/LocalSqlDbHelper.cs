using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationRecorder.SqlDbHelper {

    class LocalSqlDbHelper : SqlDbHelper {
        private static readonly string _dataSource = "(localdb)\\MSSQLLocalDB";
        private static readonly string _initialCatalog = "JianyiLocalDataBase";


        public LocalSqlDbHelper() : base(new SqlConnectionStringBuilder {
            DataSource = _dataSource,
            InitialCatalog = _initialCatalog,
            IntegratedSecurity = true,
            Encrypt = false,
            ConnectTimeout = 30,
            ApplicationIntent = ApplicationIntent.ReadWrite,
            TrustServerCertificate = false,
            MultiSubnetFailover = false
        }) { }
    }
}
