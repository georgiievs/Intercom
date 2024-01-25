using System;
using System.Windows.Threading;

namespace Intercom.Application
{
    public delegate void TimerTickAction(int tick);
    public delegate void TimerOnceAction();


    /// <summary>
    /// Таймер домофона.
    /// 
    /// Выполняет действия:
    ///     OnRunCallback - при запуске,
    ///     OnTickCallback - с заданным пользователем интервалом,
    ///     OnCanclellCallback - при остановке (автоматически или вручную).
    /// </summary>
    public class Proccess
    {
        private readonly DispatcherTimer timer = new();

        private int tick;

        public bool IsRunning { get; private set; }

        public TimerOnceAction? OnRunCallback { get; set; }
        public TimerTickAction? OnTickCallback { get; set; }
        public TimerOnceAction? OnStopCallback { get; set; }

        public Proccess(int duration)
        {
            timer.Interval = new TimeSpan(0, 0, 1);

            timer.Tick += (sender, e) =>
            {
                if(tick < duration)
                {
                    OnTick();
                }
                else
                {
                    OnShotdown();
                }
            };
        }

        public void Run()
        {
            if(IsRunning)
            {
                return;
            }

            OnRun();
        }

        public void Shotdown()
        {
            if(!IsRunning)
            {
                return;
            }

            OnShotdown();
        }

        #region Handlers

        private void OnRun()
        {
            tick = 0;
            OnRunCallback?.Invoke();
            IsRunning = true;
            timer.Start();
        }

        private void OnTick()
        {
            OnTickCallback?.Invoke(tick);
            tick += 1;
        }

        private void OnShotdown()
        {
            timer.Stop();
            IsRunning = false;
            OnStopCallback?.Invoke();
        }

        #endregion
    }
}
