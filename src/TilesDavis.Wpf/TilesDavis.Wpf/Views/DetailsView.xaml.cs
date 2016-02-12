using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TilesDavis.Wpf.ViewModels;

namespace TilesDavis.Wpf.Views
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : UserControl, IViewFor<DetailsViewModel>
    {
        public DetailsView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

            UserError.RegisterHandler(async error =>
            {
                RxApp.MainThreadScheduler.Schedule(error,
                    (scheduler, userError) =>
                    {
                        var result = MessageBox.Show(userError.ErrorMessage);
                        return Disposable.Empty;
                    });
                return await Task.Run(() => RecoveryOptionResult.CancelOperation);
            });
        }

        public DetailsViewModel ViewModel
        {
            get { return (DetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(DetailsViewModel), typeof(DetailsView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DetailsViewModel)value; }
        }
    }
}
