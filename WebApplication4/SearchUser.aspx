<%@ Page Title="Search User - Query System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchUser.aspx.cs" Inherits="MiniProject.SearchUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .search-container {
            width: 100%;
            max-width: 480px;
            margin: 0 auto;
        }

        /* Card Container */
        .card {
            background: #ffffff;
            border-radius: 16px;
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
            background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
            padding: 28px 32px;
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
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.05em;
            text-transform: uppercase;
            margin-bottom: 12px;
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .header-title {
            font-size: 24px;
            font-weight: 700;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .header-subtitle {
            font-size: 14px;
            color: #bfdbfe;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
            margin-bottom: 20px;
        }

        .form-label {
            font-size: 13px;
            font-weight: 600;
            color: #334155;
            display: flex;
            align-items: center;
            gap: 4px;
        }

        .req-star {
            color: #ef4444;
            font-weight: 700;
        }

        .input-wrapper {
            position: relative;
            display: flex;
            align-items: center;
        }

        .input-icon {
            position: absolute;
            left: 12px;
            color: #94a3b8;
            font-size: 15px;
            pointer-events: none;
        }

        .form-input {
            width: 100%;
            height: 44px;
            padding: 10px 14px 10px 38px;
            font-size: 14px;
            font-family: inherit;
            color: #0f172a;
            background: #f8fafc;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            outline: none;
            transition: all 0.2s ease;
        }

        .form-input:focus {
            background: #ffffff;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .form-input::placeholder {
            color: #94a3b8;
            font-size: 13px;
        }

        /* Primary Search Button */
        .btn-submit {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(37, 99, 235, 0.35);
            transition: all 0.2s ease;
            margin-top: 4px;
        }

        .btn-submit:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 20px 0 rgba(37, 99, 235, 0.45);
            transform: translateY(-1px);
        }

        .btn-submit:active {
            transform: translateY(0);
        }

        /* Divider */
        .divider {
            position: relative;
            text-align: center;
            margin: 28px 0 20px;
        }

        .divider::before {
            content: "";
            position: absolute;
            top: 50%;
            left: 0;
            right: 0;
            height: 1px;
            background: #e2e8f0;
        }

        .divider-text {
            position: relative;
            background: #ffffff;
            padding: 0 12px;
            font-size: 12px;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            font-weight: 600;
        }

        /* Secondary Bottom Navigation Button */
        .btn-secondary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 44px;
            background: #f8fafc;
            color: #334155;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-secondary:hover {
            background: #f1f5f9;
            color: #0f172a;
            border-color: #94a3b8;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.06);
        }

        /* Card Footer */
        .card-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 14px 32px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13px;
        }

        .nav-link {
            color: #2563eb;
            text-decoration: none;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #1d4ed8;
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="search-container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">User Query System</div>
                <h1 class="header-title">&#128269; Search User</h1>
                <p class="header-subtitle">Enter user credentials to view their profile details.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Username Field -->
                <div class="form-group">
                    <asp:Label ID="Label1" runat="server" Text="User Name" AssociatedControlID="TxtUser" CssClass="form-label">
                        User Name <span class="req-star">*</span>
                    </asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#128100;</span>
                        <asp:TextBox ID="TxtUser" runat="server" CssClass="form-input" placeholder="e.g. John Doe" autocomplete="off"></asp:TextBox>
                    </div>
                </div>

                <!-- Email Field -->
                <div class="form-group">
                    <asp:Label ID="Label2" runat="server" Text="Email Address" AssociatedControlID="TxtEmail" CssClass="form-label">
                        Email Address <span class="req-star">*</span>
                    </asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#9993;</span>
                        <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-input" TextMode="Email" placeholder="e.g. john@example.com" autocomplete="off"></asp:TextBox>
                    </div>
                </div>

                <!-- Primary Search Button -->
                <asp:Button ID="BtnSearch" runat="server" OnClick="BtnSearch_Click" Text="&#128269; Search User Details" CssClass="btn-submit" />

                <!-- Bottom Section: Navigation to SignUp -->
                <div class="divider">
                    <span class="divider-text">Quick Navigation</span>
                </div>

                <!-- Button at bottom to go to SignUp.aspx -->
                <asp:Button ID="BtnGoToSignUp" runat="server" OnClick="BtnGoToSignUp_Click" Text="&larr; Go to SignUp Page" CssClass="btn-secondary" />
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/FirstPage") %>" class="nav-link">&larr; Sign In Page</a>
                <a href="<%= ResolveUrl("~/ProductForm") %>" class="nav-link">&#128230; Product Form &rarr;</a>
            </div>
        </div>
    </div>
</asp:Content>
