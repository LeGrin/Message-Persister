using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Message.Persister.Interface;
using Message.Persister.Component;

namespace MoqObjects
{
    public class MessageGenerator : IDisposable
    {

        private readonly Dictionary<int, Bus> _buses = new Dictionary<int, Bus>
        {
            {1, new Bus {BusName = "SOLACE.MESSAGEBUS.ONE", NumberOfMessages = 10, TimeIntervalMilliseconds = 100}},
            {2, new Bus {BusName = "SOLACE.MESSAGEBUS.ONE.SLOW", NumberOfMessages = 30, TimeIntervalMilliseconds = 500}},
            {3, new Bus {BusName = "SOLACE.MESSAGEBUS.ONE.HIGH", NumberOfMessages = 100, TimeIntervalMilliseconds = 10}},
            {4, new Bus {BusName = "SOLACE.MESSAGEBUS.ONE.MEDIUM", NumberOfMessages = 79, TimeIntervalMilliseconds = 30}},
            {5, new Bus {BusName = "SOLACE.MESSAGEBUS.ONE.FAST", NumberOfMessages = 150, TimeIntervalMilliseconds = 3}}
        };
        private readonly int _loadPower;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public MessageGenerator(IMessagerPersister persister, int loadPower)
        {
            _loadPower = loadPower;
            GenerateRandomMessagesAtRandomIntervalsFromBuses(persister);
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }

        private void GenerateRandomMessagesAtRandomIntervalsFromBuses(IMessagerPersister persister)
        {
            var i = 1;
            if (_loadPower > 1) i = 2;
            for (; i < _loadPower + 1; i++)
            {
                var source = SolaceBusMock(_buses[i]);
                _subscriptions.Add(source.Subscribe(x => ProcessMessages(x, persister)));
            }
        }

        //Solace bus mock - produce observable collection of random messages which comes at random intervals 

        private IObservable<IMessage> SolaceBusMock(Bus bus)
        {
            var rnd = new Random();
            var source = Observable.Generate(0, x => x < bus.NumberOfMessages, x => x + 1,
                _ => CreateMessage(bus.BusName, rnd),
                _ => TimeSpan.FromMilliseconds(rnd.Next(1, 10)*bus.TimeIntervalMilliseconds));
            return source;
        }

        private IMessage CreateMessage(string solaceBusName, Random rnd)
        {
            var guid = Guid.NewGuid();
            return new MessagePersisterComponent.Message(guid, String.Format("Message {0} from bus {1}", guid, solaceBusName),
                rnd.Next(0, 10).ToString());
        }

        private static void ProcessMessages(IMessage message, IMessagerPersister persister)
        {
            persister.Persist(message);
        }
    }
}