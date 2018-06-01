using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TemplateWebCore.Common
{
    public class CommonAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase contextBase)
        {
            return base.AuthorizeCore(contextBase);
        }
    }
}