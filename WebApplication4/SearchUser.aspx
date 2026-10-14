<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchUser.aspx.cs" Inherits="MiniProject.SearchUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Search User - User Directory</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            color: #1e293b;
        }

        .container {
            width: 100%;
            max-width: 520px;
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
            background: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%);
            padding: 28px 32px;
            color: #ffffff;
            position: relative;
        }

        .header-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.18);
            backdrop-filter: blur(8px);
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.04em;
            text-transform: uppercase;
            margin-bottom: 12px;
            border: 1px solid rgba(255, 255, 255, 0.25);
        }

        .header-title {
            font-size: 24px;
            font-weight: 700;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .header-subtitle {
            font-size: 14px;
            color: #c7d2fe;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* Form Group */
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
            border-color: #4f46e5;
            box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.15);
        }

        .form-input::placeholder {
            color: #94a3b8;
            font-size: 13px;
        }

        /* Search Button */
        .btn-search {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px rgba(79, 70, 229, 0.35);
            transition: all 0.2s ease;
            margin-top: 4px;
        }

        .btn-search:hover {
            background: linear-gradient(135deg, #4338ca 0%, #2563eb 100%);
            box-shadow: 0 6px 18px rgba(79, 70, 229, 0.45);
            transform: translateY(-1px);
        }

        .btn-search:active {
            transform: translateY(0);
        }

        /* Alert Box */
        .alert-box {
            display: block;
            margin-top: 18px;
            padding: 14px 16px;
            border-radius: 8px;
            font-size: 13px;
            line-height: 1.5;
            border: 1px solid transparent;
            animation: fadeIn 0.3s ease;
        }

        .alert-warning {
            background-color: #fffbeb;
            border-color: #fde68a;
            color: #92400e;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Divider */
        .card-divider {
            height: 1px;
            background: #e2e8f0;
            margin: 24px 0 20px 0;
            position: relative;
            text-align: center;
        }

        .card-divider span {
            position: relative;
            top: -10px;
            background: #ffffff;
            padding: 0 12px;
            font-size: 12px;
            color: #94a3b8;
            font-weight: 500;
        }

        /* Bottom Section with Button to go SignUp.aspx */
        .bottom-action-section {
            display: flex;
            flex-direction: column;
            gap: 10px;
            text-align: center;
        }

        .btn-signup {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 44px;
            background: #f1f5f9;
            color: #334155;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-signup:hover {
            background: #e2e8f0;
            border-color: #94a3b8;
            color: #0f172a;
            transform: translateY(-1px);
        }

        .btn-signup:active {
            transform: translateY(0);
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
            color: #64748b;
        }

        .footer-link {
            color: #4f46e5;
            text-decoration: none;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .footer-link:hover {
            color: #3730a3;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">User Directory</div>
                <h1 class="header-title">&#128269; Search User</h1>
                <p class="header-subtitle">Find user records and view details via QueryString parameters.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- User Name Input -->
                <div class="form-group">
                    <asp:Label ID="Label1" runat="server" CssClass="form-label" AssociatedControlID="TxtUser" Text="User Name"></asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#128100;</span>
                        <asp:TextBox ID="TxtUser" runat="server" CssClass="form-input" placeholder="e.g. JohnDoe" autocomplete="off"></asp:TextBox>
                    </div>
                </div>

                <!-- Email Input -->
                <div class="form-group">
                    <asp:Label ID="Label2" runat="server" CssClass="form-label" AssociatedControlID="TxtEmail" Text="Email Address"></asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#9993;</span>
                        <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-input" placeholder="e.g. john@example.com" TextMode="Email" autocomplete="off"></asp:TextBox>
                    </div>
                </div>

                <!-- Primary Search Button -->
                <asp:Button ID="BtnSearch" runat="server" OnClick="BtnSearch_Click" Text="&#128269; Search User" CssClass="btn-search" />

                <!-- Validation/Feedback Message -->
                <asp:Label ID="LblMsg" runat="server" CssClass="alert-box" Visible="false"></asp:Label>

                <!-- Divider -->
                <div class="card-divider">
                    <span>OR</span>
                </div>

                <!-- Bottom Button to go SignUp.aspx -->
                <div class="bottom-action-section">
                    <asp:Button ID="BtnGoSignUp" runat="server" OnClick="BtnGoSignUp_Click" Text="&#128203; Go to SignUp Page" CssClass="btn-signup" />
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <span>Directory Query</span>
                <a href="SignUp.aspx" class="footer-link">&larr; Back to Dashboard</a>
            </div>
        </div>
    </form>
</body>
</html>
