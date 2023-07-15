using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity;

public class Database
{
    public SqlConnectionStringBuilder CallDB()
    {
        try
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = "localhost";
            builder.UserID = "sa";
            builder.Password = "bugragrms4332";
            builder.InitialCatalog = "Banka";
            builder.TrustServerCertificate = true;

            return builder;
        }
        catch (SqlException e)
        {
            return new SqlConnectionStringBuilder();
        }
    }
}