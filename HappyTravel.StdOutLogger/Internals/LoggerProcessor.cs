using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HappyTravel.StdOutLogger.Internals
{
    internal class LoggerProcessor : IDisposable
    {
        public LoggerProcessor()
        {
            _outputTask = Task.Run(Start);
        }


        public void Dispose()
        {
            _messageQueue.CompleteAdding();
            try
            {
                _outputTask.Wait(TaskDisposeTimeout);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 &&
                                                ex.InnerExceptions[0] is TaskCanceledException)
            {
                // ignored
            }
        }


        private void Start()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                    WriteMessage(message);
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch
                {
                    // ignored
                }
            }
        }


        private void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }


        public void Log(string message)
        {
            if (_messageQueue.IsAddingCompleted)
                return;

            try
            {
                _messageQueue.Add(message);
            }
            catch (InvalidOperationException)
            {
                // ignored
            }
        }


        private readonly Task _outputTask;
        private const int MaxQueuedMessages = 1024;
        private const int TaskDisposeTimeout = 1000;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>(MaxQueuedMessages);

    }
}