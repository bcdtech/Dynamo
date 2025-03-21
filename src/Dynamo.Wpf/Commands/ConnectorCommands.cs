using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
namespace Dynamo.ViewModels
{
    public partial class ConnectorViewModel
    {
        private ICommand redrawCommand;

        public ICommand RedrawCommand
        {
            get
            {
                if (redrawCommand == null)
                    redrawCommand = new RelayCommand(Redraw, CanRedraw);

                return redrawCommand;
            }
        }
    }
}
