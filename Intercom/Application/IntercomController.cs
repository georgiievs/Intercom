using Intercom.Constants;
using Intercom.Data;

namespace Intercom.Application
{
    public class IntercomController
    {
        public IntercomDbContext Database { get => database; }

        public bool IsRunning
        {
            get
            {
                return ErrorProccess.IsRunning || LockProccess.IsRunning || 
                    CallProccess.IsRunning || CommunicationProccess.IsRunning;
            }
        }

        public int? CurrentTarget { get; private set; } = null;
        
        public IntercomAccessService AccessService { get; private set; }

        public Status? CurrentStatus { get; private set; } = null;

        private readonly IntercomDbContext database;

        private readonly Proccess ErrorProccess;
        private readonly Proccess CallProccess;
        private readonly Proccess LockProccess;        
        private readonly Proccess CommunicationProccess;

        public IntercomController(IntercomDbContext database)
        {
            this.database = database;

            AccessService = new IntercomAccessService(this);

            ErrorProccess = new(Timeouts.Error);
            LockProccess = new(Timeouts.Unlocked);
            CallProccess = new(Timeouts.Connection);
            CommunicationProccess = new(Timeouts.Communication);
        }

        
        #region ErrorProccessConfiguration
      
        public void SetError(string status = "Ошибка")
        {
            if(IsRunning)
            {
                return;
            }

            CurrentStatus = new Status { Message = status };
            ErrorProccess.Run();
        }

        public void StopError() => ErrorProccess.Shotdown();

        public void AddErrorHandlers(TimerOnceAction? onStart, TimerTickAction? everySecond, TimerOnceAction? atTheEnd)
        {
            AddTimerHandlers(ErrorProccess, onStart, everySecond, atTheEnd);
        }

        #endregion

        public Result<int> SubmitCommunication(int key)
        {
            if (!IsRunning)
            {
                return Result<int>.Fail("Ошибка: нет входящего вызова");
            }

            if(CurrentTarget != key)
            {
                return Result<int>.Fail("Ошибка: неверный приемник");
            }

            StopCallToApartment();
            CurrentTarget = key;
            CommunicationProccess.Run();

            return Result<int>.Success(key);
        }

        public void StopCommunication()
        {
            CommunicationProccess.Shotdown();
            CurrentTarget = null;
        }

        #region CallProccessConfiguration

        public Result<int> CallToApartment(int key)
        {
            if(IsRunning)
            {
                return Result<int>.Fail("Ошибка: домофон занят");
            }

            if(!AccessService.IsApartmentExist(key))
            {
                return Result<int>.Fail("Ошибка: квартира не существует");
            }

            if(AccessService.IsApartmentBlocked(key))
            {
                return Result<int>.Fail("Ошибка: заблокировано");
            }

            CurrentTarget = key;
            CallProccess.Run();

            return Result<int>.Success(key);
        }

        public void StopCallToApartment()
        {
            CallProccess.Shotdown();
            CurrentTarget = null;
        }

        public void AddCallHandlers(TimerOnceAction? onStart, TimerTickAction? everySecond, TimerOnceAction? atTheEnd)
        {
            AddTimerHandlers(CallProccess, onStart, everySecond, atTheEnd);
        }

        #endregion


        #region LockProccessConfiguration

        public void CloseDoorLock() => LockProccess.Shotdown();

        public void UnlockDoor()
        {
            if(IsRunning)
            {
                return;
            }

            LockProccess.Run();
        }

        public void AddLockHandlers(TimerOnceAction? onStart, TimerTickAction? everySecond, TimerOnceAction? atTheEnd)
        {
            AddTimerHandlers(LockProccess, onStart, everySecond, atTheEnd);
        }

        #endregion


        public void AddCommunicationHandlers(TimerOnceAction? onStart, TimerTickAction? everySecond, TimerOnceAction? atTheEnd)
        {
            AddTimerHandlers(CommunicationProccess, onStart, everySecond, atTheEnd);
        }

        public void StopAll()
        {
            StopError();
            StopCallToApartment();
            StopCommunication();
            CloseDoorLock();
        }

        private static void AddTimerHandlers(Proccess timer, TimerOnceAction? doOnStart, TimerTickAction? doEverySecond, TimerOnceAction? doAtTheEnd)
        {
            if(doOnStart != null)
                timer.OnRunCallback += doOnStart;

            if(doEverySecond != null)
                timer.OnTickCallback += doEverySecond;

            if(doAtTheEnd != null)
                timer.OnStopCallback += doAtTheEnd;
        }
    }
}
