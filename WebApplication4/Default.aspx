<%@ Page Title="Home Portal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication4.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .home-container {
            max-width: 1100px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 36px;
        }

        /* Hero Banner */
        .hero {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 20px;
            padding: 48px 40px;
            color: #ffffff;
            position: relative;
            overflow: hidden;
            border: 1px solid rgba(255, 255, 255, 0.1);
            box-shadow: 0 14px 35px -5px rgba(15, 23, 42, 0.25);
        }

        .hero::after {
            content: "";
            position: absolute;
            right: -60px;
            bottom: -60px;
            width: 300px;
            height: 300px;
            background: radial-gradient(circle, rgba(37, 99, 235, 0.35) 0%, transparent 70%);
            pointer-events: none;
        }

        .hero-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.25);
            border: 1px solid rgba(96, 165, 250, 0.4);
            color: #bfdbfe;
            padding: 5px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 16px;
        }

        .hero-title {
            font-size: 36px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 12px;
            line-height: 1.2;
        }

        .hero-desc {
            font-size: 16px;
            color: #94a3b8;
            max-width: 720px;
            line-height: 1.6;
            margin-bottom: 28px;
        }

        .hero-actions {
            display: flex;
            gap: 14px;
            flex-wrap: wrap;
        }

        .btn-hero-primary {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 26px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 700;
            font-size: 14.5px;
            transition: all 0.2s ease;
            box-shadow: 0 4px 14px rgba(37, 99, 235, 0.4);
        }

        .btn-hero-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 18px rgba(37, 99, 235, 0.5);
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
        }

        .btn-hero-secondary {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 24px;
            background: rgba(255, 255, 255, 0.08);
            border: 1px solid rgba(255, 255, 255, 0.2);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14.5px;
            transition: all 0.2s ease;
        }

        .btn-hero-secondary:hover {
            background: rgba(255, 255, 255, 0.16);
        }

        /* Section Header */
        .section-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 20px;
        }

        .section-title {
            font-size: 22px;
            font-weight: 800;
            color: #0f172a;
            letter-spacing: -0.01em;
        }

        .section-subtitle {
            font-size: 14px;
            color: #64748b;
        }

        /* Modules Cards Grid */
        .module-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(310px, 1fr));
            gap: 22px;
        }

        .module-card {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            padding: 26px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.03);
            transition: all 0.25s ease;
        }

        .module-card:hover {
            transform: translateY(-4px);
            border-color: #cbd5e1;
            box-shadow: 0 12px 24px -4px rgba(0, 0, 0, 0.08);
        }

        .module-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 16px;
        }

        .module-icon {
            width: 46px;
            height: 46px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
        }

        .icon-blue    { background: #eff6ff; color: #2563eb; }
        .icon-teal    { background: #f0fdfa; color: #0d9488; }
        .icon-amber   { background: #fef3c7; color: #d97706; }
        .icon-emerald { background: #ecfdf5; color: #059669; }
        .icon-violet  { background: #f5f3ff; color: #7c3aed; }
        .icon-rose    { background: #fff1f2; color: #e11d48; }

        .tech-pill {
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            padding: 3px 8px;
            border-radius: 6px;
            background: #f1f5f9;
            color: #475569;
            letter-spacing: 0.04em;
        }

        .module-name {
            font-size: 17px;
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
            font-size: 14px;
            transition: gap 0.2s;
        }

        .module-link:hover {
            gap: 10px;
            text-decoration: underline;
        }

        /* Architecture Highlights */
        .features-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 18px;
        }

        .feature-box {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            padding: 20px;
            display: flex;
            gap: 14px;
            align-items: flex-start;
        }

        .feature-icon-badge {
            width: 40px;
            height: 40px;
            border-radius: 10px;
            background: #eff6ff;
            color: #2563eb;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
        }

        .feature-text h4 {
            font-size: 14px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 4px;
        }

        .feature-text p {
            font-size: 12.5px;
            color: #64748b;
            line-height: 1.45;
        }

        /* Diagnostic Quick Info Panel */
        .stats-panel {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            padding: 24px 28px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.03);
        }

        .stat-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .stat-label {
            font-size: 11.5px;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.06em;
        }

        .stat-val {
            font-size: 17px;
            font-weight: 700;
            color: #0f172a;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="home-container">
        
        <!-- 1. Hero Welcome Section -->
        <div class="hero">
            <div class="hero-badge">&#127760; ASP.NET WebForms Master Portal</div>
            <h1 class="hero-title">WebApplication4 Central Portal</h1>
            <p class="hero-desc">
                Welcome to the unified WebApplication4 hub. Explore modern ASP.NET WebForms features including unified master layout, user state, database inventory management, real-time AJAX validation, client-side cookies, and SEO URL routing.
            </p>
            <div class="hero-actions">
                <a href="SignUp.aspx" class="btn-hero-primary">&#128100; View User Dashboard</a>
                <a href="ProductForm.aspx" class="btn-hero-secondary">&#128230; Open Product Form</a>
                <a href="FirstPage.aspx" class="btn-hero-secondary">&#128272; Sign In Page</a>
            </div>
        </div>

        <!-- 2. System Architecture Highlights -->
        <div>
            <div class="section-header">
                <div>
                    <h2 class="section-title">&#9881; Core Architecture Highlights</h2>
                    <p class="section-subtitle">Key engineering concepts implemented in this application</p>
                </div>
            </div>

            <div class="features-row">
                <div class="feature-box">
                    <div class="feature-icon-badge">&#128203;</div>
                    <div class="feature-text">
                        <h4>Master Page Architecture</h4>
                        <p>Consistent header, live user status, global navigation, and script manager hosted centrally in <code>Site.Master</code>.</p>
                    </div>
                </div>

                <div class="feature-box">
                    <div class="feature-icon-badge">&#128161;</div>
                    <div class="feature-text">
                        <h4>Multi-Tier State Control</h4>
                        <p>Application state (live online counters), Session state (authenticated user), ViewState (counters), and HTTP Cookies.</p>
                    </div>
                </div>

                <div class="feature-box">
                    <div class="feature-icon-badge">&#9889;</div>
                    <div class="feature-text">
                        <h4>Asynchronous AJAX</h4>
                        <p>Flicker-free database availability validation powered by ScriptManager, UpdatePanel, and UpdateProgress.</p>
                    </div>
                </div>

                <div class="feature-box">
                    <div class="feature-icon-badge">&#128279;</div>
                    <div class="feature-text">
                        <h4>Clean URL Routing</h4>
                        <p>Search engine friendly URLs using ASP.NET <code>RouteTable.Routes.MapPageRoute</code> and route parameter handlers.</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- 3. Application Modules Showcase Grid -->
        <div>
            <div class="section-header">
                <div>
                    <h2 class="section-title">&#129513; Interactive Modules Directory</h2>
                    <p class="section-subtitle">Navigate through the project's specialized functional pages</p>
                </div>
            </div>

            <div class="module-grid">
                <!-- 1. Product Form -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-blue">&#128230;</div>
                            <span class="tech-pill">SQL Database</span>
                        </div>
                        <div class="module-name">Product Management</div>
                        <p class="module-desc">Add, validate, and persist catalog inventory products with formatted prices and category selection in a GridView.</p>
                    </div>
                    <a href="ProductForm.aspx" class="module-link">Open Product Form &rarr;</a>
                </div>

                <!-- 2. Check Availability -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-teal">&#128737;</div>
                            <span class="tech-pill">AJAX UpdatePanel</span>
                        </div>
                        <div class="module-name">Username Availability</div>
                        <p class="module-desc">Real-time asynchronous checking against SQL database using ASP.NET AJAX UpdatePanel without full page reload.</p>
                    </div>
                    <a href="CheckAvailability.aspx" class="module-link">Test Availability &rarr;</a>
                </div>

                <!-- 3. Search User -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-violet">&#128269;</div>
                            <span class="tech-pill">QueryString</span>
                        </div>
                        <div class="module-name">Search User Directory</div>
                        <p class="module-desc">Search users and safely transfer parameters via URL query strings to the recipient GetUser detail page.</p>
                    </div>
                    <a href="SearchUser.aspx" class="module-link">Search Directory &rarr;</a>
                </div>

                <!-- 4. Cookie State -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-amber">&#127850;</div>
                            <span class="tech-pill">HTTP State</span>
                        </div>
                        <div class="module-name">HTTP Cookie Management</div>
                        <p class="module-desc">Demonstrates client-side multi-value cookies, expiration lifecycles, and reading cookie values from incoming HTTP headers.</p>
                    </div>
                    <a href="CookiesSend.aspx" class="module-link">Manage Cookies &rarr;</a>
                </div>

                <!-- 5. URL Routing -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-emerald">&#128279;</div>
                            <span class="tech-pill">MapPageRoute</span>
                        </div>
                        <div class="module-name">SEO URL Routing</div>
                        <p class="module-desc">Demonstrates ASP.NET RouteTable with dynamic slug generation and parameterized detail views for articles.</p>
                    </div>
                    <a href="URLRouting.aspx" class="module-link">View Articles &rarr;</a>
                </div>

                <!-- 6. Counting & Metrics -->
                <div class="module-card">
                    <div>
                        <div class="module-header">
                            <div class="module-icon icon-rose">&#128290;</div>
                            <span class="tech-pill">ViewState</span>
                        </div>
                        <div class="module-name">ViewState Click Counter</div>
                        <p class="module-desc">Tracks button click counters and state preservation across postback cycles using ASP.NET ViewState.</p>
                    </div>
                    <a href="Counting.aspx" class="module-link">View Counter &rarr;</a>
                </div>
            </div>
        </div>

        <!-- 4. System Status & Runtime Info -->
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
                <span class="stat-label">Target Framework</span>
                <span class="stat-val">.NET Framework 4.8.1</span>
            </div>
            <div class="stat-item">
                <span class="stat-label">Server Timestamp</span>
                <span class="stat-val"><%= DateTime.Now.ToString("hh:mm tt") %></span>
            </div>
        </div>

    </div>
</asp:Content>
