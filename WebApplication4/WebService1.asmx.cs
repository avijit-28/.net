using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace WebApplication4
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                                       ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public XmlElement GetSdtDetails(string UserName)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT UserId, UserName, Password, Email, MobileNo, City FROM UserDetails WHERE UserName LIKE @UserName + '%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", (object)UserName ?? string.Empty);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet("UserDetailsSet");
                        da.Fill(ds, "UserDetails");
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(ds.GetXml());
                        return xmlDoc.DocumentElement;
                    }
                }
            }
        }

        [WebMethod]
        public XmlElement GetUserDetails(string UserName)
        {
            return GetSdtDetails(UserName);
        }
    }
}

namespace WebApplication4.localhostmcc
{
    /// <summary>
    /// Proxy compatibility class allowing consumption as localhostmcc.WebService1
    /// </summary>
    public class WebService1 : WebApplication4.WebService1
    {
        public string Url { get; set; }
    }
}
