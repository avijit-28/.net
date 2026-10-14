using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{

    public partial class CheckAvailability : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CheckUserNameAvailability(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtun.Text))
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    String query = "SELECT COUNT(*) FROM Users WHERE [UserName] = @un";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                    }
                }
            }
        }
    }
}