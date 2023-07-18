using System;

namespace FabrieBank.Common
{
    public interface IErrorLogger
    {
        void LogError(Exception ex, string methodName);
    }
}
