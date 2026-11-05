using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebApiPractice2.Models.BEL;
using WebApiPractice2.Models.DAL;

namespace WebApiPractice2.Models.BLL
{
    public class UserService
    {
        private DBFunction _df;
        private DataTable _dt;
        private SqlParameter[] _param;

        public UserService()
        {
            _df = new DBFunction();
        }

        public Users getLoginDetails(int UserID, string pwd)
        {
            _param = new SqlParameter[]
            {
                new SqlParameter("@UserID", UserID),
                new SqlParameter("@UserPassword", pwd ?? string.Empty)
            };
            _dt = _df.DTWithProc("spGetLoginDetails", _param);
            if (_dt.Rows.Count > 0)
            {
                return _dt.AsEnumerable().Select(row => new Users
                {
                    UserID = row.Field<int>("UserID"),
                    UserName = row.Field<string>("UserName"),
                    UserPassword = row.Field<string>("UserPassword"),
                    UserRoles = row.Field<string>("UserRoles"),
                    UserEmailID = row.Field<string>("UserEmailID")
                }).FirstOrDefault();
            }
            else
            {
                return new Users();
            }
        }

        public string InsertUser(Users user)
        {
            _param = new SqlParameter[]
            {
                new SqlParameter("@UserID", user.UserID),
                new SqlParameter("@UserName", (object)user.UserName ?? DBNull.Value),
                new SqlParameter("@UserPassword", (object)user.UserPassword ?? DBNull.Value),
                new SqlParameter("@UserRoles", (object)user.UserRoles ?? DBNull.Value),
                new SqlParameter("@UserEmailID", (object)user.UserEmailID ?? DBNull.Value)
            };
            object result = _df.executeScalarWithProc("spInsertUsers", _param);
            return result != null ? result.ToString() : "User processed.";
        }

        public string UpdateUser(int UserID, Users user)
        {
            _param = new SqlParameter[]
            {
                new SqlParameter("@UserID", UserID),
                new SqlParameter("@UserName", (object)user.UserName ?? DBNull.Value),
                new SqlParameter("@UserPassword", (object)user.UserPassword ?? DBNull.Value),
                new SqlParameter("@UserRoles", (object)user.UserRoles ?? DBNull.Value),
                new SqlParameter("@UserEmailID", (object)user.UserEmailID ?? DBNull.Value)
            };
            object result = _df.executeScalarWithProc("spUpdateUsers", _param);
            return result != null ? result.ToString() : "User updated.";
        }

        public string DeleteUser(int UserID)
        {
            _param = new SqlParameter[]
            {
                new SqlParameter("@UserID", UserID)
            };
            object result = _df.executeScalarWithProc("spDeleteUser", _param);
            return result != null ? result.ToString() : "User deleted.";
        }

        public List<Users> GetAllUsers()
        {
            var list = new List<Users>();
            try
            {
                _dt = _df.DTWithProc("spGetAllUsers", null);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    foreach (DataRow row in _dt.Rows)
                    {
                        list.Add(new Users
                        {
                            UserID = Convert.ToInt32(row["UserID"]),
                            UserName = row["UserName"]?.ToString(),
                            UserPassword = row["UserPassword"]?.ToString(),
                            UserRoles = row["UserRoles"]?.ToString(),
                            UserEmailID = row["UserEmailID"]?.ToString()
                        });
                    }
                }
            }
            catch
            {
                // Fallback if needed
            }
            return list;
        }
    }
}
