using System.Windows;
using System.Windows.Controls;

namespace Dynamo.ViewModels
{
    public class PortDataTemplateSelector:DataTemplateSelector
    {
        public DataTemplate InputDataTemplate { get; set; } = null!;
        public DataTemplate IncomeDataTemplate { get; set; } = null!;
        public DataTemplate OutputDataTemplate { get; set; } = null!;
        public DataTemplate OutcomeDataTemplate { get; set; } = null!;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is  InPortViewModel inPortViewModel)
            {
                if(inPortViewModel.PortAlignment==Graph.Nodes.PortAlignment.Top)
                {
                    return IncomeDataTemplate;
                }
                else
                {
                    return InputDataTemplate;
                }
            }
            else if (item is OutPortViewModel outPortViewModel)
            {
                if(outPortViewModel.PortAlignment == Graph.Nodes.PortAlignment.Bottom)
                {
                    return OutcomeDataTemplate;
                }
                else
                {
                    return OutputDataTemplate;
                }
            }
            
            return base.SelectTemplate(item, container);
        }
    }
}
