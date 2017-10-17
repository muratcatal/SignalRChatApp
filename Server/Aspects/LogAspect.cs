using System;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Server.Aspects
{
    [Serializable]
    public class LogAspect : OnMethodBoundaryAspect
    {
        private readonly string _message;

        public LogAspect(string message)
        {
            this._message = message;
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine($"[{DateTime.Now}] {_message} : {string.Join(",", args.Arguments)}");
        }
    }
}
