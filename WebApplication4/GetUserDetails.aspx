<%@ Page Title="User Details Search (Web Service)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GetUserDetails.aspx.cs" Inherits="WebApplication4.GetUserDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .user-portal-container {
            width: 100%;
            max-width: 1080px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 24px;
        }

        .portal-header-card {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 16px;
            padding: 28px 32px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .portal-title-area {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .portal-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.35);
            border: 1px solid rgba(147, 197, 253, 0.3);
            color: #bfdbfe;
            font-size: 12px;
            font-weight: 600;
            padding: 4px 12px;
            border-radius: 20px;
            letter-spacing: 0.04em;
            text-transform: uppercase;
            align-self: flex-start;
        }

        .portal-title {
            font-size: 26px;
            font-weight: 800;
            letter-spacing: -0.02em;
            color: #ffffff;
        }

        .portal-subtitle {
            font-size: 14px;
            color: #94a3b8;
        }

        .search-card {
            background: #ffffff;
            border-radius: 16px;
            padding: 24px 28px;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.05);
            border: 1px solid #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 16px;
        }

        .search-form-group {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            flex: 1;
        }

        .search-label {
            font-size: 15px;
            font-weight: 600;
            color: #334155;
            white-space: nowrap;
        }

        .textbox {
            width: 320px;
            max-width: 100%;
            padding: 10px 14px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            font-size: 14px;
            transition: all 0.2s ease;
        }

        .textbox:focus {
            outline: none;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .btn {
            padding: 10px 22px;
            background: linear-gradient(135deg, #16a34a 0%, #15803d 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            box-shadow: 0 2px 6px rgba(22, 163, 74, 0.3);
        }

        .btn:hover {
            background: linear-gradient(135deg, #15803d 0%, #166534 100%);
            transform: translateY(-1px);
            box-shadow: 0 4px 10px rgba(22, 163, 74, 0.35);
        }

        .btn:active {
            transform: translateY(0);
        }

        .grid-card {
            background: #ffffff;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.05);
            border: 1px solid #e2e8f0;
            overflow-x: auto;
        }

        .grid {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            overflow: hidden;
        }

        .grid th {
            background: #2563eb;
            color: #ffffff;
            padding: 14px 16px;
            font-size: 13px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            text-align: center;
            border: none;
        }

        .grid td {
            padding: 12px 16px;
            text-align: center;
            border-top: 1px solid #e2e8f0;
            font-size: 14px;
            color: #1e293b;
        }

        .grid tr:hover td {
            background-color: #f8fafc;
        }

        .empty-row td {
            padding: 28px !important;
            font-size: 15px !important;
            color: #dc2626 !important;
            font-weight: 600;
            background: #fef2f2 !important;
        }

        .footer-note {
            text-align: center;
            color: #64748b;
            font-size: 13px;
            margin-top: 8px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="user-portal-container">

        <!-- Header Card -->
        <div class="portal-header-card">
            <div class="portal-title-area">
                <span class="portal-badge">&#9881; SOAP Web Service Consumer</span>
                <h1 class="portal-title">User Details Search Portal</h1>
                <p class="portal-subtitle">Consumes WebService1.asmx to search and retrieve student/user accounts from SQL Server.</p>
            </div>
        </div>

        <!-- Search Form Card -->
        <asp:Panel runat="server" CssClass="search-card" DefaultButton="btnSubmit">
            <div class="search-form-group">
                <label class="search-label">&#128100; Enter User Name :</label>

                <asp:TextBox ID="txtUserName"
                    runat="server"
                    CssClass="textbox"
                    placeholder="Enter name (e.g. Amit, Priya, Rahul)...">
                </asp:TextBox>

                <asp:Button ID="btnSubmit"
                    runat="server"
                    Text="Search User"
                    CssClass="btn"
                    OnClick="btnSubmit_Click" />
            </div>
        </asp:Panel>

        <!-- GridView Card -->
        <div class="grid-card">
            <asp:GridView ID="gvUserDetails"
                runat="server"
                CssClass="grid"
                Width="100%"
                AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                EmptyDataText="No User Found">

                <EmptyDataRowStyle CssClass="empty-row" />

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

        <div class="footer-note">
            User Management System | ASP.NET Web Service (ASMX) Demo &bull; WebApplication4
        </div>

    </div>
</asp:Content>
