using System;
using System.Text;
using Message.Persister.Interface;

namespace MessagePersisterComponent
{
    public class FormatFileTxt : IMessageFormater
    {
        public string FormatHeader()
        {
            return ("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" +
                    Environment.NewLine);
        }

        public string FormatMessage(IMessage message)
        {
            var sb = new StringBuilder();
            sb.Append(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            sb.Append("\t");
            sb.Append(message.Guid);
            sb.Append(". ");
            if (message.Name.Length > 0)
            {
                sb.Append(message.Name);
                sb.Append(". ");
            }
            if (message.Payload.Length > 0)
            {
                sb.Append(message.Payload);
                sb.Append(". ");
            }
            sb.Append("\t");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public string FormatFooter()
        {
            return string.Empty;
        }
    }
}