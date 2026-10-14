<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetUser.aspx.cs" Inherits="MiniProject.GetUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>User Details - Profile Viewer</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #f0f9ff 0%, #e2e8f0 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            color: #1e293b;
        }

        .container {
            width: 100%;
            max-width: 540px;
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
            background: linear-gradient(135deg, #0284c7 0%, #2563eb 100%);
            padding: 28px 32px;
            color: #ffffff;
            position: relative;
        }

        .header-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(8px);
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.04em;
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
            color: #bae6fd;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* Profile Banner / Avatar */
        .profile-banner {
            display: flex;
            align-items: center;
            gap: 16px;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 16px 20px;
            margin-bottom: 24px;
        }

        .avatar-circle {
            width: 52px;
            height: 52px;
            background: linear-gradient(135deg, #0284c7 0%, #2563eb 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #ffffff;
            font-size: 24px;
            flex-shrink: 0;
            box-shadow: 0 4px 10px rgba(2, 132, 199, 0.3);
        }

        .profile-meta h3 {
            font-size: 16px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 4px;
        }

        .status-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 12px;
            color: #059669;
            font-weight: 600;
        }

        .status-dot {
            width: 8px;
            height: 8px;
            background: #10b981;
            border-radius: 50%;
        }

        /* Info Item Cards */
        .info-list {
            display: flex;
            flex-direction: column;
            gap: 14px;
            margin-bottom: 24px;
        }

        .info-card {
            background: #ffffff;
            border: 1.5px solid #e2e8f0;
            border-radius: 10px;
            padding: 14px 18px;
            display: flex;
            align-items: center;
            gap: 14px;
            transition: all 0.2s ease;
        }

        .info-card:hover {
            border-color: #0284c7;
            background: #f0f9ff;
        }

        .info-icon {
            font-size: 20px;
            color: #0284c7;
            width: 32px;
            text-align: center;
        }

        .info-details {
            display: flex;
            flex-direction: column;
            gap: 2px;
            flex-grow: 1;
        }

        .info-caption {
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            color: #64748b;
        }

        .info-val {
            font-size: 15px;
            font-weight: 600;
            color: #0f172a;
            word-break: break-all;
        }

        /* Action Buttons */
        .actions-group {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .btn-search-again {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 46px;
            background: linear-gradient(135deg, #0284c7 0%, #2563eb 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px rgba(2, 132, 199, 0.3);
            transition: all 0.2s ease;
        }

        .btn-search-again:hover {
            background: linear-gradient(135deg, #0369a1 0%, #1d4ed8 100%);
            box-shadow: 0 6px 18px rgba(2, 132, 199, 0.4);
            transform: translateY(-1px);
        }

        .btn-to-signup {
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

        .btn-to-signup:hover {
            background: #e2e8f0;
            border-color: #94a3b8;
            color: #0f172a;
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
            color: #0284c7;
            text-decoration: none;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .footer-link:hover {
            color: #0369a1;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">User Profile</div>
                <h1 class="header-title">&#128100; User Details</h1>
                <p class="header-subtitle">Account information retrieved from query string parameters.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Profile Avatar Banner -->
                <div class="profile-banner">
                    <div class="avatar-circle">&#128100;</div>
                    <div class="profile-meta">
                        <h3>Queried Account</h3>
                        <span class="status-pill">
                            <span class="status-dot"></span>
                            Record Available
                        </span>
                    </div>
                </div>

                <!-- Info Cards -->
                <div class="info-list">
                    <!-- Name Card -->
                    <div class="info-card">
                        <span class="info-icon">&#128100;</span>
                        <div class="info-details">
                            <span class="info-caption">User Name</span>
                            <asp:Label ID="LblName" runat="server" CssClass="info-val" Text="User Name"></asp:Label>
                        </div>
                    </div>

                    <!-- Email Card -->
                    <div class="info-card">
                        <span class="info-icon">&#9993;</span>
                        <div class="info-details">
                            <span class="info-caption">Email Address</span>
                            <asp:Label ID="LblEmail" runat="server" CssClass="info-val" Text="Email Address"></asp:Label>
                        </div>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="actions-group">
                    <asp:Button ID="BtnBackSearch" runat="server" OnClick="BtnBackSearch_Click" Text="&#128269; Search Another User" CssClass="btn-search-again" />
                    <asp:Button ID="BtnGoSignUp" runat="server" OnClick="BtnGoSignUp_Click" Text="&#128203; Go to SignUp Dashboard" CssClass="btn-to-signup" />
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <span>Queried via GET Request</span>
                <a href="SignUp.aspx" class="footer-link">&larr; Back to Dashboard</a>
            </div>
        </div>
    </form>
</body>
</html>
