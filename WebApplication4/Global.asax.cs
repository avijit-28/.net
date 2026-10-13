using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplication4
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            
            Application["user"] = 0;

        }

            Application.Lock();
            Application["user"] = ((int?)Application["user"] ?? 0) + 1;
            Application.UnLock();
        }
        

        protected void Application_BeginRequest(object sender, EventArgs e)
        {


        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            int count = (int?)Application["user"] ?? 0;
            if (count > 0)
            {
                Application["user"] = count - 1;
            }
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}