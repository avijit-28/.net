<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="GetUserDetails.aspx.cs"
    Inherits="WebServices1.GetUserDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Search</title>

    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f4f6f9;
            margin: 0;
            padding: 0;
        }

        .container {
            width: 800px;
            margin: 50px auto;
            background: white;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0px 0px 12px #ccc;
        }

        .header {
            text-align: center;
            color: white;
            background-color: #0078D7;
            padding: 15px;
            border-radius: 8px;
            font-size: 28px;
            font-weight: bold;
        }

        .search-panel {
            margin-top: 25px;
            text-align: center;
        }

        .textbox {
            width: 250px;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }

        .btn {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 9px 18px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
        }

        .btn:hover {
            background-color: #218838;
        }

        .grid {
            margin-top: 25px;
            width: 100%;
        }

        .grid th {
            background-color: #0078D7;
            color: white;
            padding: 10px;
        }

        .grid td {
            padding: 8px;
        }

        .footer {
            text-align: center;
            margin-top: 20px;
            color: gray;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="container">

            <div class="header">
                Student Search Using Web Service
            </div>

            <div class="search-panel">

                <label><b>Enter Student Name :</b></label>

                <asp:TextBox ID="txtUserName"
                    runat="server"
                    CssClass="textbox">
                </asp:TextBox>

                &nbsp;

                <asp:Button ID="btnSubmit"
                    runat="server"
                    Text="Search"
                    CssClass="btn"
                    OnClick="btnSubmit_Click" />

            </div>

            <div style="margin-top:20px;">

                <asp:GridView ID="gvUserDetails"
                    runat="server"
                    CssClass="grid"
                    Width="100%"
                    CellPadding="6"
                    GridLines="Horizontal"
                    EmptyDataText="No Record Found"
                    AutoGenerateColumns="true">

                    <HeaderStyle BackColor="#0078D7"
                        ForeColor="White"
                        Font-Bold="true" />

                    <RowStyle BackColor="White" />

                    <AlternatingRowStyle BackColor="#F2F2F2" />

                </asp:GridView>

            </div>

            <div class="footer">
                ASP.NET Web Service Demo
            </div>

        </div>

    </form>
</body>
</html>