using System;

namespace Message.Persister.Interface
{
    public interface ILogger
    {
        void Write(string message, Enum color);
    }
}