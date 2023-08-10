using System.Reflection;

namespace FabrieBank.DAL.Entity
{
    public class EErrorLogger : IErrorLogger
    {
        private DataAccessLayer dataAccessLayer;

        public EErrorLogger()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void LogError(Exception ex, string methodName)
        {
            dataAccessLayer.LogError(ex, methodName);
        }
    }
}