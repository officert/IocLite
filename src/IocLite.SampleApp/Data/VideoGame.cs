using System.Collections.Generic;

namespace IocLite.SampleApp.Data
{
    public class VideoGame
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<VideoGameConsole> ConsolesAvailableFor { get; set; } 
        public string PublisherName { get; set; }
    }

    public enum VideoGameConsole
    {
        Xbox360,
        Playstation3,
        Wii
    }
}