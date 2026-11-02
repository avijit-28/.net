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
            objuser.usersinfo = GetUsersFromDatabase();
            return View(objuser);
        }

        // POST: User/InsertUserDetails (Synchronous Form Submission)
        [HttpPost]
        public ActionResult InsertUserDetails(UserDetails user)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usercrudoperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", (object)user.UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@education", (object)user.Education ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@location", (object)user.Location ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", "INSERT");

                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            ViewData["result"] = rowsAffected > 0 ? "1" : "0";

            UserDetails objuser = new UserDetails();
            objuser.usersinfo = GetUsersFromDatabase();
            return View(objuser);
        }

        // AJAX POST: User/InsertUserAjax
        [HttpPost]
        public JsonResult InsertUserAjax(UserDetails user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName))
            {
                return Json(new { success = false, message = "User Name is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usercrudoperations", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@name", user.UserName.Trim());
                        cmd.Parameters.AddWithValue("@education", (object)user.Education ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@location", (object)user.Location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", "INSERT");

                        con.Open();
                        int rows = cmd.ExecuteNonQuery();
                        con.Close();

                        // With SET NOCOUNT ON in stored procedures, ExecuteNonQuery returns -1 on success
                        bool isSuccess = rows > 0 || rows == -1;
                        return Json(new { success = isSuccess, message = "User registered successfully via usercrudoperations!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX GET: User/GetUsersJson
        [HttpGet]
        public JsonResult GetUsersJson()
        {
            var list = GetUsersFromDatabase();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private List<UserDetails> GetUsersFromDatabase()
        {
            List<UserDetails> userlist = new List<UserDetails>();
            DataSet ds = new DataSet();

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
                            uobj.UserName = ds.Tables[0].Rows[i]["username"] != DBNull.Value ? ds.Tables[0].Rows[i]["username"].ToString() : string.Empty;
                            uobj.Education = ds.Tables[0].Rows[i]["education"] != DBNull.Value ? ds.Tables[0].Rows[i]["education"].ToString() : string.Empty;
                            uobj.Location = ds.Tables[0].Rows[i]["location"] != DBNull.Value ? ds.Tables[0].Rows[i]["location"].ToString() : string.Empty;
                            userlist.Add(uobj);
                        }
                    }
                }
                con.Close();
            }

            return userlist;
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

        // 1. HttpStatusCodeResult with Unauthorized status (401)
        public HttpStatusCodeResult UnauthorizedStatusCode()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not authorized to access this controller action.");
        }

        // 2. HttpStatusCodeResult with BadGateway status (502)
        public HttpStatusCodeResult BadGateway()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "I have no idea what this error means.");
        }

        // 3. HttpUnauthorizedResult (401)
        public HttpStatusCodeResult UnauthorizedResult()
        {
            return new HttpUnauthorizedResult("You are not authorized to access this controller action.");
        }

        // 4. HttpNotFoundResult (404)
        public HttpNotFoundResult NotFoundAction()
        {
            return HttpNotFound("We didn't find that action, sorry!");
        }
    }
}
