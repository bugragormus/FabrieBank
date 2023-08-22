using Npgsql;
using FabrieBank.DAL.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;

namespace FabrieBank.DAL
{
    public class DataAccessLayer
    {
        private NpgsqlConnectionStringBuilder database;

        public DataAccessLayer()
        {
            database = CallDB();
        }

        /// <summary>
        /// DB connection
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnectionStringBuilder CallDB()
        {
            try
            {
                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

                builder.Host = "localhost";
                builder.Username = "postgres";
                builder.Password = "43324332";
                builder.Database = "FabrieBank";

                return builder;
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return new NpgsqlConnectionStringBuilder();
            }
        }

        /// <summary>
        /// Password hashing algorithm
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeHash(string rawData)
        {
            string salt = "FabrieBankPasswordSafety";

            byte[] combinedBytes = Encoding.UTF8.GetBytes(rawData + salt);

            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] hashBytes = sha256Hash.ComputeHash(combinedBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}