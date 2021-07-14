using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SuperShop.Helpers
{
    public class NotFoundViewResul : ViewResult
    {
        public NotFoundViewResul(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int) HttpStatusCode.NotFound;
        }
    }
}
