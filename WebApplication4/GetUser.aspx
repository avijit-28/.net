<%@ Page Title="User Details via QueryString" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GetUser.aspx.cs" Inherits="MiniProject.GetUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .getuser-container {
            width: 100%;
            max-width: 580px;
            margin: 0 auto;
        }

        /* Card Container */
        .card {
            background: #ffffff;
            border-radius: 18px;
            box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 4px 12px -2px rgba(0, 0, 0, 0.04);
            border: 1px solid #e2e8f0;
            overflow: hidden;
            transition: box-shadow 0.3s ease;
        }

        .card:hover {
            box-shadow: 0 20px 35px -5px rgba(0, 0, 0, 0.12), 0 8px 16px -2px rgba(0, 0, 0, 0.06);
        }

        /* Card Header */
        .card-header {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            padding: 26px 30px;
            color: #ffffff;
            text-align: center;
        }

        .header-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(8px);
            padding: 4px 14px;
            border-radius: 20px;
            font-size: 11.5px;
            font-weight: 700;
            letter-spacing: 0.05em;
            text-transform: uppercase;
            margin-bottom: 10px;
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .header-title {
            font-size: 24px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .header-subtitle {
            font-size: 13.5px;
            color: #a7f3d0;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 28px 30px;
        }

        /* Status Banners */
        .status-pill-success {
            display: flex;
            align-items: center;
            gap: 8px;
            background: #ecfdf5;
            color: #065f46;
            border: 1px solid #a7f3d0;
            padding: 10px 14px;
            border-radius: 10px;
            font-size: 12.5px;
            font-weight: 600;
            margin-bottom: 20px;
        }

        .status-pill-notice {
            background: #fffbeb;
            color: #92400e;
            border: 1px solid #fde68a;
            padding: 12px 14px;
            border-radius: 10px;
            font-size: 12.5px;
            line-height: 1.5;
            margin-bottom: 20px;
        }

        .user-selector-wrap {
            margin-top: 10px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .user-select-dropdown {
            width: 100%;
            padding: 9px 12px;
            font-size: 13px;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            background: #ffffff;
            color: #1e293b;
            font-family: inherit;
            outline: none;
            cursor: pointer;
        }

        .user-select-dropdown:focus {
            border-color: #059669;
            box-shadow: 0 0 0 3px rgba(5, 150, 105, 0.15);
        }

        /* Avatar */
        .avatar-section {
            display: flex;
            justify-content: center;
            margin-bottom: 20px;
        }

        .avatar-circle {
            width: 72px;
            height: 72px;
            background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
            border: 2px solid #a7f3d0;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            font-weight: 800;
            color: #059669;
            box-shadow: 0 4px 12px rgba(5, 150, 105, 0.15);
        }

        /* Detail Boxes Grid */
        .details-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            margin-bottom: 14px;
        }

        .detail-box {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 14px 16px;
            display: flex;
            flex-direction: column;
            gap: 4px;
            transition: all 0.2s ease;
        }

        .detail-box.full-width {
            grid-column: 1 / -1;
        }

        .detail-box:hover {
            border-color: #cbd5e1;
            background: #ffffff;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
        }

        .detail-label {
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: #64748b;
            font-weight: 700;
        }

        .profile-value {
            font-size: 15px;
            font-weight: 700;
            color: #0f172a;
            word-break: break-all;
        }

        /* Informational Card */
        .info-card {
            background: #f0fdf4;
            border: 1px solid #bbf7d0;
            border-radius: 10px;
            padding: 12px 16px;
            font-size: 12.5px;
            color: #166534;
            line-height: 1.5;
            margin: 16px 0 22px;
        }

        /* Actions */
        .actions-group {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .btn-search-again {
            flex: 1;
            min-width: 170px;
            height: 44px;
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(5, 150, 105, 0.25);
            transition: all 0.2s ease;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-search-again:hover {
            background: linear-gradient(135deg, #047857 0%, #065f46 100%);
            box-shadow: 0 6px 16px rgba(5, 150, 105, 0.35);
            transform: translateY(-1px);
        }

        .btn-dashboard {
            flex: 1;
            min-width: 170px;
            height: 44px;
            background: #f1f5f9;
            color: #334155;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s ease;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-dashboard:hover {
            background: #e2e8f0;
            color: #0f172a;
        }

        /* Card Footer */
        .card-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 14px 28px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13px;
        }

        .nav-link {
            color: #059669;
            text-decoration: none;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #047857;
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="getuser-container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">User Profile Record</div>
                <h1 class="header-title">&#128100; User Details</h1>
                <p class="header-subtitle">Record retrieved and displayed via QueryString data.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                
                <!-- QueryString Status Readout -->
                <asp:Panel ID="pnlQuerySuccess" runat="server" Visible="false" CssClass="status-pill-success">
                    <span>&#9989;</span>
                    <div>
                        <strong>QueryString Received:</strong> 
                        <code>user="<asp:Literal ID="litQueryUser" runat="server" />"</code> &bull; 
                        <code>email="<asp:Literal ID="litQueryEmail" runat="server" />"</code>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlQueryNotice" runat="server" Visible="false" CssClass="status-pill-notice">
                    <span>&#128161;</span> <strong>Notice:</strong> No query string parameter was provided in the URL.
                    Showing default user profile from database. <strong>Select a student below to test QueryString:</strong>
                    <div class="user-selector-wrap">
                        <asp:DropDownList ID="ddlUsers" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged" CssClass="user-select-dropdown">
                        </asp:DropDownList>
                    </div>
                </asp:Panel>

                <!-- Avatar -->
                <div class="avatar-section">
                    <div class="avatar-circle">
                        <asp:Literal ID="litAvatarInitial" runat="server" Text="&#128100;" />
                    </div>
                </div>

                <!-- Profile Details Grid -->
                <div class="details-grid">
                    <!-- User ID -->
                    <div class="detail-box">
                        <span class="detail-label">User ID</span>
                        <asp:Label ID="LblUserId" runat="server" CssClass="profile-value" Text="#1"></asp:Label>
                    </div>

                    <!-- City -->
                    <div class="detail-box">
                        <span class="detail-label">Location / City</span>
                        <asp:Label ID="LblCity" runat="server" CssClass="profile-value" Text="General"></asp:Label>
                    </div>

                    <!-- Username Box -->
                    <div class="detail-box full-width">
                        <span class="detail-label">User Name</span>
                        <asp:Label ID="LblName" runat="server" CssClass="profile-value" Text="Not specified"></asp:Label>
                    </div>

                    <!-- Email Box -->
                    <div class="detail-box full-width">
                        <span class="detail-label">Email Address</span>
                        <asp:Label ID="LblEmail" runat="server" CssClass="profile-value" Text="Not specified"></asp:Label>
                    </div>

                    <!-- Mobile Box -->
                    <div class="detail-box full-width">
                        <span class="detail-label">Mobile Number</span>
                        <asp:Label ID="LblMobile" runat="server" CssClass="profile-value" Text="Not specified"></asp:Label>
                    </div>
                </div>

                <!-- Info note -->
                <div class="info-card">
                    &#128161; <strong>QueryString Data Transmission:</strong> Values are passed from <code>SearchUser.aspx</code> using <code>Request.QueryString["user"]</code> and <code>Request.QueryString["email"]</code>.
                </div>

                <!-- Action Buttons -->
                <div class="actions-group">
                    <asp:Button ID="BtnBackToSearch" runat="server" OnClick="BtnBackToSearch_Click" Text="&#128269; Search Another User" CssClass="btn-search-again" CausesValidation="false" />
                    <asp:Button ID="BtnGoToDashboard" runat="server" OnClick="BtnGoToDashboard_Click" Text="&#127968; Go to SignUp / Dashboard" CssClass="btn-dashboard" CausesValidation="false" />
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/SearchUser") %>" class="nav-link">&larr; Back to Search</a>
                <a href="<%= ResolveUrl("~/SignUp") %>" class="nav-link">Dashboard &rarr;</a>
            </div>
        </div>
    </div>
</asp:Content>
