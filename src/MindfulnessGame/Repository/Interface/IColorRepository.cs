using System.Windows.Media;

namespace MindfulnessGame.Repository.Interface
{
    public interface IColorRepository
    {
        Brush GetRandomColor();
    }
}