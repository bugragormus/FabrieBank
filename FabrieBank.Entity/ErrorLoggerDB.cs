using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;
using System.Reflection;
using FabrieBank.DAL;

namespace FabrieBank.Entity
{
    public class ErrorLoggerDB : IErrorLogger
    {
        private DataAccessLayer dataAccessLayer;

        public ErrorLoggerDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void LogError(Exception ex, string methodName)
        {
            dataAccessLayer.LogError(ex, methodName);
        }
    }
}
