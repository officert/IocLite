using System.Collections.Generic;

namespace IocLite.SampleApp.Data
{
    public class VideoGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> ConsolesIds { get; set; } 
        public string PublisherName { get; set; }
    }
}