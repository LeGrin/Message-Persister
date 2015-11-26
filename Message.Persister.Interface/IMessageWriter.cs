namespace Message.Persister.Interface
{
    public interface IMessageWriter
    {
        void CreateTodaysRepository();
        bool CreateNewMessageEntry(IMessage message);
    }
}