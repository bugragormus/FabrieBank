using System.Reflection;

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

        public void LogAndHandleError(Exception ex)
        {
            MethodBase method = MethodBase.GetCurrentMethod();
            dataAccessLayer.LogError(ex, method.ToString());

            Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
        }
    }
}