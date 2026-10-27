using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class CrystalReportPractice : System.Web.UI.Page
{
    private ReportDocument rd;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindCountry();
        }

        // Always ensure report data is bound so viewer pagination and export work seamlessly
        LoadReport();
    }

    private string GetConnectionString()
    {
        if (ConfigurationManager.ConnectionStrings["EmployeeDbConnection"] != null &&
            !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["EmployeeDbConnection"].ConnectionString))
        {
            return ConfigurationManager.ConnectionStrings["EmployeeDbConnection"].ConnectionString;
        }

        // Fallback default
        return "Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234;Connect Timeout=30;";
    }

    private void BindCountry()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string query = "SELECT DISTINCT Country FROM [dbo].[AllEmployee] WHERE Country IS NOT NULL AND LTRIM(RTRIM(Country)) <> '' ORDER BY Country ASC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlCountry.DataSource = dt;
                        ddlCountry.DataTextField = "Country";
                        ddlCountry.DataValueField = "Country";
                        ddlCountry.DataBind();

                        ddlCountry.Items.Insert(0, new ListItem("-- All Countries --", ""));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowAlert("Note on Countries: " + ex.Message, "warning");
        }
    }

    public void LoadReport()
    {
        try
        {
            string fname = (txtFirstName.Text ?? string.Empty).Trim();
            string country = (ddlCountry.SelectedValue ?? string.Empty).Trim();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM [dbo].[AllEmployee] 
                                WHERE (@fname = '' OR FirstName LIKE '%' + @fname + '%') 
                                  AND (@country = '' OR Country = @country)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 100).Value = fname;
                    cmd.Parameters.Add("@country", SqlDbType.NVarChar, 100).Value = country;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "AllEmployee");

                        int rowCount = ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0;
                        lblRecordCount.Text = string.Format("<i class='fa-solid fa-users me-1'></i> {0} Record{1} Found", rowCount, rowCount == 1 ? "" : "s");

                        // Auto-standardize employee photos to uniform Passport Size (420x540 @ 96 DPI, 3.5:4.5 ratio)
                        if (ds.Tables.Count > 0 && ds.Tables[0].Columns.Contains("Photo"))
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (row["Photo"] != DBNull.Value)
                                {
                                    string originalPhoto = row["Photo"].ToString();
                                    string standardizedPhoto = EnsureStandardizedPhoto(originalPhoto);
                                    if (!string.IsNullOrEmpty(standardizedPhoto))
                                    {
                                        row["Photo"] = standardizedPhoto;
                                    }
                                }
                            }
                        }

                        string reportPath = Server.MapPath("~/CrystalReport2.rpt");
                        if (!File.Exists(reportPath))
                        {
                            reportPath = Server.MapPath("~/CrystalReport.rpt");
                        }

                        if (!File.Exists(reportPath))
                        {
                            ShowAlert("Report file not found. Please verify CrystalReport2.rpt exists in the project root.", "danger");
                            return;
                        }

                        // Close previous report if any
                        if (rd != null)
                        {
                            rd.Close();
                            rd.Dispose();
                        }

                        rd = new ReportDocument();
                        rd.Load(reportPath);
                        rd.SetDataSource(ds.Tables[0]);

                        CrystalReportViewer1.ReportSource = rd;
                        CrystalReportViewer1.DataBind();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowAlert("Error loading report: " + ex.Message, "danger");
        }
    }

    /// <summary>
    /// Ensures that dynamic employee photos are formatted in standard Passport Size
    /// (420x540 pixels @ 96 DPI, 35mm x 45mm standard portrait aspect ratio 7:9).
    /// </summary>
    private string EnsureStandardizedPhoto(string photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath) || !File.Exists(photoPath))
        {
            return photoPath;
        }

        try
        {
            const int targetWidth = 420;
            const int targetHeight = 540;
            const float targetDpi = 96.0f;

            string dir = Path.GetDirectoryName(photoPath);
            string filename = Path.GetFileName(photoPath);
            string cacheDir = Path.Combine(dir, "Normalized_Passport");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            string cachedPath = Path.Combine(cacheDir, filename);

            if (File.Exists(cachedPath) && File.GetLastWriteTimeUtc(cachedPath) >= File.GetLastWriteTimeUtc(photoPath))
            {
                return cachedPath;
            }

            using (System.Drawing.Image src = System.Drawing.Image.FromFile(photoPath))
            {
                // If the source image already has exact passport dimensions and DPI, return it
                if (src.Width == targetWidth && src.Height == targetHeight &&
                    Math.Abs(src.HorizontalResolution - targetDpi) < 1.0f &&
                    Math.Abs(src.VerticalResolution - targetDpi) < 1.0f)
                {
                    return photoPath;
                }

                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    bmp.SetResolution(targetDpi, targetDpi);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.Clear(System.Drawing.Color.White);

                        double srcRatio = (double)src.Width / src.Height;
                        double targetRatio = (double)targetWidth / targetHeight;

                        int drawX = 0, drawY = 0, drawW = targetWidth, drawH = targetHeight;
                        if (srcRatio > targetRatio)
                        {
                            // Landscape source: scale by height, center horizontally
                            double scale = (double)targetHeight / src.Height;
                            drawW = (int)(src.Width * scale);
                            drawX = (targetWidth - drawW) / 2;
                        }
                        else
                        {
                            // Portrait source: scale by width, align towards upper portion for face
                            double scale = (double)targetWidth / src.Width;
                            drawH = (int)(src.Height * scale);
                            drawY = (int)((targetHeight - drawH) * 0.15);
                        }

                        g.DrawImage(src, new System.Drawing.Rectangle(drawX, drawY, drawW, drawH));
                    }

                    System.Drawing.Imaging.ImageCodecInfo jpegCodec = null;
                    foreach (var codec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
                    {
                        if (codec.MimeType == "image/jpeg")
                        {
                            jpegCodec = codec;
                            break;
                        }
                    }

                    if (jpegCodec != null)
                    {
                        using (var encParams = new System.Drawing.Imaging.EncoderParameters(1))
                        {
                            encParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95L);
                            bmp.Save(cachedPath, jpegCodec, encParams);
                        }
                    }
                    else
                    {
                        bmp.Save(cachedPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }

            return cachedPath;
        }
        catch
        {
            // Fallback gracefully
            return photoPath;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadReport();
        string fname = txtFirstName.Text.Trim();
        string country = ddlCountry.SelectedItem != null ? ddlCountry.SelectedItem.Text : "";

        string msg = "Search executed";
        if (!string.IsNullOrEmpty(fname) || (ddlCountry.SelectedIndex > 0))
        {
            msg += string.Format(" for Name: '{0}', Country: '{1}'", 
                string.IsNullOrEmpty(fname) ? "Any" : fname,
                ddlCountry.SelectedIndex > 0 ? country : "All");
        }
        ShowAlert(msg, "success");
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtFirstName.Text = string.Empty;
        if (ddlCountry.Items.Count > 0)
        {
            ddlCountry.SelectedIndex = 0;
        }
        LoadReport();
        ShowAlert("Filters have been reset. Showing all employees.", "info");
    }

    protected void btnPdf_Download(object sender, EventArgs e)
    {
        ExportReport(ExportFormatType.PortableDocFormat, "EmployeeReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
    }

    protected void btnExcel_Download(object sender, EventArgs e)
    {
        ExportReport(ExportFormatType.Excel, "EmployeeReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
    }

    private void ExportReport(ExportFormatType formatType, string fileName)
    {
        try
        {
            LoadReport();

            if (rd != null && rd.IsLoaded)
            {
                rd.ExportToHttpResponse(formatType, Response, true, fileName);
                Response.End();
            }
            else
            {
                ShowAlert("Unable to export: Report document is not loaded.", "warning");
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
            // Expected exception caused by Response.End()
        }
        catch (Exception ex)
        {
            ShowAlert("Export failed: " + ex.Message, "danger");
        }
    }

    private void ShowAlert(string message, string alertType = "info")
    {
        pnlAlert.Visible = true;
        pnlAlert.CssClass = string.Format("alert alert-{0} alert-dismissible fade show", alertType);
        litAlertMessage.Text = string.Format("<strong>Notice:</strong> {0}", message);
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        // Clean up Crystal Report Document COM and temp file handles
        if (rd != null)
        {
            try
            {
                rd.Close();
                rd.Dispose();
            }
            catch
            {
                // Suppress disposal exceptions on page unload
            }
            finally
            {
                rd = null;
            }
        }
    }
}
