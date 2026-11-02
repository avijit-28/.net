using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web.Mvc;
using JQuery.Models;

namespace JQuery.Controllers
{
    public class UserController : Controller
    {
        private string GetConnectionString()
        {
            var setting = ConfigurationManager.ConnectionStrings["DBConn"];
            return setting != null ? setting.ConnectionString : "Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234";
        }

        // ================================================================
        // USER DETAILS: INSERT & GET WITH STORED PROCEDURE & REDIRECT
        // ================================================================

        // GET: User/InsertUserDetails
        public ActionResult InsertUserDetails()
        {
            UserDetails objuser = new UserDetails();
            DataSet ds = new DataSet();
            List<UserDetails> userlist = new List<UserDetails>();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usercrudoperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@status", "GET");
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            UserDetails uobj = new UserDetails();
                            uobj.UserId = Convert.ToInt32(ds.Tables[0].Rows[i]["userid"].ToString());
                            uobj.UserName = ds.Tables[0].Rows[i]["username"].ToString();
                            uobj.Education = ds.Tables[0].Rows[i]["education"].ToString();
                            uobj.Location = ds.Tables[0].Rows[i]["location"].ToString();
                            userlist.Add(uobj);
                        }
                    }
                }
                con.Close();
            }

            objuser.usersinfo = userlist;
            return View(objuser);
        }

        // POST: User/InsertUserDetails
        [HttpPost]
        public ActionResult InsertUserDetails(UserDetails user)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usercrudoperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", user.UserName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@education", user.Education ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@location", user.Location ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", "INSERT");

                    con.Open();
                    ViewData["result"] = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            // Reload user list for display
            return InsertUserDetails();
        }

        // ================================================================
        // REDIRECT DEMONSTRATIONS (Redirect, RedirectToRoute, RedirectToAction)
        // ================================================================

        // 1. Redirect Result to an external URL
        public RedirectResult RedirectToExternal()
        {
            return Redirect("https://mccit.net");
        }

        // 2. RedirectToRoute Result to an action using route object
        public RedirectToRouteResult RedirectToRouteDemo()
        {
            return RedirectToRoute(new { controller = "User", action = "InsertUserDetails" });
        }

        // 3. RedirectToAction Result to a controller and action
        public ActionResult RedirectToActionDemo()
        {
            return RedirectToAction("InsertUserDetails", "User");
        }

        // ================================================================
        // STATUS RESULTS (HttpStatusCodeResult, HttpUnauthorizedResult, HttpNotFoundResult)
        // ================================================================

        // 1. HttpStatusCodeResult with Unauthorized status
        public HttpStatusCodeResult UnauthorizedStatusCode()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not authorized to access this controller action.");
        }

        // 2. HttpStatusCodeResult with BadGateway status
        public HttpStatusCodeResult BadGateway()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "I have no idea what this error means.");
        }

        // 3. HttpUnauthorizedResult
        public HttpStatusCodeResult UnauthorizedResult()
        {
            return new HttpUnauthorizedResult("You are not authorized to access this controller action.");
        }

        // 4. HttpNotFoundResult
        public HttpNotFoundResult NotFoundAction()
        {
            return HttpNotFound("We didn't find that action, sorry!");
        }
    }
}
