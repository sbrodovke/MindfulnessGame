using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using MindfulnessGame.Repository;
using MindfulnessGame.Repository.Interface;
using MindfulnessGame.Service;
using MindfulnessGame.Service.Interface;

namespace MindfulnessGame.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            DispatcherHelper.Initialize();
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IScoreRepository, ScoreRepository>();
            SimpleIoc.Default.Register<IColorRepository, ColorRepository>();
            SimpleIoc.Default.Register<IGameService, GameService>();
            SimpleIoc.Default.Register<GameZoneViewModel>();
            SimpleIoc.Default.Register<GameButtonViewModel>();
            SimpleIoc.Default.Register<MenuViewModel>();
        }

        public GameZoneViewModel GameZoneViewModel => ServiceLocator.Current.GetInstance<GameZoneViewModel>();

        public GameButtonViewModel GameButtonViewModel => ServiceLocator.Current.GetInstance<GameButtonViewModel>();

        public MenuViewModel MenuViewModel => ServiceLocator.Current.GetInstance<MenuViewModel>();

        public static void Cleanup()
        {
            //TODO
        }
    }
}