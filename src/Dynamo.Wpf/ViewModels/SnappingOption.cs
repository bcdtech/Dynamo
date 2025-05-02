namespace Dynamo.ViewModels
{
    public class SnappingOption
    {
        public bool EnableSnapping
        {
            get; set;
        }
        public double SnappingRadius
        {
            get; set;
        } = 50;
        public double BlockSnappingRadius
        {
            get; set;
        } = 30;
    }
}
