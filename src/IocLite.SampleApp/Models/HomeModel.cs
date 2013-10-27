using System.Collections.Generic;
using IocLite.SampleApp.Data;

namespace IocLite.SampleApp.Models
{
    public class HomeModel
    {
        public IEnumerable<VideoGame> VideoGames { get; set; } 
    }
}