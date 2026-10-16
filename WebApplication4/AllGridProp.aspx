<%@ Page Title="GridView Capabilities Hub" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="WebApplication4.AllGridProp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .grid-hub-container {
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 24px;
        }

        /* Hero Banner Card */
        .grid-hero-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
            border-radius: 16px;
            padding: 28px 32px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            position: relative;
            overflow: hidden;
        }

        .grid-hero-card::after {
            content: "";
            position: absolute;
            right: -40px;
            bottom: -40px;
            width: 220px;
            height: 220px;
            background: radial-gradient(circle, rgba(56, 189, 248, 0.2) 0%, transparent 70%);
            pointer-events: none;
        }

        .grid-badge-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-bottom: 12px;
        }

        .grid-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            background: rgba(56, 189, 248, 0.2);
            border: 1px solid rgba(56, 189, 248, 0.4);
            color: #7dd3fc;
            font-size: 11.5px;
            font-weight: 700;
            padding: 4px 12px;
            border-radius: 20px;
            letter-spacing: 0.04em;
            text-transform: uppercase;
        }

        .grid-hero-title {
            font-size: 26px;
            font-weight: 800;
            color: #ffffff;
            margin-bottom: 8px;
            letter-spacing: -0.02em;
        }

        .grid-hero-desc {
            font-size: 14.5px;
            color: #94a3b8;
            max-width: 850px;
            line-height: 1.6;
        }

        /* Modules Grid Layout */
        .modules-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
            gap: 20px;
        }

        .module-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px -3px rgba(0, 0, 0, 0.05);
            padding: 24px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            transition: all 0.2s ease;
        }

        .module-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 25px -4px rgba(0, 0, 0, 0.1);
            border-color: #3b82f6;
        }

        .card-top {
            display: flex;
            align-items: flex-start;
            gap: 16px;
            margin-bottom: 14px;
        }

        .card-icon {
            width: 46px;
            height: 46px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            flex-shrink: 0;
        }

        .icon-blue   { background: #eff6ff; color: #1d4ed8; border: 1px solid #dbeafe; }
        .icon-green  { background: #f0fdf4; color: #15803d; border: 1px solid #dcfce7; }
        .icon-purple { background: #faf5ff; color: #7e22ce; border: 1px solid #f3e8ff; }
        .icon-rose   { background: #fff1f2; color: #be123c; border: 1px solid #ffe4e6; }
        .icon-sky    { background: #f0f9ff; color: #0369a1; border: 1px solid #e0f2fe; }
        .icon-teal   { background: #f0fdfa; color: #0f766e; border: 1px solid #ccfbf1; }

        .card-title {
            font-size: 16.5px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 4px;
            line-height: 1.3;
        }

        .card-tag {
            font-size: 11.5px;
            font-weight: 700;
            color: #2563eb;
            text-transform: uppercase;
            letter-spacing: 0.04em;
        }

        .card-body-text {
            font-size: 13.5px;
            color: #64748b;
            line-height: 1.55;
            margin-bottom: 20px;
        }

        .btn-launch {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 18px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff !important;
            text-decoration: none;
            font-size: 13px;
            font-weight: 700;
            border-radius: 8px;
            transition: all 0.2s;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
        }

        .btn-launch:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.35);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="grid-hub-container">
        
        <!-- Header Banner -->
        <div class="grid-hero-card">
            <div class="grid-badge-row">
                <span class="grid-badge">&#128203; ASP.NET WebForms</span>
                <span class="grid-badge">&#9881; GridView Mastery</span>
                <span class="grid-badge">&#128188; Core Architecture</span>
            </div>
            <h1 class="grid-hero-title">GridView Capabilities &amp; Feature Modules</h1>
            <p class="grid-hero-desc">
                Select any module below to inspect its dedicated implementation, including multi-culture currency formatting, hidden column binding, runtime conditional highlighting, TemplateField editing, field validation, and complete inline CRUD operations.
            </p>
        </div>

        <!-- Modules Grid -->
        <div class="modules-grid">
            
            <!-- 1. Multi-Culture Formatting -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-green">&#128181;</div>
                        <div>
                            <span class="card-tag">RowDataBound Formatting</span>
                            <h3 class="card-title">Multi-Culture Currency Formatting</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Dynamically inspects the country in <code>RowDataBound</code> and applies country-specific currency symbols (US: $, UK: &pound;, India: &#8377;, South Africa: R, Malaysia: RM) using <code>CultureInfo</code>.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridMultiCulture") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

            <!-- 2. Hidden Culture Column -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-purple">&#128065;</div>
                        <div>
                            <span class="card-tag">Column Visibility</span>
                            <h3 class="card-title">Hidden Column Currency Binding</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Retrieves culture codes from a hidden column, formats the annual salary, and programmatically hides the culture cell in both header and data rows.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridHiddenCulture") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

            <!-- 3. Conditional Highlighting -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-rose">&#127912;</div>
                        <div>
                            <span class="card-tag">Runtime Styles</span>
                            <h3 class="card-title">Conditional Row Highlighting</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Combines design-time styling with runtime conditional formatting. Rows where annual salary exceeds 70,000 are dynamically styled with a prominent red background and white text.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridConditionalFormatting") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

            <!-- 4. TemplateField Editing -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-sky">&#9997;</div>
                        <div>
                            <span class="card-tag">Custom Templates</span>
                            <h3 class="card-title">TemplateField DropDownList Editing</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Replaces the default text editing interface for Gender with a user-friendly <code>DropDownList</code> control in <code>EditItemTemplate</code>, bound with <code>SelectedValue</code>.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridTemplateField") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

            <!-- 5. Field Validation -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-blue">&#9888;</div>
                        <div>
                            <span class="card-tag">Data Integrity</span>
                            <h3 class="card-title">GridView Field Validation</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Integrates <code>RequiredFieldValidator</code> controls directly inside templates with <code>InitialValue="Select Gender"</code>, accompanied by a clean <code>ValidationSummary</code>.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridValidation") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

            <!-- 6. Inline CRUD Operations -->
            <div class="module-card">
                <div>
                    <div class="card-top">
                        <div class="card-icon icon-teal">&#10010;</div>
                        <div>
                            <span class="card-tag">Full Inline Management</span>
                            <h3 class="card-title">Inline Insert, Update &amp; Delete</h3>
                        </div>
                    </div>
                    <p class="card-body-text">
                        Complete data management workflow: insert new employees directly through <code>FooterTemplate</code>, edit existing records with validation groups, and delete rows cleanly.
                    </p>
                </div>
                <a href="<%= ResolveUrl("~/GridInsertUpdateDelete") %>" class="btn-launch">Open Module &rarr;</a>
            </div>

        </div>
    </div>
</asp:Content>
