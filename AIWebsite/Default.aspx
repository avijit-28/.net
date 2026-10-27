<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("~/CrystalReportPractice.aspx");
    }
</script>
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="refresh" content="0;url=CrystalReportPractice.aspx" />
    <title>Redirecting...</title>
</head>
<body>
    <p>Redirecting to <a href="CrystalReportPractice.aspx">Employee Crystal Report Dashboard</a>...</p>
</body>
</html>
