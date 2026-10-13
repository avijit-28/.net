using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class ProductForm : System.Web.UI.Page
    {
        public string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void BtnAddPrd_Click(object sender, EventArgs e)
        {

            String Dtls = "";
            Dtls += " Product Id "+TxtPdrId.Text + "\n";
            Dtls += " Product Name "+TxtPrdName.Text + "\n";
            Dtls += " Product Price "+TxtPrdPrc.Text + "\n";
            Dtls += " Stock "+TxtPrdStk.Text + "\n";

            using(SqlConnection con = new SqlConnection(conStr))
            {
                string query = "insert into Products values(@PrdId,@PrdName,@PrdPrc,@PrdStk)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PrdId", TxtPdrId.Text);
                    cmd.Parameters.AddWithValue("@PrdName", TxtPrdName.Text);
                    cmd.Parameters.AddWithValue("@PrdPrc", TxtPrdPrc.Text);
                    cmd.Parameters.AddWithValue("@PrdStk", TxtPrdStk.Text);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Dtls += "Product added successfully.";
                    }
                    else
                    {
                        Dtls += "Failed to add product.";
                    }
                }
            }


            LblMsg.Text = Dtls;

        }
    }
}