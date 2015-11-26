using Message.Persister.Interface;
using MessagePersisterComponent.MessagePersisters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Message.Unit.Test
{
    /// <summary>
    ///     Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MessageWriterTests
    {
        private readonly Mock<IMessageWriter> _moqfileworker;

        public MessageWriterTests()
        {
            _moqfileworker = new Mock<IMessageWriter>();
        }

        private IMessagerPersister PreparePersister()
        {
            return new AsyncMessagePersister(_moqfileworker.Object);
        }
    }
}