using System;
using System.Management.Instrumentation;
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

        public IHttpActionResult Get(string id)
        {
            var eventKey = id;
            try
            {
                return Json(DataRepo.GetGameDetail(eventKey));
            }
            catch (InstanceNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
