using System.Windows.Media;

namespace MindfulnessGame.Message
{
    public class ButtonClicked
    {
        public ButtonClicked(Brush color)
        {
            Color = color;
        }

        public Brush Color { get; }
    }
}