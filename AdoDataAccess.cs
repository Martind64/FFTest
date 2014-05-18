using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Configuration;

namespace OpretBruger
{
    public static class AdoDataAccess
    {
        private static readonly string dataProvider =
            ConfigurationManager.AppSettings.Get("DataProvider");

        private static readonly DbProviderFactory factory =
            DbProviderFactories.GetFactory(dataProvider);

        private static readonly string connectionStringName =
            ConfigurationManager.AppSettings.Get("ConnectionStringName");

        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

        public static T Read<T>(string sql, Func<IDataReader, T> makeDataObject,
            Dictionary<string, object> parms = null)
        {
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                using (var command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.SetParameters(parms);

                    connection.Open();

                    T t = default(T);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        t = makeDataObject(reader);
                    }
                    return t;
                }
            }
        }

        public static List<T> ReadList<T>(string sql, Func<IDataReader, T> makeDataObject,
            Dictionary<string, object> parms = null)
        {
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                using (var command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.SetParameters(parms);

                    connection.Open();

                    List<T> list = new List<T>();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(makeDataObject(reader));
                    }

                    return list;
                }
            }
        }

        public static int Insert(string sql, Dictionary<string, object> parms = null)
        {
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                using (var command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql + ";SELECT @@IDENTITY";
                    command.SetParameters(parms);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static int Update(string sql, Dictionary<string, object> parms = null)
        {
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                using (var command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.SetParameters(parms);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(string sql, Dictionary<string, object> parms = null)
        {
            Update(sql, parms);
        }

        private static void SetParameters(this DbCommand command, Dictionary<string, object> parms)
        {
            if (parms != null && parms.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in parms)
                {
                    string name = pair.Key;
                    object value = pair.Value;

                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = name;
                    dbParameter.Value = value ?? DBNull.Value;

                    command.Parameters.Add(dbParameter);
                }
            }
        }
    }
}
