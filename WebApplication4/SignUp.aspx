<%@ Page Title="User Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="WebApplication4.SignUp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .dashboard-container {
            max-width: 1040px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 28px;
        }

        /* Profile Header Banner */
        .profile-banner {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 20px;
            padding: 32px;
            color: #ffffff;
            border: 1px solid rgba(255, 255, 255, 0.1);
            box-shadow: 0 10px 30px -5px rgba(15, 23, 42, 0.25);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 24px;
            position: relative;
            overflow: hidden;
        }

        .profile-banner::after {
            content: "";
            position: absolute;
            right: -40px;
            top: -40px;
            width: 220px;
            height: 220px;
            background: radial-gradient(circle, rgba(59, 130, 246, 0.25) 0%, transparent 70%);
            pointer-events: none;
        }

        .user-intro {
            display: flex;
            align-items: center;
            gap: 20px;
        }

        .user-avatar-large {
            width: 72px;
            height: 72px;
            border-radius: 50%;
            background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
            color: #ffffff;
            font-size: 32px;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 4px 14px rgba(37, 99, 235, 0.4);
            border: 3px solid rgba(255, 255, 255, 0.2);
            flex-shrink: 0;
        }

        .user-meta {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .welcome-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.25);
            border: 1px solid rgba(96, 165, 250, 0.4);
            color: #93c5fd;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            width: fit-content;
        }

        .user-heading {
            font-size: 24px;
            font-weight: 800;
            letter-spacing: -0.02em;
            color: #ffffff;
            line-height: 1.2;
        }

        .user-email-display {
            font-size: 14px;
            color: #94a3b8;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        /* Banner Status Items */
        .banner-stats {
            display: flex;
            align-items: center;
            gap: 16px;
            flex-wrap: wrap;
        }

        .stat-badge {
            background: rgba(255, 255, 255, 0.08);
            border: 1px solid rgba(255, 255, 255, 0.12);
            padding: 12px 18px;
            border-radius: 12px;
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .stat-badge-label {
            font-size: 11px;
            text-transform: uppercase;
            color: #94a3b8;
            font-weight: 600;
            letter-spacing: 0.05em;
        }

        .stat-badge-value {
            font-size: 15px;
            font-weight: 700;
            color: #ffffff;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .online-dot-pulse {
            width: 8px;
            height: 8px;
            background-color: #10b981;
            border-radius: 50%;
            box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
            animation: pulse-ring 2s infinite;
        }

        @keyframes pulse-ring {
            0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
            70% { box-shadow: 0 0 0 6px rgba(16, 185, 129, 0); }
            100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
        }

        /* Credentials Details Card */
        .details-card {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.04);
            padding: 24px 28px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 20px;
        }

        .credentials-grid {
            display: flex;
            gap: 36px;
            flex-wrap: wrap;
        }

        .cred-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .cred-label {
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 0.06em;
            color: #64748b;
            font-weight: 700;
        }

        .cred-val {
            font-size: 16px;
            font-weight: 700;
            color: #0f172a;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .cred-val-masked {
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
            background: #f1f5f9;
            padding: 3px 10px;
            border-radius: 6px;
            border: 1px solid #cbd5e1;
            color: #334155;
            letter-spacing: 0.08em;
            font-size: 14px;
        }

        .cred-actions {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btn-primary-action {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            height: 42px;
            padding: 0 20px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
            transition: all 0.2s ease;
        }

        .btn-primary-action:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 16px rgba(37, 99, 235, 0.4);
            transform: translateY(-1px);
        }

        .btn-signout {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            height: 42px;
            padding: 0 16px;
            background: #f1f5f9;
            color: #475569;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-signout:hover {
            background: #fee2e2;
            color: #dc2626;
            border-color: #fca5a5;
        }

        /* Section Title Styling */
        .section-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 16px;
        }

        .section-title {
            font-size: 18px;
            font-weight: 700;
            color: #0f172a;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .section-sub {
            font-size: 13px;
            color: #64748b;
        }

        /* Media Gallery Grid */
        .gallery-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 22px;
        }

        .gallery-card {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 14px rgba(0, 0, 0, 0.04);
            overflow: hidden;
            transition: all 0.25s ease;
            display: flex;
            flex-direction: column;
        }

        .gallery-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 12px 28px -4px rgba(0, 0, 0, 0.1);
            border-color: #cbd5e1;
        }

        .gallery-image-wrapper {
            width: 100%;
            height: 280px;
            background: #f1f5f9;
            overflow: hidden;
            position: relative;
        }

        .gallery-image-wrapper img, 
        .gallery-image-wrapper input {
            width: 100% !important;
            height: 100% !important;
            object-fit: cover;
            border: none;
            display: block;
            transition: transform 0.3s ease;
        }

        .gallery-card:hover img, 
        .gallery-card:hover input {
            transform: scale(1.03);
        }

        .gallery-info {
            padding: 16px 20px;
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .gallery-tag {
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            color: #2563eb;
        }

        .gallery-name {
            font-size: 15px;
            font-weight: 700;
            color: #0f172a;
        }

        /* Dashboard Quick Actions Bar */
        .quick-actions-panel {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.04);
            padding: 24px;
        }

        .actions-tile-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 14px;
            margin-top: 14px;
        }

        /* Action Buttons */
        .action-tile-btn {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            height: 48px;
            border-radius: 10px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            border: none;
            transition: all 0.2s ease;
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.06);
            width: 100%;
        }

        .btn-counting-tile {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            color: #ffffff;
        }
        .btn-counting-tile:hover {
            background: linear-gradient(135deg, #047857 0%, #065f46 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 14px rgba(5, 150, 105, 0.35);
        }

        .btn-search-tile {
            background: linear-gradient(135deg, #4f46e5 0%, #4338ca 100%);
            color: #ffffff;
        }
        .btn-search-tile:hover {
            background: linear-gradient(135deg, #4338ca 0%, #3730a3 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 14px rgba(79, 70, 229, 0.35);
        }

        .btn-cookies-tile {
            background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
            color: #ffffff;
        }
        .btn-cookies-tile:hover {
            background: linear-gradient(135deg, #b45309 0%, #92400e 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 14px rgba(217, 119, 6, 0.35);
        }

        .btn-check-tile {
            background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
            color: #ffffff;
        }
        .btn-check-tile:hover {
            background: linear-gradient(135deg, #0f766e 0%, #115e59 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 14px rgba(13, 148, 136, 0.35);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-container">
        
        <!-- 1. Top Profile Hero Banner -->
        <div class="profile-banner">
            <div class="user-intro">
                <div class="user-avatar-large">
                    &#128100;
                </div>
                <div class="user-meta">
                    <div class="welcome-pill">&#9679; Authenticated Session</div>
                    <h1 class="user-heading">Welcome Back, <asp:Label ID="outUserName" runat="server" Text="User"></asp:Label>!</h1>
                    <span class="user-email-display">&#9993; Active logged-in user profile</span>
                </div>
            </div>

            <!-- Live Stats in Banner -->
            <div class="banner-stats">
                <div class="stat-badge">
                    <span class="stat-badge-label">Live Active Visitors</span>
                    <span class="stat-badge-value">
                        <span class="online-dot-pulse"></span>
                        <asp:Label ID="lblOnlineUsers" runat="server" Text="0"></asp:Label> Users Online
                    </span>
                </div>
                <div class="stat-badge">
                    <span class="stat-badge-label">Session State</span>
                    <span class="stat-badge-value" style="color: #34d399;">
                        &#10004; Active
                    </span>
                </div>
            </div>
        </div>

        <!-- 2. Account Credentials & Quick Actions Card -->
        <div class="details-card">
            <div class="credentials-grid">
                <div class="cred-item">
                    <span class="cred-label">Login Identifier</span>
                    <span class="cred-val">&#128100; <%= Session["UserName"] != null ? Session["UserName"].ToString() : "Guest" %></span>
                </div>
                <div class="cred-item">
                    <span class="cred-label">Current Password</span>
                    <span class="cred-val">
                        &#128274; <asp:Label ID="outPassword" runat="server" CssClass="cred-val-masked" Text="••••••••"></asp:Label>
                    </span>
                </div>
                <div class="cred-item">
                    <span class="cred-label">Access Level</span>
                    <span class="cred-val" style="color: #2563eb;">&#9733; Verified Member</span>
                </div>
            </div>

            <div class="cred-actions">
                <asp:Button ID="BtnProductForm" runat="server" Text="&#128230; Open Product Form" OnClick="BtnProductForm_Click" CssClass="btn-primary-action" />
                <a href="FirstPage.aspx" class="btn-signout">&#128682; Sign Out</a>
            </div>
        </div>

        <!-- 3. Media Gallery Showcase -->
        <div>
            <div class="section-header">
                <div>
                    <h2 class="section-title">&#128444; Personal Media Gallery</h2>
                    <p class="section-sub">High-resolution catalog assets linked to your dashboard</p>
                </div>
            </div>

            <div class="gallery-grid">
                <!-- Gallery Item 1 -->
                <div class="gallery-card">
                    <div class="gallery-image-wrapper">
                        <asp:Image ID="Image1" runat="server" ImageUrl="https://images.unsplash.com/photo-1791608047988-9c48157584ad?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" AlternateText="Asset 1" />
                    </div>
                    <div class="gallery-info">
                        <span class="gallery-tag">Catalog Photo 1</span>
                        <span class="gallery-name">Modern Architecture Asset</span>
                    </div>
                </div>

                <!-- Gallery Item 2 -->
                <div class="gallery-card">
                    <div class="gallery-image-wrapper">
                        <asp:ImageMap ID="ImageMap1" runat="server" ImageUrl="https://images.unsplash.com/photo-1791724389897-0ece687f2b7a?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D">
                        </asp:ImageMap>
                    </div>
                    <div class="gallery-info">
                        <span class="gallery-tag">Interactive ImageMap</span>
                        <span class="gallery-name">Geometric Urban Landscape</span>
                    </div>
                </div>

                <!-- Gallery Item 3 -->
                <div class="gallery-card">
                    <div class="gallery-image-wrapper">
                        <asp:Image ID="Image2" runat="server" ImageUrl="https://images.unsplash.com/photo-1791724389693-b873064e086d?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" AlternateText="Asset 3" />
                    </div>
                    <div class="gallery-info">
                        <span class="gallery-tag">Catalog Photo 2</span>
                        <span class="gallery-name">Architectural Perspective</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. Quick Navigation Dock -->
        <div class="quick-actions-panel">
            <div class="section-header" style="margin-bottom: 0;">
                <div>
                    <h2 class="section-title">&#9889; User Action Shortcuts</h2>
                    <p class="section-sub">Direct access to application utilities from your dashboard</p>
                </div>
            </div>

            <div class="actions-tile-grid">
                <asp:Button ID="BtnCounting" runat="server" Text="&#128290; Click Counter Page" OnClick="BtnCounting_Click" CssClass="action-tile-btn btn-counting-tile" />
                <asp:Button ID="BtnSearchUser" runat="server" Text="&#128269; Search User Directory" OnClick="BtnSearchUser_Click" CssClass="action-tile-btn btn-search-tile" />
                <asp:Button ID="BtnCookiesSend" runat="server" Text="&#127850; Manage HTTP Cookies" OnClick="BtnCookiesSend_Click" CssClass="action-tile-btn btn-cookies-tile" />
                <asp:Button ID="BtnCheckAvailability" runat="server" Text="&#9989; Check Availability" OnClick="BtnCheckAvailability_Click" CssClass="action-tile-btn btn-check-tile" />
            </div>
        </div>

    </div>
</asp:Content>
