using System.Web.Http;
using HockeyStats.CLI.DomainServices;

namespace HockeyStats.CLI.Api.Controllers
{
    public class GamesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Json(DataRepo.GetGames());
        }
    }
}
