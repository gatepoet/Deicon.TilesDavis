using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TilesDavis.Core;
using TilesDavis.Wpf.Util;
using System.Reactive.Linq;
using System.Reactive;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace TilesDavis.Wpf.ViewModels
{
    public class DetailsViewModel : ViewModel, IRoutableViewModel
    {
        private readonly Core.TilesDavis tilesDavis = new Core.TilesDavis();
        public string UrlPathSegment
        {
            get
            {
                return "details";
            }
        }

        public IScreen HostScreen
        {
            get;
            protected set;
        }

        public DetailsViewModel(IScreen screen)
        {
            HostScreen = screen;
            shortcuts = new ReactiveList<ShortcutViewModel>();
            ShortcutsView = CreateCollectionView(shortcuts);
            OpenShortcutLocationCommand = new ActionCommand<IList>(OpenShortcutLocation, l => true);
            AddToIgnoreCommand = new ActionCommand<IList>(AddToIgnoreList, l => true);
            ShowFlyoutCommand = new ActionCommand<IList>(ShowFlyout, l => true);
            LoadShortcuts();
            MessageBus.Current.Listen<UpdateShortcutsWithSameTargetCommand>()
                .Subscribe(UpdateShortcutsWithSameTarget);
        }

        private void UpdateShortcutsWithSameTarget(UpdateShortcutsWithSameTargetCommand command)
        {
            shortcuts.Where(s => s.Target == command.Shortcut.Target && s != command.Shortcut)
                .ToList()
                .ForEach(s => s.Reload());
        }

        private void OpenShortcutLocation(IList items)
        {
            var shortcut = items.Cast<ShortcutViewModel>().FirstOrDefault();
            Process.Start(Path.GetDirectoryName(shortcut.ShortcutPath));
        }

        private void ShowFlyout(IList itemsToShow)
        {
            var current = itemsToShow.Cast<ShortcutViewModel>().First();
            MessageBus.Current.SendMessage(current);
        }

        private MultiSelectCollectionView<ShortcutViewModel> CreateCollectionView(ReactiveList<ShortcutViewModel> shortcuts)
        {
            var view = new MultiSelectCollectionView<ShortcutViewModel>(shortcuts);
            view.Filter = ApplyFilter;
            view.SortDescriptions.Add(new SortDescription(nameof(ShortcutViewModel.Name), ListSortDirection.Ascending));
            return view;
        }

        private void LoadShortcuts()
        {
            Task.Factory.StartNew(() =>
            {
                tilesDavis.LoadShortcuts()
                    .ToObservable()
                    .ObserveOn(App.Current.Dispatcher)
                    .Select(CreateViewModel)
                    .Subscribe(shortcuts.Add);
            });
        }

        private void AddToIgnoreList(IList itemsToHide)
        {
            var items = itemsToHide.Cast<ShortcutViewModel>().ToList();
            shortcuts.RemoveAll(items);
            var entries = items.Select(shortcut => new IgnoreEntry(path: shortcut.ShortcutPath)).ToArray();
            tilesDavis.AddToIgnoreList(entries);
        }

        public ICommand OpenShortcutLocationCommand
        {
            get;
            private set;
        }

        public ICommand AddToIgnoreCommand
        {
            get;
            private set;
        }

        public ICommand ShowFlyoutCommand
        {
            get;
            private set;
        }

        private bool ApplyFilter(object item)
        {
            ShortcutViewModel shortcut = item as ShortcutViewModel;
            if (shortcut != null)
            {
                switch (Filter)
                {
                    case ShortcutFilter.HasTile:
                        return shortcut.HasTile;
                    case ShortcutFilter.MissingTile:
                        return !shortcut.HasTile;
                    case ShortcutFilter.All:
                    default:
                        return true;
                }
            }

            return false;
        }

        public ICollectionView ShortcutsView
        {
            get;
            private set;
        }

        private ShortcutViewModel CreateViewModel(Shortcut shortcut)
        {
            var vm = new ShortcutViewModel(shortcut);
            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ShortcutViewModel.HasTile))
                    ShortcutsView.Refresh();
            };

            return vm;
        }

        public enum ShortcutFilter
        {
            All,
            HasTile,
            MissingTile
        }

        private ShortcutFilter filter = ShortcutFilter.All;
        private ReactiveList<ShortcutViewModel> shortcuts = new ReactiveList<ShortcutViewModel>();
        public ShortcutFilter Filter
        {
            get
            {
                return filter;
            }

            set
            {
                filter = value;
                ShortcutsView.Refresh();
            }
        }
    }
}