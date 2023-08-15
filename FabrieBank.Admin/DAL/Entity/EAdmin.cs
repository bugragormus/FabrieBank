using FabrieBank.DAL.Entity;
using Npgsql;
using System.Data;
using System.Reflection;
using FabrieBank.Admin.DAL.Common.DTO;
using FabrieBank.DAL;
using NpgsqlTypes;

namespace FabrieBank.Admin.DAL.Entity
{
	public class EAdmin
	{
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public EAdmin()
		{
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
		}

        public DTOAdmin ReadAdmin(DTOAdmin admin)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadAdmin";

                    string vsql = $"SELECT * FROM {functionName}(@p_nickname)";

                    using (NpgsqlCommand command = new NpgsqlCommand(vsql, connection))
                    {
                        command.Parameters.AddWithValue("@p_nickname", NpgsqlDbType.Varchar, (object)admin.Nickname ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DTOAdmin dTOAdmin = new DTOAdmin
                            {
                                Id = (int)dataTable.Rows[0]["id"],
                                Nickname = dataTable.Rows[0]["nickname"].ToString(),
                                Password = dataTable.Rows[0]["password"].ToString(),
                                AccessLevel = (int)dataTable.Rows[0]["accesslevel"],
                            };

                            return dTOAdmin;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return admin;
        }
    }
}

