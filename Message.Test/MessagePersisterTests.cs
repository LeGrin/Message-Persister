using System;
using System.IO;
using Message.Persister.Interface;
using MessagePersisterComponent;
using MessagePersisterComponent.MessagePersisters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Message.Test
{
    [TestClass]
    public class MessagePersisterTests
    {
        private static IMessagerPersister PreparePersister()
        {
            return new AsyncMessagePersister();
        }

        private static void GenerateMessages(int number, IMessagerPersister persister)
        {
            for (var i = 0; i < number; i++)
            {
                persister.Persist(CreateMessage());
            }
        }

        private static MessagePersisterComponent.Message CreateMessage()
        {
            var guid = Guid.NewGuid();
            return new MessagePersisterComponent.Message(guid, String.Format("Message {0}", guid), "test");
        }

        [TestMethod]
        public void I_Persister_Stop_Messages_In_Queue_Processed()
        {
            DateTimeProvider.Now = DateTime.Parse("1 Jan 2000");
            var persister = PreparePersister();
            GenerateMessages(10, persister);
            persister.Stop();
            Assert.IsTrue(
                Directory.GetFiles(@"C:\Messages\Persisted 20000101\", "*", SearchOption.AllDirectories).Length == 10);
            Directory.Delete(@"C:\Messages\Persisted 20000101\", true);
        }

        [TestMethod]
        public void I_Persister_StopImmediately_Message_In_Queue_Aborted()
        {
            DateTimeProvider.Now = DateTime.Parse("1 Jan 2000");
            var persister = PreparePersister();
            GenerateMessages(10, persister);
            persister.StopImmediately();
            Assert.IsTrue(Directory.GetFiles(@"C:\Messages\Persisted 20000101\", "*",
                SearchOption.AllDirectories).Length < 10);
            Directory.Delete(@"C:\Messages\Persisted 20000101\", true);
        }

        [TestMethod]
        public void I_Persister_Stop_New_Messages_Not_Queued()
        {
            DateTimeProvider.Now = DateTime.Parse("1 Jan 2000");
            var persister = PreparePersister();
            GenerateMessages(10, persister);
            persister.Stop();
            persister.Persist(CreateMessage());
            Assert.IsTrue(Directory.GetFiles(@"C:\Messages\Persisted 20000101\", "*",
                SearchOption.AllDirectories).Length == 10);
            Directory.Delete(@"C:\Messages\Persisted 20000101\", true);
        }

        [TestMethod]
        public void I_Persister_New_Folder_Created_Over_Night()
        {
            DateTimeProvider.Now = DateTime.Parse("1 Jan 2000");
            var persister = PreparePersister();
            DateTimeProvider.Now = DateTime.Parse("2 Jan 2000");
            persister.Stop();
            Assert.IsTrue(Directory.Exists(@"C:\Messages\Persisted 20000102\"));
            Directory.Delete(@"C:\Messages\Persisted 20000101\", true);
            Directory.Delete(@"C:\Messages\Persisted 20000102\", true);
        }
    }
}