using System;
using System.Threading.Tasks;


namespace UniversalCSharp
{
    public class PluginTask
    {
        public Action<int, string> OnCallbackEvent;
        private string message;
        private int delay;

        private int counter = 0;
        private Task backgroundTask;


        public PluginTask(string msg, int d)
        {
            message = msg;
            delay = d;
        }

        public void StartThread()
        {
            backgroundTask = Task.Run(() =>
                {
                    for (;;)
                    {
                        counter++;
                        OnCallbackEvent?.Invoke(counter, $"{message}-{DateTime.Now:T}");
                        Task.Delay(delay).Wait();
                    }
                });
        }


        public string GetInfo()
        {
            return "UniversalCSharp::GetInfo()";
        }
    }
}
