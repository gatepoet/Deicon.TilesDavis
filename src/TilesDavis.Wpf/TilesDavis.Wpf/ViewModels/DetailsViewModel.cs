using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using TilesDavis.Core;
using TilesDavis.Wpf.Util;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace TilesDavis.Wpf.ViewModels
{
    public class DetailsViewModel : ViewModel, IRoutableViewModel
    {
        public string UrlPathSegment
        {
            get { return "details"; }
        }

        public IScreen HostScreen { get; protected set; }
        public DetailsViewModel(IScreen screen)
        {
            HostScreen = screen;
            ignoreList = new ObservableCollection<ShortcutViewModel>();
            shortcuts = new ReactiveList<ShortcutViewModel>();
            ShortcutsView = CreateCollectionView(shortcuts);
            AddToIgnoreCommand = new ActionCommand<IList>(AddToIgnoreList, l => true);
            ShowFlyoutCommand = new ActionCommand<IList>(ShowFlyout, l => true);
            LoadShortcuts();
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

        private const string IgnoreFile = "ignore.txt";
        private void LoadShortcuts()
        {
            string[] shortcutsToIgnore = File.Exists(IgnoreFile)
                ? File.ReadAllLines(IgnoreFile)
                : new string[0];

            Core.TilesDavis.GetAllShortcuts(shortcutsToIgnore)
                .ToObservable()
                .ObserveOn(App.Current.Dispatcher)
                .Select(CreateViewModel)
                .Subscribe(shortcuts.Add);
        }

        private void AddToIgnoreList(IList itemsToHide)
        {
            var items = itemsToHide.Cast<ShortcutViewModel>().ToList();
            shortcuts.RemoveAll(items);

            File.AppendAllLines("ignore.txt", items.Select(s => s.ShortcutPath).ToArray());
        }

        public ICommand AddToIgnoreCommand { get; private set; }
        public ICommand ShowFlyoutCommand { get; private set; }



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
            vm.PropertyChanged += (s, e) =>
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