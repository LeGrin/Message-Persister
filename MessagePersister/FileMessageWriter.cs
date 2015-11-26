using System;
using System.IO;
using System.Threading;
using Message.Persister.Interface;


namespace MessagePersisterComponent
{
    public class FileMessageWriter : IMessageWriter
    {
        private readonly IMessageFormater _formater;
        private readonly string _messagesExtension = ConfigurationHelper.MessageExtension;
        private readonly string _messagesPersistedFolder = ConfigurationHelper.MessageFolder;
        private readonly string _rootFolder = ConfigurationHelper.RootFolder;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StreamWriter _writer;

        public FileMessageWriter(IMessageFormater messageFormater)
        {
            _formater = messageFormater;
        }

        public bool CreateNewMessageEntry(IMessage message)
        {
            try
            {
                using (_writer = CreateFileWriter(message))
                {
                    Thread.Sleep(200);
                    _writer.Write(_formater.FormatHeader());
                    _writer.Write(_formater.FormatMessage(message));
                    _writer.Write(_formater.FormatFooter());
                    _writer.AutoFlush = true;
                    log.Info(String.Format("{0} PERSISTED!", message.Name));
                }
                return true;
            }
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception e)
            {
                log.Error(string.Format("{0} have failed with exception: {1}", message.Name, e.Message));
                return false;
            }
        }

        public void CreateTodaysRepository()
        {
            try
            {
                if (!Directory.Exists(_rootFolder))
                    Directory.CreateDirectory(_rootFolder);

                if (!Directory.Exists(_messagesPersistedFolder + DateTimeProvider.Now.ToString("yyyyMMdd")))
                    Directory.CreateDirectory(_messagesPersistedFolder + DateTimeProvider.Now.ToString("yyyyMMdd"));
                log.Info(string.Format("Folder avaliable"));
            }
            catch (Exception e)
            {
                log.Error(string.Format("Folder creation failed with exception: {0}", e.Message));
            }
        }

        private StreamWriter CreateFileWriter(IMessage message)
        {
            return File.AppendText(_messagesPersistedFolder + DateTimeProvider.Now.ToString("yyyyMMdd") +
                                   @"\" + message.Name + DateTimeProvider.Now.ToString(" yyyyMMdd HHmmss fff") +
                                   _messagesExtension);
        }
    }
}