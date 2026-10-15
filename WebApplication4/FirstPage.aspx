<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstPage.aspx.cs" Inherits="WebApplication4.FirstPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Sign In - WebApplication4 Portal</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #334155 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 16px;
            color: #1e293b;
        }

        .auth-container {
            width: 100%;
            max-width: 440px;
        }

        .auth-card {
            background: #ffffff;
            border-radius: 20px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(255, 255, 255, 0.1);
            overflow: hidden;
            transition: transform 0.3s ease;
        }

        /* Card Header */
        .card-header {
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            padding: 32px 28px;
            color: #ffffff;
            text-align: center;
            position: relative;
        }

        .header-icon {
            width: 52px;
            height: 52px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(8px);
            border: 1px solid rgba(255, 255, 255, 0.3);
            border-radius: 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 26px;
            margin-bottom: 12px;
        }

        .header-title {
            font-size: 24px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 4px;
        }

        .header-subtitle {
            font-size: 13.5px;
            color: #bfdbfe;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px 28px;
        }

        /* Online Users Badge */
        .online-badge {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            background: #ecfdf5;
            border: 1px solid #a7f3d0;
            padding: 8px 14px;
            border-radius: 20px;
            margin-bottom: 24px;
            font-size: 13px;
            color: #065f46;
            font-weight: 500;
        }

        .status-dot {
            width: 8px;
            height: 8px;
            background-color: #10b981;
            border-radius: 50%;
            box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
            animation: pulse-ring 2s infinite cubic-bezier(0.4, 0, 0.6, 1);
        }

        @keyframes pulse-ring {
            0% {
                box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
            }
            70% {
                box-shadow: 0 0 0 6px rgba(16, 185, 129, 0);
            }
            100% {
                box-shadow: 0 0 0 0 rgba(16, 185, 129, 0);
            }
        }

        .online-count {
            font-weight: 700;
            font-size: 14px;
            color: #047857;
        }

        .form-group {
            margin-bottom: 18px;
        }

        .form-label {
            display: flex;
            align-items: center;
            gap: 4px;
            margin-bottom: 6px;
            font-size: 13px;
            font-weight: 600;
            color: #334155;
        }

        .req-star {
            color: #ef4444;
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

        .form-control {
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

        .form-control:focus {
            background: #ffffff;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .val-error {
            display: block;
            font-size: 12px;
            color: #ef4444;
            font-weight: 500;
            margin-top: 4px;
        }

        /* Demo Credentials Helper Pill */
        .demo-credentials {
            background: #f1f5f9;
            border: 1px dashed #cbd5e1;
            border-radius: 8px;
            padding: 10px 14px;
            font-size: 12px;
            color: #475569;
            margin-bottom: 20px;
            line-height: 1.5;
        }

        .demo-credentials code {
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
            background: #ffffff;
            padding: 2px 6px;
            border-radius: 4px;
            border: 1px solid #e2e8f0;
            color: #2563eb;
            font-weight: 600;
        }

        /* Submit Button */
        .btn-submit {
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(37, 99, 235, 0.35);
            transition: all 0.2s ease;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }

        .btn-submit:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 20px 0 rgba(37, 99, 235, 0.45);
            transform: translateY(-1px);
        }

        .btn-submit:active {
            transform: translateY(0);
        }

        /* Card Footer */
        .card-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 16px 28px;
            text-align: center;
            font-size: 13px;
        }

        .home-link {
            color: #2563eb;
            text-decoration: none;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            transition: color 0.2s;
        }

        .home-link:hover {
            color: #1d4ed8;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <div class="auth-container">
        <form id="form1" runat="server" class="auth-card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-icon">&#128272;</div>
                <h1 class="header-title">Welcome Back</h1>
                <p class="header-subtitle">Sign in to access your dashboard &amp; portal features</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Online Users Counter Badge -->
                <div class="online-badge">
                    <span class="status-dot"></span>
                    <span>Active users online: </span>
                    <asp:Label ID="lblOnlineUsers" runat="server" CssClass="online-count" Text="0"></asp:Label>
                </div>

                <!-- Username (Email) Field -->
                <div class="form-group">
                    <asp:Label ID="lblUser" runat="server" AssociatedControlID="txtUserName" CssClass="form-label">
                        Email Address <span class="req-star">*</span>
                    </asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#9993;</span>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="avijit@gmail.com" autocomplete="off"></asp:TextBox>
                    </div>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorEmail" runat="server" 
                        ControlToValidate="txtUserName" 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Please enter a valid email address." 
                        ForeColor="Red"
                        CssClass="val-error"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>
                </div>

                <!-- Password Field -->
                <div class="form-group">
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="form-label">
                        Password <span class="req-star">*</span>
                    </asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#128274;</span>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="********"></asp:TextBox>
                    </div>
                </div>

                <!-- Confirm Password Field -->
                <div class="form-group">
                    <asp:Label ID="lblRePassword" runat="server" AssociatedControlID="txtRePassword" CssClass="form-label">
                        Confirm Password <span class="req-star">*</span>
                    </asp:Label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#128270;</span>
                        <asp:TextBox ID="txtRePassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="********"></asp:TextBox>
                    </div>
                    <asp:CompareValidator ID="CompareValidatorPassword" runat="server"
                        ErrorMessage="Passwords do not match."
                        ControlToValidate="txtRePassword"
                        ControlToCompare="txtPassword"
                        Type="String"
                        Operator="Equal"
                        ForeColor="Red"
                        CssClass="val-error"
                        Display="Dynamic">
                    </asp:CompareValidator>
                </div>

                <!-- Demo Hint Box -->
                <div class="demo-credentials">
                    &#128161; <strong>Demo Login:</strong> Email: <code>avijit@gmail.com</code> | Password: <code>1234</code>
                </div>

                <!-- Submit Button -->
                <asp:Button ID="BtnSign" runat="server" OnClick="BtnSign_Click" Text="&#10148; Sign In to Dashboard" CssClass="btn-submit" />
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/Home") %>" class="home-link">&larr; Back to Home Portal</a>
            </div>
        </form>
    </div>
</body>
</html>
