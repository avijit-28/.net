using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;
namespace WebServices1
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
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
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
                string query = "Select * from [UserDetails] where Name like @UserName+'%' ";
                using(SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    using(SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        XmlDataDocument xmlDoc = new XmlDataDocument(ds);
                        XmlElement xmlElement = xmlDoc.DocumentElement;
                        return xmlElement;
                    }
                }
            }
        }
    }
}
