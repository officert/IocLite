using System.Web.Mvc;
using IocLite.SampleApp.Data;
using IocLite.SampleApp.Models;

namespace IocLite.SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVideoGameRepository _videoGameRepository;

        public HomeController(IVideoGameRepository videoGameRepository)
        {
            _videoGameRepository = videoGameRepository;
        }

        public ActionResult Index()
        {
            var model = new HomeModel
            {
                VideoGames = _videoGameRepository.GetAllVideoGames()
            };
            return View(model);
        }
    }
}
