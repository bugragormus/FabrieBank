using FabrieBank.Common;
using FabrieBank.DAL;

namespace FabrieBank.DAL.Entity
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
