<%@ Page Title="Home Portal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication4.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        /* Hero Banner */
        .hero {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 18px;
            padding: 44px 36px;
            color: #ffffff;
            margin-bottom: 36px;
            position: relative;
            overflow: hidden;
            border: 1px solid rgba(255, 255, 255, 0.1);
            box-shadow: 0 10px 30px -5px rgba(15, 23, 42, 0.2);
        }

        .hero::after {
            content: "";
            position: absolute;
            right: -60px;
            bottom: -60px;
            width: 260px;
            height: 260px;
            background: radial-gradient(circle, rgba(37, 99, 235, 0.35) 0%, transparent 70%);
            pointer-events: none;
        }

        .hero-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.3);
            border: 1px solid rgba(96, 165, 250, 0.4);
            color: #bfdbfe;
            padding: 5px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 14px;
        }

        .hero-title {
            font-size: 32px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 10px;
            line-height: 1.2;
        }

        .hero-desc {
            font-size: 16px;
            color: #94a3b8;
            max-width: 680px;
            line-height: 1.6;
            margin-bottom: 24px;
        }

        .hero-actions {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btn-hero-primary {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 24px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14.5px;
            transition: all 0.2s ease;
            box-shadow: 0 4px 14px rgba(37, 99, 235, 0.4);
        }

        .btn-hero-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 18px rgba(37, 99, 235, 0.5);
        }

        .btn-hero-secondary {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 22px;
            background: rgba(255, 255, 255, 0.08);
            border: 1px solid rgba(255, 255, 255, 0.15);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14.5px;
            transition: all 0.2s ease;
        }

        .btn-hero-secondary:hover {
            background: rgba(255, 255, 255, 0.15);
        }

        /* Section Title */
        .section-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 20px;
        }

        .section-title {
            font-size: 20px;
            font-weight: 700;
            color: #0f172a;
        }

        .section-subtitle {
            font-size: 13.5px;
            color: #64748b;
        }

        /* Module Cards Grid */
        .module-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 22px;
            margin-bottom: 36px;
        }

        .module-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            padding: 24px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.03);
            transition: all 0.25s ease;
        }

        .module-card:hover {
            transform: translateY(-3px);
            border-color: #cbd5e1;
            box-shadow: 0 12px 24px -4px rgba(0, 0, 0, 0.08);
        }

        .module-icon {
            width: 44px;
            height: 44px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            margin-bottom: 16px;
        }

        .icon-blue   { background: #eff6ff; color: #2563eb; }
        .icon-teal   { background: #f0fdfa; color: #0d9488; }
        .icon-amber  { background: #fef3c7; color: #d97706; }
        .icon-emerald{ background: #ecfdf5; color: #059669; }
        .icon-violet { background: #f5f3ff; color: #7c3aed; }
        .icon-rose   { background: #fff1f2; color: #e11d48; }

        .module-name {
            font-size: 16.5px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 6px;
        }

        .module-desc {
            font-size: 13.5px;
            color: #64748b;
            line-height: 1.5;
            margin-bottom: 20px;
            flex-grow: 1;
        }

        .module-link {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            color: #2563eb;
            text-decoration: none;
            font-weight: 600;
            font-size: 13.5px;
            transition: gap 0.2s;
        }

        .module-link:hover {
            gap: 10px;
            text-decoration: underline;
        }

        /* Diagnostic Quick Info Panel */
        .stats-panel {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            padding: 24px 28px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
        }

        .stat-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .stat-label {
            font-size: 12px;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.05em;
        }

        .stat-val {
            font-size: 18px;
            font-weight: 700;
            color: #0f172a;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hero Section -->
    <div class="hero">
        <div class="hero-badge">&#127760; ASP.NET WebForms Master Layout</div>
        <h1 class="hero-title">WebApplication4 Portal</h1>
        <p class="hero-desc">
            A comprehensive ASP.NET application showcasing master pages, state management, asynchronous AJAX controls, client cookies, database operations, and SEO routing.
        </p>
        <div class="hero-actions">
            <a href="SignUp.aspx" class="btn-hero-primary">&#128100; Go to User Dashboard</a>
            <a href="ProductForm.aspx" class="btn-hero-secondary">&#128230; Manage Inventory</a>
        </div>
    </div>

    <!-- Section Header -->
    <div class="section-header">
        <div>
            <h2 class="section-title">Application Modules</h2>
            <p class="section-subtitle">Quick access to all interactive pages and demonstrations</p>
        </div>
    </div>

    <!-- Modules Cards Grid -->
    <div class="module-grid">
        <!-- 1. Product Form -->
        <div class="module-card">
            <div class="module-icon icon-blue">&#128230;</div>
            <div class="module-name">Product Management</div>
            <p class="module-desc">Add, validate, and persist catalog inventory products with formatted prices and category selection.</p>
            <a href="ProductForm.aspx" class="module-link">Open Product Form &rarr;</a>
        </div>

        <!-- 2. Check Availability -->
        <div class="module-card">
            <div class="module-icon icon-teal">&#128737;</div>
            <div class="module-name">Username Availability</div>
            <p class="module-desc">Real-time asynchronous checking against the database using ASP.NET AJAX UpdatePanel and UpdateProgress.</p>
            <a href="CheckAvailability.aspx" class="module-link">Test Availability &rarr;</a>
        </div>

        <!-- 3. Search User -->
        <div class="module-card">
            <div class="module-icon icon-violet">&#128269;</div>
            <div class="module-name">Search User Directory</div>
            <p class="module-desc">Search users and safely transfer parameters via URL query strings to the recipient GetUser detail page.</p>
            <a href="SearchUser.aspx" class="module-link">Search Directory &rarr;</a>
        </div>

        <!-- 4. Cookie State -->
        <div class="module-card">
            <div class="module-icon icon-amber">&#127850;</div>
            <div class="module-name">HTTP Cookie Management</div>
            <p class="module-desc">Demonstrates client-side multi-value cookies, expiration lifecycles, and reading cookie values from requests.</p>
            <a href="CookiesSend.aspx" class="module-link">Manage Cookies &rarr;</a>
        </div>

        <!-- 5. URL Routing -->
        <div class="module-card">
            <div class="module-icon icon-emerald">&#128279;</div>
            <div class="module-name">SEO URL Routing</div>
            <p class="module-desc">Demonstrates ASP.NET MapPageRoute with dynamic slug generation and parameterized detail views.</p>
            <a href="URLRouting.aspx" class="module-link">View Articles &rarr;</a>
        </div>

        <!-- 6. Counting & Metrics -->
        <div class="module-card">
            <div class="module-icon icon-rose">&#128290;</div>
            <div class="module-name">Live Counting &amp; Sessions</div>
            <p class="module-desc">Tracks application-wide hit counters and active session state across all connected browser clients.</p>
            <a href="Counting.aspx" class="module-link">View Counters &rarr;</a>
        </div>
    </div>

    <!-- Diagnostic Quick Info Panel -->
    <div class="stats-panel">
        <div class="stat-item">
            <span class="stat-label">Application Status</span>
            <span class="stat-val" style="color: #059669;">&#9679; Online &amp; Healthy</span>
        </div>
        <div class="stat-item">
            <span class="stat-label">Current Session User</span>
            <span class="stat-val"><%= Session["UserName"] ?? "Guest Visitor" %></span>
        </div>
        <div class="stat-item">
            <span class="stat-label">Target Runtime</span>
            <span class="stat-val">.NET Framework 4.8.1</span>
        </div>
        <div class="stat-item">
            <span class="stat-label">System Time</span>
            <span class="stat-val"><%= DateTime.Now.ToString("hh:mm tt") %></span>
        </div>
    </div>
</asp:Content>
