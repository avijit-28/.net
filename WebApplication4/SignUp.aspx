<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="WebApplication4.SignUp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Dashboard</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: #f8fafc;
            color: #1e293b;
            min-height: 100vh;
            padding: 40px 20px;
        }

        .container {
            max-width: 900px;
            margin: 0 auto;
        }

        /* Header Bar */
        .top-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
            margin-bottom: 24px;
        }

        h1 {
            font-size: 24px;
            font-weight: 700;
            color: #0f172a;
        }

        /* Online Users Badge Design */
        .online-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            background: #ecfdf5;
            border: 1px solid #a7f3d0;
            padding: 8px 18px;
            border-radius: 9999px;
            font-size: 14px;
            font-weight: 500;
            color: #065f46;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
        }

        .status-dot {
            width: 10px;
            height: 10px;
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
                box-shadow: 0 0 0 8px rgba(16, 185, 129, 0);
            }
            100% {
                box-shadow: 0 0 0 0 rgba(16, 185, 129, 0);
            }
        }

        .online-count {
            font-weight: 700;
            font-size: 16px;
            color: #047857;
        }

        /* Profile Card */
        .profile-card {
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
            padding: 24px;
            margin-bottom: 28px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .info-row {
            display: flex;
            gap: 32px;
            flex-wrap: wrap;
        }

        .info-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .info-label {
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            color: #64748b;
            font-weight: 600;
        }

        .info-value {
            font-size: 16px;
            font-weight: 600;
            color: #0f172a;
        }

        .action-group {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btn-product {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 9px 18px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.25);
            transition: all 0.2s ease;
        }

        .btn-product:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 16px rgba(37, 99, 235, 0.35);
            transform: translateY(-1px);
        }

        .btn-product:active {
            transform: translateY(0);
        }

        .back-link {
            display: inline-flex;
            align-items: center;
            padding: 8px 16px;
            background: #f1f5f9;
            color: #334155;
            text-decoration: none;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 500;
            transition: background 0.2s;
        }

        .back-link:hover {
            background: #e2e8f0;
        }

        /* Gallery Grid */
        .section-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 16px;
            color: #334155;
        }

        .gallery {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
        }

        .gallery img, .gallery input {
            border-radius: 12px;
            object-fit: cover;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .gallery img:hover, .gallery input:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

            <!-- Top Bar with Header & Live Online Users Status -->
            <div class="top-bar">
                <h1>User Dashboard</h1>

                <!-- Online Users Counter Badge -->
                <div class="online-badge">
                    <span class="status-dot"></span>
                    <span>The num of users online = </span>
                    <asp:Label ID="lblOnlineUsers" runat="server" CssClass="online-count" Text="0"></asp:Label>
                </div>
            </div>

            <!-- Profile Info Card -->
            <div class="profile-card">
                <div class="info-row">
                    <div class="info-item">
                        <span class="info-label">Signed-in User</span>
                        <asp:Label ID="outUserName" runat="server" CssClass="info-value" Text="Label"></asp:Label>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Password</span>
                        <asp:Label ID="outPassword" runat="server" CssClass="info-value" Text="Label"></asp:Label>
                    </div>
                </div>
                <a href="FirstPage.aspx" class="back-link">&nbsp;Back to Sign In</a>
            </div>

            <!-- Image Gallery -->
            <div class="section-title">Gallery</div>
            <div class="gallery">
                <asp:Image ID="Image1" runat="server" Height="360px" ImageUrl="https://images.unsplash.com/photo-1791608047988-9c48157584ad?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" Width="260px" />
                <asp:ImageMap ID="ImageMap1" runat="server" Height="360px" Width="260px" ImageUrl="https://images.unsplash.com/photo-1791724389897-0ece687f2b7a?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D">
                </asp:ImageMap>
                <asp:Image ID="Image2" runat="server" Height="360px" ImageUrl="https://images.unsplash.com/photo-1791724389693-b873064e086d?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" Width="260px" />
            </div>

        </div>
    </form>
</body>
</html>
