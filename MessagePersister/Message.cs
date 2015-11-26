using System;
using Message.Persister.Interface;

namespace MessagePersisterComponent
{
    /// <summary>
    ///     This is the object that the diff. persisters(file persister,  database logger etc.) will operate on. The
    ///     FormatMessage() method will be called to get the message (formatted) to persist
    /// </summary>
    public class Message: IMessage
    {
        public Message(Guid guid, string name, string payload)
        {
            Guid = guid;
            Name = name;
            Payload = payload;
        }

        /// <summary>
        ///     Return a formatted message
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///     The Timestamp is initialized when the message is added.
        /// </summary>
        public virtual DateTime Timestamp { get; set; }

        public string Name { get; }
        public Guid Guid { get; }
        public string Payload { get; }
    }
}