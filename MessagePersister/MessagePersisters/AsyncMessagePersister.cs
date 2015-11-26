using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Message.Persister.Interface;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MessagePersisterComponent.MessagePersisters
{
    public class AsyncMessagePersister : IMessagerPersister
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private volatile ConcurrentQueue<IMessage> _messages = new ConcurrentQueue<IMessage>();
        private readonly IMessageWriter _messageWriter;
        private readonly List<Thread> _runTasks = new List<Thread>();
        private DateTime _curDate = DateTimeProvider.Now;
        private bool _exit;
        private bool _stop;
        private int _numberOfThreads;
        private int _numberOfMessages;

        public AsyncMessagePersister()
        {
            IUnityContainer container = new UnityContainer().LoadConfiguration("default"); 
            _messageWriter = container.Resolve<IMessageWriter>
                ("Writer", new ParameterOverrides
                {
                    { "messageFormater",container.Resolve<IMessageFormater>("Formater")}
                });


            _messageWriter.CreateTodaysRepository();
            AddTask();
        }

        public AsyncMessagePersister(IMessageWriter messageWriter)
        {
            _messageWriter = messageWriter;
            _messageWriter.CreateTodaysRepository();
            AddTask();
        }

        private void AddTask()
        {
            var runTask = new Thread(Processing);
            _runTasks.Add(runTask);
            runTask.Start();
        }

        private void DeleteTask()
        {
            var task = _runTasks.FirstOrDefault();
            if (task != null)
            {
                task.Abort();
                _runTasks.Remove(task);
            }
            Interlocked.Decrement(ref _numberOfThreads);
        }

        public void StopImmediately()
        {
            _exit = true;
            foreach (var task in _runTasks)
            {
                task.Abort();
                Interlocked.Decrement(ref _numberOfThreads);
            }
            Log.Error("******EXITING*****");
            Log.Error(string.Format("{0} messages aborted.", _numberOfMessages));
        }

        public void Stop()
        {
            _stop = true;
            Log.Warn("******STOPING******");
            Log.Warn(string.Format("{0} messages left to persist.", _numberOfMessages));
            var firstOrDefault = _runTasks.FirstOrDefault();
            while (firstOrDefault != null && firstOrDefault.IsAlive) Thread.Sleep(50);
        }

        public void Persist(IMessage message)
        {
            if (_stop || _exit) return;
            message.Timestamp = DateTimeProvider.Now;
            _messages.Enqueue(message);
           Interlocked.Increment(ref _numberOfMessages);
            Log.Info(String.Format("{1} Message {0} added to queue.", message.Name, _numberOfThreads));
        }

        public bool Stoped => (_exit && _messages.Count == 0) || (_numberOfThreads == 0);

        private void Processing()
        {
            Interlocked.Increment(ref _numberOfThreads);
            while (!_exit)
            {
                if ((DateTimeProvider.Now - _curDate).Days != 0)
                {
                    _curDate = DateTimeProvider.Now;
                    _messageWriter.CreateTodaysRepository();
                }

                if (_stop && _messages.Count == 0)
                    _exit = true;

                if (_messages.Count <= 0) continue;

                if (_messages.Count > 50*_numberOfThreads && _numberOfThreads < Environment.ProcessorCount)
                {
                    AddTask();
                }
                if (_messages.Count < 50*(_numberOfThreads-1) && _numberOfThreads > 0)
                {
                    DeleteTask();
                }
                IMessage message;
                bool processed = false;
                while (!_messages.TryDequeue(out message)) Thread.Sleep(10);
                try
                {
                    processed = _messageWriter.CreateNewMessageEntry(message);
                    if (processed) Interlocked.Decrement(ref _numberOfMessages);
                }
                catch (Exception)
                {
                    if (!processed) _messages.Enqueue(message);
                }
                
            }
        }
    }
}