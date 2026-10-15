<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="GetUserDetails.aspx.cs"
    Inherits="WebServices1.GetUserDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Search Portal</title>

    <style>
        body {
            font-family: Segoe UI, Arial, sans-serif;
            background: #eef2f7;
            margin: 0;
        }

        .container {
            width: 90%;
            max-width: 1000px;
            margin: 40px auto;
            background: #fff;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.15);
        }

        .header {
            text-align: center;
            background: #0d6efd;
            color: white;
            padding: 15px;
            border-radius: 10px;
            font-size: 30px;
            font-weight: bold;
        }

        .search-panel {
            text-align: center;
            margin-top: 25px;
        }

        .search-panel label {
            font-size: 16px;
            font-weight: bold;
        }

        .textbox {
            width: 280px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }

        .btn {
            padding: 10px 20px;
            background-color: #198754;
            color: white;
            border: none;
            border-radius: 5px;
            font-weight: bold;
            cursor: pointer;
        }

        .btn:hover {
            background-color: #157347;
        }

        .grid {
            margin-top: 25px;
            width: 100%;
            border-collapse: collapse;
        }

        .grid th {
            background-color: #0d6efd;
            color: white;
            padding: 12px;
            text-align: center;
        }

        .grid td {
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #ddd;
        }

        .footer {
            text-align: center;
            margin-top: 20px;
            color: gray;
            font-size: 13px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="container">

            <div class="header">
                User Details Search Portal
            </div>

            <asp:Panel runat="server" CssClass="search-panel" DefaultButton="btnSubmit">

                <label>Enter User Name :</label>

                <asp:TextBox ID="txtUserName"
                    runat="server"
                    CssClass="textbox"
                    placeholder="Enter name to search...">
                </asp:TextBox>

                &nbsp;

                <asp:Button ID="btnSubmit"
                    runat="server"
                    Text="Search User"
                    CssClass="btn"
                    OnClick="btnSubmit_Click" />

            </asp:Panel>

            <div>

                <asp:GridView ID="gvUserDetails"
                    runat="server"
                    CssClass="grid"
                    Width="100%"
                    AutoGenerateColumns="False"
                    ShowHeaderWhenEmpty="True"
                    EmptyDataText="No User Found">

                    <EmptyDataRowStyle ForeColor="#dc3545" Font-Bold="True" HorizontalAlign="Center" />

                    <Columns>

                        <asp:BoundField DataField="UserId"
                            HeaderText="User ID" />

                        <asp:BoundField DataField="UserName"
                            HeaderText="User Name" />

                        <asp:BoundField DataField="Password"
                            HeaderText="Password" />

                        <asp:BoundField DataField="Email"
                            HeaderText="Email" />

                        <asp:BoundField DataField="MobileNo"
                            HeaderText="Mobile No" />

                        <asp:BoundField DataField="City"
                            HeaderText="City" />

                    </Columns>

                </asp:GridView>

            </div>

            <div class="footer">
                User Management System | ASP.NET Web Service Demo
            </div>

        </div>

    </form>
</body>
</html>