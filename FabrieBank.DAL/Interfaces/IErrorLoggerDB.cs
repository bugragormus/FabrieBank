namespace FabrieBank.DAL
{
    public interface IErrorLogger
    {
        void LogError(Exception ex, string methodName);
    }
}