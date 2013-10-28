using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using IocLite.SampleApp.Data;
using IocLite.SampleApp.Models;
using Console = IocLite.SampleApp.Data.Console;

namespace IocLite.SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVideoGameRepository _videoGameRepository;
        private readonly IConsoleRepository _consoleRepository;

        public HomeController(IVideoGameRepository videoGameRepository, IConsoleRepository consoleRepository)
        {
            _videoGameRepository = videoGameRepository;
            _consoleRepository = consoleRepository;
        }

        public ActionResult Index()
        {
            var model = new HomeModel
            {
                VideoGames = _videoGameRepository.GetAllVideoGames(),
                Consoles = GetConsoles()
            };
            return View(model);
        }

        public ActionResult Console(string id)
        {
            Ensure.ArgumentIsNotNull(id, "id");

            var console = _consoleRepository.GetAllConsoles().FirstOrDefault(x => x.Id.ToString() == id);
            
            if(console == null) throw new ArgumentException(string.Format("No console exists with the id {0}.", id));

            var games = _videoGameRepository.GetAllVideoGames().Where(x => x.ConsolesIds.Contains(console.Id));

            var model = new ConsoleModel
            {
                Console = console,
                Consoles = GetConsoles(),
                Games = games
            };

            return View(model);
        }

        private IEnumerable<Console> GetConsoles()
        {
            return _consoleRepository.GetAllConsoles();
        }
    }
}
