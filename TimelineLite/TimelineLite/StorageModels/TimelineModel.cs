using System.Runtime.InteropServices.ComTypes;

namespace TimelineLite.StorageModels
{
    public class TimelineModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}