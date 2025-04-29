using Dynamo.UI;
using Dynamo.ViewModels;

namespace Dynamo.Controls
{
    /// <summary>
    /// Interaction logic for InPortView.xaml
    /// </summary>
    public partial class InPortView : IViewModelView<InPortViewModel>
    {
        public InPortView()
        {
            InitializeComponent();
        }

        public InPortViewModel ViewModel => this.DataContext as InPortViewModel;
    }
}
