using System;
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
    public class PersisterTest
    {
        private readonly Mock<IMessageFormater> _formaterMock;
        private readonly Mock<IMessageWriter> _moqfileworker;

        public PersisterTest()
        {
            _formaterMock = new Mock<IMessageFormater>();
            _moqfileworker = new Mock<IMessageWriter>();
        }

        private IMessagerPersister PreparePersister()
        {
            return new AsyncMessagePersister(_moqfileworker.Object);
        }

        [TestMethod]
        public void U_Formater_FormatMessage_Trigered()
        {
            var persister = new AsyncMessagePersister(new FileMessageWriter(_formaterMock.Object));
            var message = new MessagePersisterComponent.Message(Guid.NewGuid(), "test", "test");
            persister.Persist(message);
            persister.Stop();
            _formaterMock.Verify(
                t => t.FormatMessage(It.IsAny<MessagePersisterComponent.Message>()));
        }

        [TestMethod]
        public void U_IMessageWriter_CreateNewMessageEntry_Trigered()
        {
            var persister = PreparePersister();
            var message = new MessagePersisterComponent.Message(Guid.NewGuid(), "test", "test");
            persister.Persist(message);
            _moqfileworker.Verify(
                t => t.CreateNewMessageEntry(It.Is<MessagePersisterComponent.Message>(o => o.Name == "test")));
        }

        [TestMethod]
        public void U_IMessageWriter_CreateTodaysRepository_Trigered()
        {
            var persister = PreparePersister();
            var message = new MessagePersisterComponent.Message(Guid.NewGuid(), "test", "test");
            persister.Persist(message);
            _moqfileworker.Verify(t => t.CreateTodaysRepository());
        }
    }
}