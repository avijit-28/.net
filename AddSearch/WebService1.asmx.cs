using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;

namespace AddSearch
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

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        
        public XmlElement GetUserDetails(string name)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Products where UserName like @name+'%'", con);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // Create an instance of DataSet.
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            // Return the DataSet as an XmlElement.
            XmlDataDocument xmldata = new XmlDataDocument(ds);
            XmlElement xmlElement = xmldata.DocumentElement;
            return xmlElement;


        }
}
