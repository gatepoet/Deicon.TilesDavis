using ReactiveUI;
using Splat;
using System;
using TilesDavis.Wpf.Views;

namespace TilesDavis.Wpf.ViewModels
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; private set; }

        private bool isFlyoutOpen;
        public bool IsFlyoutOpen
        {
            get { return isFlyoutOpen; }
            set { this.RaiseAndSetIfChanged(ref isFlyoutOpen, value); }
        }

        private ShortcutViewModel currentShortcut;
        public ShortcutViewModel CurrentShortcut
        {
            get { return currentShortcut; }
            set { this.RaiseAndSetIfChanged(ref currentShortcut, value); }
        }
        public IReactiveCommand<object> CloseFlyoutCommand { get; private set; } = ReactiveCommand.Create();

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            // Bind 
            RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
            LogHost.Default.Level = LogLevel.Debug;

            // Navigate to the opening page of the application
                      Router.Navigate.Execute(new DetailsViewModel(this));
            //Router.Navigate.Execute(new WelcomeViewModel(this));
            
            CloseFlyoutCommand.Subscribe(o => IsFlyoutOpen = false);
            MessageBus.Current.Listen<ShortcutViewModel>()
                .Subscribe(ShowFlyout);
        }
        private void ShowFlyout(ShortcutViewModel vm)
        {
            CurrentShortcut = vm;
            IsFlyoutOpen = true;
        }
        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new DetailsView(), typeof(IViewFor<DetailsViewModel>));
        }
    }
}
