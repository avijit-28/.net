<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CrystalReportPractice.aspx.cs" Inherits="CrystalReportPractice" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Employee Reports Portal | Crystal Reports</title>

    <!-- Bootstrap 5.3 CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" />
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <!-- Google Fonts: Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet" />

    <!-- Crystal Report Viewer Client Scripts -->
    <script src='<%=ResolveUrl("~/crystalreportviewers13/js/crviewer/crv.js")%>' type="text/javascript"></script>

    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
            --card-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.04);
            --header-shadow: 0 4px 20px 0 rgba(0, 0, 0, 0.12);
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
            background-color: #f4f6f9;
            color: #334155;
            min-height: 100vh;
        }

        /* Top Navigation Header */
        .app-header {
            background: var(--primary-gradient);
            box-shadow: var(--header-shadow);
            padding: 1.25rem 0;
            color: #ffffff;
            margin-bottom: 2rem;
        }

        .app-header .brand-title {
            font-size: 1.5rem;
            font-weight: 700;
            letter-spacing: -0.025em;
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .app-header .brand-subtitle {
            font-size: 0.875rem;
            opacity: 0.85;
            margin-top: 0.2rem;
        }

        /* Modern Dashboard Card */
        .dash-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            box-shadow: var(--card-shadow);
            padding: 1.5rem;
            margin-bottom: 1.75rem;
            transition: all 0.2s ease;
        }

        .card-header-custom {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding-bottom: 1rem;
            margin-bottom: 1.25rem;
            border-bottom: 1px solid #f1f5f9;
        }

        .card-header-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #1e293b;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        /* Form Controls Styling */
        .form-label {
            font-size: 0.85rem;
            font-weight: 600;
            color: #475569;
            margin-bottom: 0.35rem;
        }

        .form-control, .form-select {
            border-radius: 8px;
            padding: 0.625rem 0.875rem;
            font-size: 0.925rem;
            border: 1.5px solid #cbd5e1;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }

        .form-control:focus, .form-select:focus {
            border-color: #2a5298;
            box-shadow: 0 0 0 3px rgba(42, 82, 152, 0.15);
        }

        .input-group-text {
            background-color: #f8fafc;
            border: 1.5px solid #cbd5e1;
            border-right: none;
            color: #64748b;
            border-radius: 8px 0 0 8px;
        }

        .input-group .form-control,
        .input-group .form-select {
            border-left: none;
            border-radius: 0 8px 8px 0;
        }

        /* Buttons Styling */
        .btn-modern {
            padding: 0.6rem 1.2rem;
            font-size: 0.9rem;
            font-weight: 500;
            border-radius: 8px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
            transition: all 0.2s ease;
        }

        .btn-search {
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
            border: none;
            color: white;
        }

        .btn-search:hover {
            background: linear-gradient(135deg, #172e56 0%, #21437c 100%);
            color: white;
            box-shadow: 0 4px 12px rgba(30, 60, 114, 0.3);
            transform: translateY(-1px);
        }

        .btn-pdf {
            background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
            border: none;
            color: white;
        }

        .btn-pdf:hover {
            background: linear-gradient(135deg, #b91c1c 0%, #991b1b 100%);
            color: white;
            box-shadow: 0 4px 12px rgba(220, 38, 38, 0.3);
            transform: translateY(-1px);
        }

        .btn-excel {
            background: linear-gradient(135deg, #16a34a 0%, #15803d 100%);
            border: none;
            color: white;
        }

        .btn-excel:hover {
            background: linear-gradient(135deg, #15803d 0%, #166534 100%);
            color: white;
            box-shadow: 0 4px 12px rgba(22, 163, 74, 0.3);
            transform: translateY(-1px);
        }

        .btn-reset {
            background-color: #f1f5f9;
            border: 1px solid #cbd5e1;
            color: #475569;
        }

        .btn-reset:hover {
            background-color: #e2e8f0;
            color: #1e293b;
        }

        /* Badges */
        .stat-badge {
            font-size: 0.8rem;
            font-weight: 600;
            padding: 0.35rem 0.75rem;
            border-radius: 20px;
            background-color: #e0f2fe;
            color: #0369a1;
            display: inline-flex;
            align-items: center;
            gap: 0.35rem;
        }

        /* Report Viewer Box */
        .report-viewport {
            background: #ffffff;
            border-radius: 12px;
            padding: 1.5rem;
            min-height: 800px;
            overflow-x: auto;
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .report-viewport table {
            margin: 0 auto;
        }

        /* Footer */
        .app-footer {
            text-align: center;
            font-size: 0.85rem;
            color: #94a3b8;
            padding: 2rem 0 3rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Top Navigation Bar -->
        <header class="app-header">
            <div class="container-fluid px-4">
                <div class="row align-items-center">
                    <div class="col-md-7">
                        <div class="brand-title">
                            <i class="fa-solid fa-chart-pie fa-lg text-warning"></i>
                            <span>Employee Report Portal</span>
                        </div>
                        <div class="brand-subtitle">
                            Automated Reporting &bull; Crystal Reports 13 &bull; SQL Server Data Integration
                        </div>
                    </div>
                    <div class="col-md-5 text-md-end mt-2 mt-md-0">
                        <span class="badge bg-light text-dark px-3 py-2 rounded-pill shadow-sm">
                            <i class="fa-solid fa-server text-success me-1"></i> Connected: Assignmet DB
                        </span>
                    </div>
                </div>
            </div>
        </header>

        <!-- Main Content Container -->
        <main class="container-fluid px-4">

            <!-- Filter & Action Controls Card -->
            <div class="dash-card">
                <div class="card-header-custom">
                    <div class="card-header-title">
                        <i class="fa-solid fa-filter text-primary"></i>
                        <span>Search & Filter Parameters</span>
                    </div>
                    <div>
                        <asp:Label ID="lblRecordCount" runat="server" CssClass="stat-badge">
                            <i class="fa-solid fa-database"></i> Ready
                        </asp:Label>
                    </div>
                </div>

                <!-- Alert Message Panel (Conditional) -->
                <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible fade show" role="alert">
                    <asp:Literal ID="litAlertMessage" runat="server"></asp:Literal>
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </asp:Panel>

                <!-- Filter Form Row -->
                <div class="row g-3 align-items-end">
                    <!-- First Name Filter -->
                    <div class="col-lg-3 col-md-6">
                        <label class="form-label" for="txtFirstName">
                            <i class="fa-solid fa-user me-1 text-muted"></i> First Name
                        </label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fa-solid fa-magnifying-glass"></i></span>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                placeholder="Enter first name to search..." AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Country Dropdown Filter -->
                    <div class="col-lg-3 col-md-6">
                        <label class="form-label" for="ddlCountry">
                            <i class="fa-solid fa-globe me-1 text-muted"></i> Country
                        </label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fa-solid fa-earth-americas"></i></span>
                            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Action Buttons -->
                    <div class="col-lg-6 col-md-12">
                        <div class="d-flex flex-wrap gap-2 justify-content-lg-end">
                            <!-- Search Button -->
                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-modern btn-search" OnClick="btnSearch_Click">
                                <i class="fa-solid fa-magnifying-glass"></i> Search
                            </asp:LinkButton>

                            <!-- Reset Button -->
                            <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-modern btn-reset" OnClick="btnReset_Click">
                                <i class="fa-solid fa-arrows-rotate"></i> Reset
                            </asp:LinkButton>

                            <!-- Convert to PDF Button -->
                            <asp:LinkButton ID="btnPdf" runat="server" CssClass="btn btn-modern btn-pdf" OnClick="btnPdf_Download">
                                <i class="fa-solid fa-file-pdf"></i> Export to PDF
                            </asp:LinkButton>

                            <!-- Convert to Excel Button -->
                            <asp:LinkButton ID="btnExcel" runat="server" CssClass="btn btn-modern btn-excel" OnClick="btnExcel_Download">
                                <i class="fa-solid fa-file-excel"></i> Export to Excel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Report Viewer Section -->
            <div class="dash-card">
                <div class="card-header-custom">
                    <div class="card-header-title">
                        <i class="fa-solid fa-table-list text-primary"></i>
                        <span>Employee Details Report Preview</span>
                    </div>
                    <div class="text-muted small">
                        <i class="fa-solid fa-file-lines me-1"></i> CrystalReport2.rpt
                    </div>
                </div>

                <div class="report-viewport shadow-sm">
                    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
                        AutoDataBind="true"
                        BestFitPage="False"
                        DisplayToolbar="True"
                        EnableDatabaseLogonPrompt="False"
                        EnableParameterPrompt="False"
                        HasCrystalLogo="False"
                        HasToggleGroupTreeButton="False"
                        HasToggleParameterPanelButton="False"
                        ToolPanelView="None" 
                        Width="100%" 
                        Height="1000px" />
                </div>
            </div>

        </main>

        <!-- Footer -->
        <footer class="app-footer">
            <div class="container">
                <p class="mb-0">Employee Analytics & Reporting System &bull; Redesigned with ASP.NET & Crystal Reports</p>
            </div>
        </footer>
    </form>

    <!-- Bootstrap 5 Bundle JS (includes Popper) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
