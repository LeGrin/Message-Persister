using System;

namespace Message.Persister.Interface
{
    public interface IMessage
    {

        DateTime Timestamp { get; set; }

        string Name { get; }
        Guid Guid { get; }
        string Payload { get; }
    }
}
