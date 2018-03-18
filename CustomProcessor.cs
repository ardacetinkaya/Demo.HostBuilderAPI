using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace SomeKindOfProcessor
{
    public class CustomProcessor : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly ICustomProcessorData _customProcessorData;
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public static IConfiguration Configuration { get; set; }
        private Task _backgroundTask;
        private int _repeatCount = 0;


        public CustomProcessor(ICustomProcessorData data, IConfiguration config)
        {
            this._customProcessorData = data;
            Configuration = config;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _repeatCount = Int32.Parse($"{Configuration["BatchCount"]}");
            Task t = Task.Factory.StartNew(() =>
            {
                return DoSomeBackgroundTask();
            });

            _cancellationToken = new CancellationTokenSource();
            _cancellationToken.CancelAfter(900000);

            _backgroundTask = t;
            Console.WriteLine($"Hello {_customProcessorData.Name}");
            Console.WriteLine("Initialiaztion is finished...Let's start to do some work.");
            if (_backgroundTask.IsCompleted)
            {
                return _backgroundTask;
            }
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

            if (_backgroundTask == null)
            {
                return;
            }
            Console.WriteLine("CustomProcessor is finished batch...");
            try
            {
                _cancellationToken.Cancel();
            }
            finally
            {
                await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            _cancellationToken.Cancel();
        }
        private async Task DoSomeBackgroundTask()
        {
            int i = 0;
            while (!_cancellationToken.IsCancellationRequested)
            {
                i++;
                if (i == _repeatCount)
                {
                    i=0;
                    _cancellationToken.Cancel();
                }
                Console.WriteLine("CustomProcessor is doing some background process.");
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            if (_cancellationToken.IsCancellationRequested)
            {
                _backgroundTask = null;
                await StopAsync(_cancellationToken.Token);
                Console.WriteLine("Do you want to restart?");
                var repeat = Console.ReadLine();
                if (repeat == "y")
                {
                    await StartAsync(_cancellationToken.Token);
                }
                else
                    Environment.Exit(-1);

            }

        }


    }
}