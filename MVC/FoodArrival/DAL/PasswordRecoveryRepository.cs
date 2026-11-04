using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class PasswordRecoveryRepository
    {
        public bool SubmitRequest(string email, string reason, out int requestId, out string userRole, out string message)
        {
            requestId = 0;
            userRole = "Customer";
            message = string.Empty;

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_SubmitRequest", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Reason", reason ?? string.Empty);

                    var outReqId = new SqlParameter("@RequestId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var outRole = new SqlParameter("@UserRole", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
                    var outFound = new SqlParameter("@UserFound", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    var outMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outReqId);
                    cmd.Parameters.Add(outRole);
                    cmd.Parameters.Add(outFound);
                    cmd.Parameters.Add(outMsg);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outReqId.Value != DBNull.Value && outReqId.Value != null)
                        requestId = Convert.ToInt32(outReqId.Value);

                    if (outRole.Value != DBNull.Value && outRole.Value != null)
                        userRole = outRole.Value.ToString();

                    message = outMsg.Value != DBNull.Value && outMsg.Value != null ? outMsg.Value.ToString() : "Request submitted.";
                    return requestId > 0;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public PasswordResetRequest GetLatestByEmail(string email)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_GetLatestByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PasswordResetRequest
                            {
                                RequestId = Convert.ToInt32(reader["RequestId"]),
                                Email = reader["Email"].ToString(),
                                UserRole = reader["UserRole"].ToString(),
                                UserId = reader["UserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["UserId"]),
                                UserName = reader["UserName"] == DBNull.Value ? string.Empty : reader["UserName"].ToString(),
                                Reason = reader["Reason"].ToString(),
                                Status = reader["Status"].ToString(),
                                AdminNotes = reader["AdminNotes"] == DBNull.Value ? string.Empty : reader["AdminNotes"].ToString(),
                                ApprovedByAdminId = reader["ApprovedByAdminId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ApprovedByAdminId"]),
                                ApprovedAt = reader["ApprovedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ApprovedAt"]),
                                CompletedAt = reader["CompletedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CompletedAt"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public bool SetNewPassword(string email, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_SetNewPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword ?? string.Empty);

                    var outSuccess = new SqlParameter("@IsSuccess", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    var outMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    bool isSuccess = outSuccess.Value != DBNull.Value && Convert.ToBoolean(outSuccess.Value);
                    message = outMsg.Value != DBNull.Value && outMsg.Value != null ? outMsg.Value.ToString() : string.Empty;
                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<PasswordResetRequest> GetAllRequests(string status = null, string searchEmail = null)
        {
            var list = new List<PasswordResetRequest>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_GetAllRequests", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SearchEmail", (object)searchEmail ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new PasswordResetRequest
                            {
                                RequestId = Convert.ToInt32(reader["RequestId"]),
                                Email = reader["Email"].ToString(),
                                UserRole = reader["UserRole"].ToString(),
                                UserId = reader["UserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["UserId"]),
                                UserName = reader["UserName"] == DBNull.Value ? string.Empty : reader["UserName"].ToString(),
                                Reason = reader["Reason"].ToString(),
                                Status = reader["Status"].ToString(),
                                AdminNotes = reader["AdminNotes"] == DBNull.Value ? string.Empty : reader["AdminNotes"].ToString(),
                                ApprovedByAdminId = reader["ApprovedByAdminId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ApprovedByAdminId"]),
                                ApprovedByAdminName = reader["ApprovedByAdminName"] == DBNull.Value ? string.Empty : reader["ApprovedByAdminName"].ToString(),
                                ApprovedAt = reader["ApprovedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ApprovedAt"]),
                                CompletedAt = reader["CompletedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CompletedAt"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool ApproveRequest(int requestId, int adminId, string adminNotes, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_Approve", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.Parameters.AddWithValue("@AdminNotes", (object)adminNotes ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = $"Request #{requestId} has been approved. The user can now enter their new password.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectRequest(int requestId, int adminId, string adminNotes, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_Reject", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.Parameters.AddWithValue("@AdminNotes", (object)adminNotes ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = $"Request #{requestId} has been rejected.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public UserVerificationResult VerifyUserByEmail(string email)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_PasswordRecovery_VerifyUserByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool found = Convert.ToBoolean(reader["Found"]);
                            return new UserVerificationResult
                            {
                                Found = found,
                                UserRole = reader["UserRole"] == DBNull.Value ? string.Empty : reader["UserRole"].ToString(),
                                UserId = reader["UserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["UserId"]),
                                FullName = reader["FullName"] == DBNull.Value ? string.Empty : reader["FullName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                CreatedAt = reader["CreatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return new UserVerificationResult { Found = false, Email = email };
        }
    }
}
