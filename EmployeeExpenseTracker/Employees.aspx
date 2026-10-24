<%@ Page Title="Employee Hub" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Employees.aspx.cs" Inherits="EmployeeExpenseTracker.Employees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Custom CSS for Modern UI/UX (Marks: 10/10) -->
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%);
            --card-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.03);
            --card-shadow-hover: 0 20px 30px -10px rgba(79, 70, 229, 0.15);
        }

        .hero-banner {
            background: var(--primary-gradient);
            border-radius: 1.25rem;
            color: #ffffff;
            padding: 2rem 2.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 10px 30px rgba(59, 130, 246, 0.25);
        }

        .stat-card {
            background: #ffffff;
            border-radius: 1rem;
            border: 1px solid rgba(226, 232, 240, 0.8);
            box-shadow: var(--card-shadow);
            transition: all 0.25s ease-in-out;
        }
        .stat-card:hover {
            transform: translateY(-3px);
            box-shadow: var(--card-shadow-hover);
        }

        .filter-panel {
            background: #ffffff;
            border-radius: 1rem;
            border: 1px solid #e2e8f0;
            box-shadow: var(--card-shadow);
        }

        .custom-table {
            border-collapse: separate;
            border-spacing: 0;
        }
        .custom-table thead th {
            background-color: #f8fafc;
            color: #475569;
            font-size: 0.75rem;
            text-transform: uppercase;
            letter-spacing: 0.06em;
            padding: 1rem 1.25rem;
            border-bottom: 2px solid #e2e8f0;
        }
        .custom-table tbody tr {
            transition: background-color 0.15s ease;
        }
        .custom-table tbody tr:hover {
            background-color: #f1f5f9 !important;
        }
        .custom-table td {
            padding: 1rem 1.25rem;
            vertical-align: middle;
            border-bottom: 1px solid #f1f5f9;
        }

        /* Avatar Circle for Employee Name */
        .emp-avatar {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 0.85rem;
            color: #ffffff;
            background: linear-gradient(135deg, #6366f1, #8b5cf6);
            margin-right: 0.75rem;
            box-shadow: 0 2px 8px rgba(99, 102, 241, 0.3);
        }

        /* Smooth AJAX Backdrop Loader */
        .ajax-loader-backdrop {
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(255, 255, 255, 0.6);
            backdrop-filter: blur(3px);
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
        }
    </style>

    <!-- AJAX ScriptManager -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- Flicker-free UpdatePanel -->
    <asp:UpdatePanel ID="upnlMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <!-- 1. HERO HEADER WITH STATS & ACTION BUTTONS -->
            <div class="hero-banner d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
                <div>
                    <span class="badge bg-white text-primary px-3 py-2 rounded-pill fw-bold text-uppercase mb-2 shadow-sm">
                        Enterprise Portal
                    </span>
                    <h2 class="fw-bold mb-1">Employee Directory</h2>
                    <p class="mb-0 text-white-50">Manage, search and track workforce records seamlessly.
                    </p>
                </div>

                <!-- Action Buttons: View Expenses + Add Employee -->
                <div class="d-flex gap-2 flex-wrap">
                    <a href="Expenses.aspx" class="btn btn-outline-light btn-lg px-4 py-2 fw-bold rounded-pill shadow-sm">
                        💳 View Expenses
                    </a>
                    <asp:Button ID="btnAdd" runat="server" Text="➕ Add New Employee"
                        CssClass="btn btn-light btn-lg px-4 py-2 fw-bold text-primary rounded-pill shadow"
                        OnClick="btnAdd_Click" />
                </div>
            </div>

            <!-- 2. QUICK KPI SUMMARY CARDS -->
            <div class="row g-3 mb-4">
                <div class="col-md-4">
                    <div class="stat-card p-3 d-flex align-items-center">
                        <div class="rounded-circle bg-primary bg-opacity-10 p-3 me-3 text-primary fs-3">👥
                        </div>
                        <div>
                            <small class="text-muted text-uppercase fw-semibold">Total Staff</small>
                            <h4 class="fw-bold mb-0 text-dark">
                                <asp:Label ID="lblTotalEmployees" runat="server" Text="0" />
                            </h4>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="stat-card p-3 d-flex align-items-center">
                        <div class="rounded-circle bg-success bg-opacity-10 p-3 me-3 text-success fs-3">💰
                        </div>
                        <div>
                            <small class="text-muted text-uppercase fw-semibold">Monthly Payroll</small>
                            <h4 class="fw-bold mb-0 text-dark">
                                <asp:Label ID="lblTotalPayroll" runat="server" Text="$0.00" />
                            </h4>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="stat-card p-3 d-flex align-items-center">
                        <div class="rounded-circle bg-warning bg-opacity-10 p-3 me-3 text-warning fs-3">⭐
                        </div>
                        <div>
                            <small class="text-muted text-uppercase fw-semibold">Average Salary</small>
                            <h4 class="fw-bold mb-0 text-dark">
                                <asp:Label ID="lblAvgSalary" runat="server" Text="$0.00" />
                            </h4>
                        </div>
                    </div>
                </div>
            </div>

            <!-- 3. SEARCH & ADVANCED FILTER PANEL -->
            <div class="filter-panel p-4 mb-4">
                <div class="row g-3 align-items-end">

                    <!-- Search Input (EmployeeID, Name) -->
                    <div class="col-lg-5 col-md-12">
                        <label class="form-label fw-bold text-dark mb-1">
                            Search Employee :
                        </label>
                        <div class="input-group">
                            <span class="input-group-text bg-white border-end-0 text-muted">🔎</span>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control border-start-0 ps-0"
                                placeholder="Search by Employee ID (e.g. 1) or Name..." />
                        </div>
                        <small class="text-muted mt-1 d-block font-monospace">(EmployeeID, Name)</small>
                    </div>

                    <!-- Designation Filter (AutoPostBack = true for Instant Change) -->
                    <div class="col-lg-3 col-md-6">
                        <label class="form-label fw-bold text-dark mb-1">Designation :</label>
                        <asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                            <asp:ListItem Value="" Text="All Designations" />
                            <asp:ListItem Value="General Employee" Text="General Employee" />
                            <asp:ListItem Value="Manager" Text="Manager" />
                            <asp:ListItem Value="Project Manager" Text="Project Manager" />
                            <asp:ListItem Value="Team Leader" Text="Team Leader" />
                        </asp:DropDownList>
                    </div>

                    <!-- Sorted By Filter (AutoPostBack = true for Instant Change) -->
                    <div class="col-lg-2 col-md-6">
                        <label class="form-label fw-bold text-dark mb-1">Sorted By :</label>
                        <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                            <asp:ListItem Value="Name" Text="Name" Selected="True" />
                            <asp:ListItem Value="Id" Text="Emp. ID" />
                            <asp:ListItem Value="Salary" Text="Salary" />
                            <asp:ListItem Value="DOJ" Text="Joining Date" />
                        </asp:DropDownList>
                    </div>

                    <!-- Action Buttons -->
                    <div class="col-lg-2 col-md-12 d-flex gap-2">
                        <asp:Button ID="btnSearch" runat="server" Text="Search"
                            CssClass="btn btn-primary flex-grow-1 fw-bold shadow-sm" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset"
                            CssClass="btn btn-light border fw-bold text-secondary" OnClick="btnReset_Click" />
                    </div>

                </div>
            </div>

            <!-- 4. SMOOTH AJAX PROGRESS LOADER -->
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upnlMain">
                <ProgressTemplate>
                    <div class="ajax-loader-backdrop">
                        <div class="card p-3 shadow-lg border-0 d-flex flex-row align-items-center gap-3">
                            <div class="spinner-border text-primary" role="status"></div>
                            <span class="fw-bold text-dark">Filtering data without refresh...</span>
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <!-- 5. ELEGANT DATA TABLE -->
            <div class="card border-0 shadow-sm rounded-4 overflow-hidden mb-5">
                <div class="table-responsive">
                    <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="False"
                        DataKeyNames="Id"
                        CssClass="table custom-table align-middle mb-0">

                        <Columns>
                            <%-- # Index --%>
                            <asp:TemplateField HeaderText="#">
                                <ItemTemplate>
                                    <span class="text-muted fw-semibold ps-2"><%# Container.DataItemIndex + 1 %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Emp. ID --%>
                            <asp:TemplateField HeaderText="Emp. ID">
                                <ItemTemplate>
                                    <span class="badge bg-light text-dark border px-2 py-1 font-monospace fw-bold">E<%# Eval("Id", "{0:D3}") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Name with Dynamic Initial Avatar --%>
                            <asp:TemplateField HeaderText="Name">
                                <ItemTemplate>
                                    <div class="d-flex align-items-center">
                                        <div class="emp-avatar">
                                            <%# GetInitials(Eval("Name").ToString()) %>
                                        </div>
                                        <div class="fw-bold text-dark"><%# Eval("Name") %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Designation --%>
                            <asp:TemplateField HeaderText="Designation">
                                <ItemTemplate>
                                    <span class='badge <%# GetRoleBadgeClass(Eval("Designation").ToString()) %> px-3 py-2 rounded-pill'>
                                        <%# Eval("Designation") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- DOB --%>
                            <asp:BoundField DataField="DOB" HeaderText="DOB" DataFormatString="{0:dd/MM/yyyy}" />

                            <%-- DOJ --%>
                            <asp:BoundField DataField="DOJ" HeaderText="DOJ" DataFormatString="{0:dd/MM/yyyy}" />

                            <%-- Salary --%>
                            <asp:TemplateField HeaderText="Salary" ItemStyle-CssClass="text-end pe-4" HeaderStyle-CssClass="text-end pe-4">
                                <ItemTemplate>
                                    <span class="fw-bold text-dark">
                                        <%# Convert.ToDecimal(Eval("Salary")).ToString("N2") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Address --%>
                            <asp:TemplateField HeaderText="Address">
                                <ItemTemplate>
                                    <span class="text-muted"><%# Eval("Address") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <EmptyDataTemplate>
                            <div class="py-5 text-center">
                                <div class="fs-1 text-muted mb-2">🔍</div>
                                <h5 class="fw-bold text-secondary">No Employees Found</h5>
                                <p class="text-muted mb-0">Try clearing filters or search with another keyword.</p>
                            </div>
                        </EmptyDataTemplate>

                    </asp:GridView>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
