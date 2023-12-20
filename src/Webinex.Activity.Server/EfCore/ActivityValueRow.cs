namespace Webinex.Activity.Server.EfCore
{
    public class ActivityValueRow
    {
        public int Id { get; set; }
        
        public int ActivityId { get; set; }
        
        public virtual ActivityRow Activity { get; set; } = null!;
        
        public string Path { get; set; } = null!;
        
        public string SearchPath { get; set; } = null!;
        
        public ActivityValueKind Kind { get; set; }

        public string Value { get; set; } = null!;

        public ActivityValueScalar ToScalar()
        {
            return new ActivityValueScalar(Path, Kind, Value);
        }
    }
}