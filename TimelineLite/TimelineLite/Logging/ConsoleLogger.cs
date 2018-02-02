
using System;

namespace TimelineLite.Logging
{
    public class ConsoleLogger : ILog
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}