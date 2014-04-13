using System;
using System.Management.Instrumentation;
using System.Net;
using System.Web.Http;
using HockeyStats.CLI.DomainServices;

namespace HockeyStats.CLI.Api.Controllers
{
    public class TeamsController : ApiController
    {
        public IHttpActionResult Get(int id)
        {
            try
            {
                return Json(DataRepo.GetTeam(id));
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