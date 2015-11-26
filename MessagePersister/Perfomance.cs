using System;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace MessagePersisterComponent
{
    class Stopwatch : HandlerAttribute
    {
        public override ICallHandler CreateHandler(Microsoft.Practices.Unity.IUnityContainer container)
        {
            return new StopwatchCallHandler();
        }
    }

    public class StopwatchCallHandler : ICallHandler
    {
        public int Order { get; set; }

        public StopwatchCallHandler() : this(0) { }

        public StopwatchCallHandler(int order)
        {
            Order = order;
        }

        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var result = getNext().Invoke(input, getNext);

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Time passed {0} ms", sw.ElapsedMilliseconds);

            return result;
        }

        #endregion
    }
}