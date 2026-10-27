<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CrystalReportPractice.aspx.cs" Inherits="CrystalReportPractice" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src='<%=ResolveUrl("~/crystalreportviewers13/js/crviewer/crv.js")%>' type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="Panel1" runat="server">
            <table>
                <tr> <td><asp:Button ID="Button2" runat="server" Text="Convert to PDF" OnClick="btnPdf_Download" /></td></tr>
                <tr>
                   
                    <td><asp:Label ID="lblName" runat="server" Text="Enter First"></asp:Label></td>
                    <td><asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:DropDownList ID="ddlCountry" runat="server">
                           
                        </asp:DropDownList>
                        <asp:Button ID="Button1" runat="server" Text="Button" />
                    </td>
                    <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
                </tr>
            </table>
        </asp:Panel>
    </div>

     <div>
         <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true"
             Height="1039px" ReportSourceID="CrystalReportSource1" Width="901px" ToolPanelView="ParameterPanel"  />
         <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
             <Report FileName="CrystalReport.rpt"></Report>
         </CR:CrystalReportSource>
    </div>
    </form>
</body>
</html>
