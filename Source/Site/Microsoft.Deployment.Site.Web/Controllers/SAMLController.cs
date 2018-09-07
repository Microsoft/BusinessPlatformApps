using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Deployment.Site.Web.Controllers
{
    public class SAMLController : Controller
    {
        [Route("saml/redirect")]
        public IActionResult Redirect()
        {
            string samlResponse = Request.Form["SAMLResponse"];
            if(!string.IsNullOrWhiteSpace(samlResponse))
            {
                ViewData["samlAuthCode"] = samlResponse;
            }

            return View("~/Views/Redirect.cshtml");
        }
    }
}
