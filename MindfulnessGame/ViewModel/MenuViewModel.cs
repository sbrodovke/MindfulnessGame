using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MindfulnessGame.Message;
using MindfulnessGame.Repository.Interface;

namespace MindfulnessGame.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly IScoreRepository _scoreRepository;

        public readonly string RecordPropertyName = "Record";

        private bool _gameRunning;

        private string _record;

        public MenuViewModel(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
            ChangeRecord();
            _scoreRepository.RecordChanged += score =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { ChangeRecord(score); });
            };

            Start = new RelayCommand(ExecuteStart, CanExecuteStart);
            Pause = new RelayCommand(ExecutePause, CanExecutePause);
            End = new RelayCommand(ExecuteEnd);
            MessengerInstance.Register<PlayerLose>(this, DoAfterLosePlayer);
        }

        private bool GameRunning
        {
            get => _gameRunning;
            set
            {
                _gameRunning = value;
                RaiseCanExecuteCommand();
            }
        }

        public string Record
        {
            get => _record;
            set => Set(RecordPropertyName, ref _record, value);
        }

        public ICommand Start { get; set; }

        public ICommand Pause { get; set; }

        public ICommand End { get; set; }

        private void DoAfterLosePlayer(PlayerLose _)
        {
            GameRunning = false;
        }

        private void RaiseCanExecuteCommand()
        {
            ((RelayCommand) Start).RaiseCanExecuteChanged();
            ((RelayCommand) Pause).RaiseCanExecuteChanged();
        }

        private bool CanExecutePause()
        {
            return GameRunning;
        }

        private bool CanExecuteStart()
        {
            return !GameRunning;
        }

        private void ExecuteEnd()
        {
            MessengerInstance.Send(new GameEnded());
            GameRunning = false;
        }

        private void ExecutePause()
        {
            GameRunning = false;
            MessengerInstance.Send(new GamePaused());
        }

        private void ExecuteStart()
        {
            MessengerInstance.Send(new GameStarted());
            GameRunning = true;
        }

        private void ChangeRecord()
        {
            var score = _scoreRepository.GetRecord();
            ChangeRecord(score);
        }

        private void ChangeRecord(int score)
        {
            Record = $"Рекорд: {score}";
        }
    }
}