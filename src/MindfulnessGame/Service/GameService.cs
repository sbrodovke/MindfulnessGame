using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using MindfulnessGame.Repository.Interface;
using MindfulnessGame.Service.Interface;

namespace MindfulnessGame.Service
{
    public class GameService : IGameService
    {
        private readonly IColorRepository _colorRepository;

        private CancellationTokenSource _gameToken;

        private Brush _needColor;

        public GameService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
            _gameToken = new CancellationTokenSource();
        }

        public event Action<Brush> OnChangeActivePanelColor;
        public event Action<Brush> OnChangeNeedColor;
        public event Action OnPlayerLoseRound;
        public event Action OnPlayerWinRound;

        public void EndGame()
        {
            _gameToken.Cancel();
            _needColor = null;
            OnChangeNeedColor?.Invoke(null);
            OnChangeActivePanelColor?.Invoke(null);
        }

        public void ProcessAClick(Brush activePanelColor)
        {
            if (_needColor == null)
                return;

            if (_needColor == activePanelColor)
            {
                EndGame();
                OnPlayerWinRound?.Invoke();
            }
            else
            {
                EndGame();
                _needColor = null;
                OnPlayerLoseRound?.Invoke();
            }
        }

        public async void StartGame()
        {
            _needColor = _colorRepository.GetRandomColor();
            OnChangeNeedColor?.Invoke(_needColor);

            _gameToken = new CancellationTokenSource();
            await GameCycle(_gameToken.Token).ConfigureAwait(false);
        }

        private async Task GameCycle(CancellationToken cancellationToken)
        {
            do
            {
                var color = _colorRepository.GetRandomColor();
                OnChangeActivePanelColor?.Invoke(color);
                try
                {
                    await Task.Delay(700, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    //Ignor
                }
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}