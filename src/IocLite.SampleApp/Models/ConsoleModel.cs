using System.Collections.Generic;
using IocLite.SampleApp.Data;

namespace IocLite.SampleApp.Models
{
    public class ConsoleModel : LayoutModel
    {
        public Console Console { get; set; }
        public IEnumerable<VideoGame> Games { get; set; }
    }
}