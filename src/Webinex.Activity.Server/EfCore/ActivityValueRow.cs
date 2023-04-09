namespace Webinex.Activity.Server.EfCore
{
    public class ActivityValueRow
    {
        public int Id { get; set; }
        
        public int ActivityId { get; set; }
        
        public virtual ActivityRow Activity { get; set; }
        
        public string Path { get; set; }
        
        public string SearchPath { get; set; }
        
        public ActivityValueKind Kind { get; set; }
        
        public string Value { get; set; }

        public ActivityValueScalar ToScalar()
        {
            return new ActivityValueScalar(Path, Kind, Value);
        }
    }
}