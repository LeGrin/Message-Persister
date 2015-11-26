using System;

namespace MessagePersisterComponent
{
    public static class DateTimeProvider
    {
        private static DateTime? _now;

        public static DateTime Now
        {
            get { return _now ?? DateTime.Now; }
            set { _now = value; }
        }
    }
}