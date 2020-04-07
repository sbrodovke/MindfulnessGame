using System.Windows.Media;

namespace MindfulnessGame.Message
{
    public class NewColor
    {
        public NewColor(Brush color)
        {
            Color = color;
        }

        public Brush Color { get; }
    }
}