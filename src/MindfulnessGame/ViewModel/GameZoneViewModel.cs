using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using MindfulnessGame.Message;
using MindfulnessGame.Repository.Interface;

namespace MindfulnessGame.ViewModel
{
    public class GameZoneViewModel : ViewModelBase
    {
        private readonly IColorRepository _colorRepository;
        private readonly IScoreRepository _scoreRepository;

        public readonly string CurrentScorePropertyName = "CurrentScore";
        public readonly string GameStatusPropertyName = "GameStatus";
        public readonly string NeedColorPropertyName = "NeedColor";
        private string _currentScore;

        private bool _gameRunning;
        private string _gameStatus;
        private Brush _needColor;
        private int _score;
        private CancellationTokenSource _startGameToken;

        public GameZoneViewModel(IColorRepository colorRepository, IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
            _colorRepository = colorRepository;
            _startGameToken = new CancellationTokenSource();

            Score = 0;
            NeedColor = Brushes.White;
            GameStatus = "Начни игру!";
            MessengerInstance.Register<GameStarted>(this, StartGame);
            MessengerInstance.Register<GamePaused>(this, PauseGame);
            MessengerInstance.Register<GameEnded>(this, EndGame);
            MessengerInstance.Register<ButtonClicked>(this, ClickButton);
        }

        public Brush NeedColor
        {
            get => _needColor;
            set => Set(NeedColorPropertyName, ref _needColor, value);
        }

        public string CurrentScore
        {
            get => _currentScore;
            set => Set(CurrentScorePropertyName, ref _currentScore, value);
        }

        public string GameStatus
        {
            get => _gameStatus;
            set => Set(GameStatusPropertyName, ref _gameStatus, value);
        }

        private int Score
        {
            get => _score;
            set
            {
                _score = value;
                CurrentScore = $"Текущий счёт: {_score}";
            }
        }

        private void EndGame(GameEnded _)
        {
            EndGameAndWriteScore();
            GameStatus = "Начни игру!";
        }

        private void StopGame()
        {
            _startGameToken.Cancel();
            NeedColor = Brushes.White;
            MessengerInstance.Send(new NewColor(Brushes.White));
        }

        private void EndGameAndWriteScore()
        {
            StopGame();
            _scoreRepository.WriteResult(Score);
            Score = 0;
        }

        private void PauseGame(GamePaused _)
        {
            StopGame();
            GameStatus = "Пауза";
        }

        private async void ClickButton(ButtonClicked buttonClicked)
        {
            if (!_gameRunning)
                return;

            if (buttonClicked.Color == NeedColor)
            {
                Score++;
                _startGameToken.Cancel();
                StopGame();
                await StartGame();
            }
            else
            {
                GameStatus = "О нет, ты проиграл!";
                EndGameAndWriteScore();
                MessengerInstance.Send(new PlayerLose());
            }
        }

        private async void StartGame(GameStarted _)
        {
            await StartGame();
        }

        private async Task StartGame()
        {
            _startGameToken = new CancellationTokenSource();
            await StartGame(_startGameToken.Token);
        }

        private async Task StartGame(CancellationToken cancellationToken)
        {
            for (var i = 3; i > 0; i--)
            {
                GameStatus = $"Раунд начнётся через: {i}";
                try
                {
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(true);
                }
                catch
                {
                    _gameRunning = false;
                    return;
                }
            }

            _gameRunning = true;
            GameStatus = "Раунд начался!";
            NeedColor = _colorRepository.GetRandomColor();
            await GameCycle(cancellationToken).ConfigureAwait(false);
        }


        public async Task GameCycle(CancellationToken cancellationToken)
        {
            do
            {
                var color = _colorRepository.GetRandomColor();
                MessengerInstance.Send(new NewColor(color));
                try
                {
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    //Ignor
                }
            } while (!cancellationToken.IsCancellationRequested);

            _gameRunning = false;
        }
    }
}