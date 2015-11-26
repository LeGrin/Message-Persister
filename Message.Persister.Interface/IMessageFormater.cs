namespace Message.Persister.Interface
{
    public interface IMessageFormater
    {
        string FormatHeader();
        string FormatMessage(IMessage message);
        string FormatFooter();
    }
}