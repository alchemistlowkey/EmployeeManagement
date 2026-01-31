using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        // GET: ErrorController
        [Route("Error/{statusCode}")]
        public ActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    logger.LogWarning($"404 Error occured. Path = {statusCodeResult.OriginalPath} and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            logger.LogError($"The Path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");
            return View("Error");
        }

    }
}
