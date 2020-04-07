using System;
using System.Windows.Media;

namespace MindfulnessGame.Service.Interface
{
    public interface IGameService
    {
        event Action<Brush> OnChangeActivePanelColor;
        event Action<Brush> OnChangeNeedColor;
        event Action OnPlayerLoseRound;
        event Action OnPlayerWinRound;

        void StartGame();
        void EndGame();
        void ProcessAClick(Brush activePanelColor);
    }
}