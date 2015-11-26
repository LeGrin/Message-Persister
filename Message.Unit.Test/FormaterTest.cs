using Message.Persister.Interface;
using MessagePersisterComponent;
using MessagePersisterComponent.MessagePersisters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Message.Unit.Test
{
    /// <summary>
    ///     Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FormaterTest
    {
        private readonly Mock<IMessageFormater> _formaterMock;
        private readonly IMessageWriter _moqfileworker;

        public FormaterTest()
        {
            _formaterMock = new Mock<IMessageFormater>();
            _moqfileworker = new FileMessageWriter(_formaterMock.Object);
            
        }

        private IMessagerPersister PreparePersister()
        {
            return new AsyncMessagePersister(_moqfileworker);
        }
    }
}