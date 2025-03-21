using CommunityToolkit.Mvvm.Input;
using Dynamo.Graph.Nodes;
using System.Windows.Input;

namespace DynamoCoreWpf.Extensions;

static class PortModelExtensions
{
    public static bool CanAutoCompleteInput(this PortModel port)
    {
        return !(port.PortType == PortType.Input && port.Connectors?.FirstOrDefault()?.Start?.Owner != null);
    }
}
static class CommandExtensions
{
    public static void RaiseCanExecuteChanged(this ICommand command)
    {
        (command as IRelayCommand)?.NotifyCanExecuteChanged();
    }
}
