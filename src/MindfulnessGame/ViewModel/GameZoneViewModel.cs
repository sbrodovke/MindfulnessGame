using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using MindfulnessGame.Message;
using MindfulnessGame.Repository.Interface;
using MindfulnessGame.Service.Interface;

namespace MindfulnessGame.ViewModel
{
    public class GameZoneViewModel : ViewModelBase
    {
        private readonly IGameService _gameService;
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

        public GameZoneViewModel(IScoreRepository scoreRepository, IGameService gameService)
        {
            _scoreRepository = scoreRepository;
            _gameService = gameService;
            _startGameToken = new CancellationTokenSource();

            PreparingGame();

            MessengerInstance.Register<GameStarted>(this, StartGame);
            MessengerInstance.Register<GamePaused>(this, PauseGame);
            MessengerInstance.Register<GameEnded>(this, EndGame);
            MessengerInstance.Register<ButtonClicked>(this, ClickButton);

            _gameService.OnPlayerLoseRound += PlayerLoseHandler;
            _gameService.OnPlayerWinRound += PlayerWinHandler;
            _gameService.OnChangeNeedColor += ChangeNeedColorHandler;
            _gameService.OnChangeActivePanelColor += ChangeActivePanelColorHandler;
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

        private void ChangeActivePanelColorHandler(Brush color)
        {
            MessengerInstance.Send(new NewColor(color ?? Brushes.White));
        }

        private void ChangeNeedColorHandler(Brush color)
        {
            NeedColor = color ?? Brushes.White;
        }

        private async void PlayerWinHandler()
        {
            Score++;
            await StartGame();
        }

        private void PreparingGame()
        {
            Score = 0;
            GameStatus = "Начни игру!";
        }

        private void PlayerLoseHandler()
        {
            GameStatus = "Ты проиграл! Начни игру!";
            MessengerInstance.Send(new PlayerLose());
            _scoreRepository.WriteResult(Score);
            Score = 0;
        }

        private void EndGame(GameEnded _)
        {
            _startGameToken?.Cancel();
            _gameService.EndGame();
            _scoreRepository.WriteResult(Score);
            PreparingGame();
        }

        private void PauseGame(GamePaused _)
        {
            _startGameToken?.Cancel();
            _gameService.EndGame();
            GameStatus = "Пауза";
        }

        private void ClickButton(ButtonClicked buttonClicked)
        {
            if (!_gameRunning)
                return;

            _gameService.ProcessAClick(buttonClicked.Color);
        }

        private async void StartGame(GameStarted _)
        {
            await StartGame();
        }

        private async Task StartGame()
        {
            _startGameToken?.Cancel();
            _startGameToken = new CancellationTokenSource();
            await StartGame(_startGameToken.Token);
        }

        private async Task StartGame(CancellationToken cancellationToken)
        {
            for (var i = 20; i > 0; i--)
            {
                GameStatus = $"Раунд начнётся через: {(double) i / 10:F1}";
                try
                {
                    await Task.Delay(100, cancellationToken).ConfigureAwait(true);
                }
                catch
                {
                    _gameRunning = false;
                    return;
                }
            }

            _gameRunning = true;
            GameStatus = "Раунд начался!";
            _gameService.StartGame();
        }
    }
}