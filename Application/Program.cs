using System;
using System.Threading;
using Message.Persister.Interface;
using Message.Persister.Component;
using MessagePersisterComponent.MessagePersisters;
using MoqObjects;

namespace MessageProcessorApplication
{
    internal class Program
    {
        private static void TestPersister(IMessagerPersister persister)
        {
            var moqMessages = new MessageGenerator(persister, 5);
            Thread.Sleep(2000);
            persister.Stop();
            while (!persister.Stoped) Thread.Sleep(100);
            moqMessages.Dispose();
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        private static void Main(string[] args)
        {
            //case 1
            //IMessagerPersister persister = new AsyncMessagePersister();
            //var moqMessage = new MessageGenerator(persister, 1);
            //Thread.Sleep(2000);
            //persister.Stop();
            //while (!persister.Stoped) Thread.Sleep(100);
            //moqMessage.Dispose();
            //Console.WriteLine("Press enter to proceed to case 2");
            //Console.ReadLine();

            //case 2
            IMessagerPersister anotherPersister = new AsyncMessagePersister();
            TestPersister(anotherPersister);
            Console.WriteLine("Press enter to Exit");
            Console.ReadLine();
        }
    }
}