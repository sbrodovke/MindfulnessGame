using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MindfulnessGame.Message;

namespace MindfulnessGame.ViewModel
{
    public class GameButtonViewModel : ViewModelBase
    {
        public readonly string ColorPropertyName = "Color";
        private Brush _color;

        public GameButtonViewModel()
        {
            Color = Brushes.White;
            Click = new RelayCommand(ExecuteClick);
            MessengerInstance.Register<NewColor>(this, ChangeColor);
        }

        public Brush Color
        {
            get => _color;
            set => Set(ColorPropertyName, ref _color, value);
        }

        public ICommand Click { get; set; }

        public void ChangeColor(NewColor newColor)
        {
            Color = newColor.Color;
        }

        private void ExecuteClick()
        {
            MessengerInstance.Send(new ButtonClicked(Color));
        }
    }
}