using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SoMeSimulator.Data;
using SomeSimulator.Data.Models.Configurations;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Services.MessageManager
{
    public class TimedHostedService : IHostedService
    {
        private int TimerIntervalMilliseconds = DefaultValues.TimerIntervalMilliseconds;
        public IServiceProvider Services { get; set; }
        private readonly ISendMessage _sendMessage;
        private readonly IEntityFactory _factory;
        private readonly IStressLevelCalculator _stressLevelCalculator;
        private readonly IOptions<CommentsSettings> _commentsSettings;
        private static Timer _timer;
        private static readonly Object _lock = new Object();

        public TimedHostedService(IServiceProvider services, ISendMessage sendMessage, IEntityFactory factory, 
        IStressLevelCalculator stressLevelCalculator, IOptions<CommentsSettings> commentsSettings)
        {
            Services = services;
            _sendMessage = sendMessage;
            _factory = factory;
            _stressLevelCalculator = stressLevelCalculator;
            _commentsSettings = commentsSettings;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
                TimeSpan.FromMilliseconds(TimerIntervalMilliseconds));
            return Task.CompletedTask;
        }

        private void DoWorkAsync(Object state)
        {
            Log.Debug("DoWorkAsync");

            var hasLock = false;
            var activeSessions = false;
            
            try
            {
                Monitor.TryEnter(_lock, ref hasLock);

                if (!hasLock)
                {
                    Log.Debug("Did not get lock.");
                    return;
                }

                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                using (var scope = Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SoMeContext>();
                    var messageManager = new SessionManager(_sendMessage, dbContext, _factory, 
                    _stressLevelCalculator, _commentsSettings);
                    
                    messageManager.SendToRunningSessions();
                    
                    activeSessions = messageManager.HasActiveSessionGroups();
                }
            }
            finally
            {
                if (hasLock)
                {

                    int intervall = TimerIntervalMilliseconds;
                    
                    if (DefaultValues.TimerIntervalSlowDown)
                    {
                        var modifier = DateTime.Now.Hour > 22 && DateTime.Now.Hour < 6 ? 30 : 10;
                        intervall = activeSessions ? TimerIntervalMilliseconds : TimerIntervalMilliseconds * modifier;    
                    }
                    
                    

                    Monitor.Exit(_lock);
                    _timer.Change(
                        TimeSpan.FromMilliseconds(intervall),
                        TimeSpan.FromMilliseconds(intervall));
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}