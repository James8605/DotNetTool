using FastMember;

using Oracle.ManagedDataAccess.Client;

using PetaPoco;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DotNetTool
{
    internal class OraCol
    {
        public string data_table_col_name;
        public Member memberInfo;
        public string db_col_name;
        public System.Type type;
    }

    internal class BulKType
    {
        public TypeAccessor accessor;
        public List<OraCol> cols;
    }

    public class DBHelper
    {
        private static readonly object _locker = new();
        private static string _connect_str = string.Empty;
        private static string _provider = string.Empty;
        private static readonly ConcurrentDictionary<System.Type, BulKType> _type_dict = new();
        private static readonly PetaPoco.Database _db;

        public static string connect_str
        {
            get
            {
                if (_connect_str == string.Empty)
                {
                    _connect_str = GetConnectStr($"{(ConfigHelper.GetBool("dev") == true ? "db:dev" : "db:pro")}");
                }
                return _connect_str;
            }
        }

        public static string provider
        {
            get
            {
                if (_provider == string.Empty)
                {
                    _provider = ConfigHelper.GetString($"{(ConfigHelper.GetBool("dev") == true ? "db:dev:provider" : "db:pro:provider")}");
                }
                return _provider;
            }
        }

        public static Database db
        {
            get
            {
                return new Database(connect_str, provider);

            }
        }

        public static Database GetNewDB()
        {
            var config = ConfigHelper.GetBool("dev") ? "db:dev" : "db:pro";
            return GetDB(config);
        }

        public static Database GetDB(string config)
        {
            return new Database(GetConnectStr(config), GetProvider(config));
        }

        public static string GetConnectStr(string config)
        {
            string uid = EncryptHelper.Decrypt(ConfigHelper.GetString($"{config}:uid"));
            string password = EncryptHelper.Decrypt(ConfigHelper.GetString($"{config}:password"));
            string connect = ConfigHelper.GetString($"{config}:connect_str");

            return $"User Id={uid};Password={password};{connect}";
        }

        public static string GetProvider(string config)
        {
            return ConfigHelper.GetString($"{config}:provider");
        }

        public static async Task<object> InsertWithCreateAsync(object poco, string db_name, string table,
            string primary_key, string create_sql)
        {
            try
            {
                return await db.InsertAsync($"{db_name}.{table}", primary_key, poco);
            }
            catch
            {

                if (!TableExists(db_name, table))
                {
                    db.Execute(create_sql.ToUpper());
                    return await db.InsertAsync($"{db_name}.{table}", primary_key, poco);
                }
                else
                {
                    throw;
                }
            }
        }

        public static bool TableExists(string db_name, string table)
        {
            var sql = $"SELECT COUNT(*) FROM ALL_TABLES WHERE " +
                                              $"OWNER = '{db_name}' " +
                                              $"AND TABLE_NAME = " +
                                              $"'{table}'";

            return 1 == db.First<int>(sql.ToUpper());
        }

        public static void CreateTableIfNotExists(string db_name, string table_name, string createSQL)
        {
            lock (_locker)
            {
                if (!TableExists(db_name, table_name))
                {
                    db.Execute(createSQL.ToUpper());
                }
            }

        }

        private static void SaveUsingOracleBulkCopy(string db_name, string table_name, DataTable dt, List<OraCol> cols)
        {
            using var connection = new OracleConnection(connect_str);
            connection.Open();
            using var bulk = new OracleBulkCopy(connection, OracleBulkCopyOptions.UseInternalTransaction);

            foreach (var col in cols)
            {
                bulk.ColumnMappings.Add(col.data_table_col_name, col.db_col_name);
            }

            bulk.DestinationSchemaName = db_name;
            bulk.DestinationTableName = table_name;
            bulk.BatchSize = 0;
            bulk.BulkCopyTimeout = 600;

            bulk.WriteToServer(dt);
        }

        public static void BulkCopy2TempTable<T>(List<T> source, string db_name, string table_name)
        {
            db.Execute($"truncate table {db_name}.{table_name}".ToUpper());

            if (source.Count == 0)
            {
                return;
            }

            AddNewEntry2DictIfNotExists(source);

            var bulk_type = _type_dict[typeof(T)];

            DataTable dt = MakeDataTable(source, bulk_type);

            SaveUsingOracleBulkCopy(db_name, table_name, dt, bulk_type.cols);
        }

        private static DataTable MakeDataTable<T>(List<T> source, BulKType bulk_type)
        {
            var dt = new DataTable();

            foreach (var col in bulk_type.cols)
            {
                dt.Columns.Add(col.data_table_col_name, col.type);
            }

            foreach (var poco in source)
            {
                DataRow row = dt.NewRow();

                foreach (var col in bulk_type.cols)
                {
                    var p = poco;
                    row[col.data_table_col_name] = bulk_type.accessor[p, col.data_table_col_name];
                }
                dt.Rows.Add(row);
            }

            return dt;
        }


        private static void AddNewEntry2DictIfNotExists<T>(List<T> source)
        {
            if (!_type_dict.ContainsKey(typeof(T)))
            {
                var model = source[0];
                TypeAccessor accessor = TypeAccessor.Create(model.GetType());
                MemberSet members = accessor.GetMembers();

                var col_ls = new List<OraCol>();

                foreach (var member in members)
                {
                    if (!member.IsDefined(typeof(IgnoreAttribute)))
                    {
                        string db_col_name;
                        if (member.IsDefined(typeof(ColumnAttribute)))
                        {
                            db_col_name = ((ColumnAttribute)member
                                .GetAttribute(typeof(ColumnAttribute), true)).Name.ToUpper();
                        }
                        else
                        {
                            db_col_name = member.Name.ToUpper();
                        }

                        col_ls.Add(new OraCol
                        {
                            memberInfo = member,
                            data_table_col_name = member.Name,
                            db_col_name = db_col_name,
                            type = member.Type
                        });
                    }
                }

                _type_dict.TryAdd(typeof(T), new BulKType { accessor = accessor, cols = col_ls });
            }
        }
    }

}
