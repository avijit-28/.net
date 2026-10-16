<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="WebApplication4.AllGridProp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>GridView Properties & Advanced Features Lab (Pages 10 - 30)</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet" />
    <style>
        :root {
            --bg-body: #0f172a;
            --bg-card: #1e293b;
            --bg-card-subtle: #334155;
            --border-color: #334155;
            --text-primary: #f8fafc;
            --text-secondary: #94a3b8;
            --text-muted: #64748b;
            --accent-blue: #38bdf8;
            --accent-indigo: #6366f1;
            --accent-emerald: #10b981;
            --accent-rose: #f43f5e;
            --accent-amber: #f59e0b;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #0b1120 0%, #0f172a 50%, #1e1b4b 100%);
            color: var(--text-primary);
            min-height: 100vh;
            padding: 32px 20px 60px 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .header-panel {
            background: rgba(30, 41, 59, 0.8);
            backdrop-filter: blur(12px);
            border: 1px solid rgba(255, 255, 255, 0.1);
            border-radius: 20px;
            padding: 32px;
            margin-bottom: 28px;
            box-shadow: 0 20px 40px -15px rgba(0, 0, 0, 0.5);
            position: relative;
            overflow: hidden;
        }

        .header-panel::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: linear-gradient(90deg, #38bdf8, #818cf8, #c084fc, #f43f5e);
        }

        .badge-bar {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-bottom: 16px;
        }

        .badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            padding: 6px 14px;
            border-radius: 9999px;
            border: 1px solid rgba(255, 255, 255, 0.15);
        }

        .badge-blue { background: rgba(56, 189, 248, 0.15); color: #38bdf8; border-color: rgba(56, 189, 248, 0.3); }
        .badge-emerald { background: rgba(16, 185, 129, 0.15); color: #34d399; border-color: rgba(16, 185, 129, 0.3); }
        .badge-rose { background: rgba(244, 63, 94, 0.15); color: #fb7185; border-color: rgba(244, 63, 94, 0.3); }
        .badge-purple { background: rgba(192, 132, 252, 0.15); color: #c084fc; border-color: rgba(192, 132, 252, 0.3); }

        .title {
            font-size: 32px;
            font-weight: 800;
            letter-spacing: -0.03em;
            color: #ffffff;
            margin-bottom: 10px;
        }

        .subtitle {
            font-size: 15px;
            color: var(--text-secondary);
            line-height: 1.6;
            max-width: 900px;
        }

        .nav-tabs-bar {
            display: flex;
            gap: 12px;
            margin-bottom: 24px;
            border-bottom: 1px solid var(--border-color);
            padding-bottom: 12px;
            overflow-x: auto;
        }

        .tab-btn {
            background: rgba(30, 41, 59, 0.6);
            color: var(--text-secondary);
            border: 1px solid var(--border-color);
            padding: 10px 20px;
            border-radius: 12px;
            font-weight: 600;
            font-size: 14px;
            cursor: pointer;
            transition: all 0.2s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .tab-btn:hover {
            color: #ffffff;
            border-color: var(--accent-blue);
            transform: translateY(-1px);
        }

        .tab-btn.active {
            background: linear-gradient(135deg, #2563eb, #1d4ed8);
            color: #ffffff;
            border-color: #3b82f6;
            box-shadow: 0 4px 14px rgba(37, 99, 235, 0.4);
        }

        .card-section {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 18px;
            padding: 28px;
            margin-bottom: 32px;
            box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.3);
        }

        .card-header-area {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 20px;
            padding-bottom: 16px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.08);
            flex-wrap: wrap;
            gap: 16px;
        }

        .card-title-group h2 {
            font-size: 20px;
            font-weight: 700;
            color: #ffffff;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .card-title-group p {
            font-size: 13.5px;
            color: var(--text-secondary);
            margin-top: 6px;
            line-height: 1.5;
        }

        .page-badge {
            background: rgba(99, 102, 241, 0.2);
            color: #a5b4fc;
            padding: 4px 10px;
            border-radius: 6px;
            font-size: 12px;
            font-family: 'JetBrains Mono', monospace;
            font-weight: 600;
        }

        .info-callout {
            background: rgba(15, 23, 42, 0.6);
            border-left: 4px solid var(--accent-blue);
            padding: 14px 18px;
            border-radius: 0 10px 10px 0;
            margin-bottom: 20px;
            font-size: 13.5px;
            color: #cbd5e1;
            line-height: 1.5;
        }

        .info-callout code {
            font-family: 'JetBrains Mono', monospace;
            background: rgba(255, 255, 255, 0.1);
            color: #38bdf8;
            padding: 2px 6px;
            border-radius: 4px;
            font-size: 12px;
        }

        /* GridView Styling */
        .gridview-wrapper {
            overflow-x: auto;
            border-radius: 12px;
            background: #ffffff;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
            margin: 16px 0;
        }

        .aspnet-grid {
            width: 100%;
            border-collapse: collapse;
            font-family: 'Inter', Arial, sans-serif;
            font-size: 13.5px;
        }

        .aspnet-grid th {
            padding: 12px 16px;
            text-align: left;
            font-weight: 700;
            letter-spacing: 0.02em;
        }

        .aspnet-grid td {
            padding: 10px 16px;
            border-bottom: 1px solid rgba(0, 0, 0, 0.08);
            vertical-align: middle;
        }

        /* Controls inside GridView */
        .aspnet-grid input[type="text"] {
            padding: 6px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
            width: 100%;
            max-width: 180px;
            box-sizing: border-box;
        }

        .aspnet-grid input[type="text"]:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
        }

        .aspnet-grid select {
            padding: 6px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
            background: #ffffff;
            cursor: pointer;
        }

        .aspnet-grid select:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
        }

        .aspnet-grid a {
            color: #1e40af;
            font-weight: 600;
            text-decoration: none;
            padding: 4px 10px;
            border-radius: 6px;
            display: inline-block;
            transition: all 0.15s ease;
            margin-right: 6px;
        }

        .aspnet-grid a:hover {
            background: #dbeafe;
            color: #1d4ed8;
            text-decoration: underline;
        }

        .btn-insert-link {
            background: #059669 !important;
            color: #ffffff !important;
            font-weight: 700 !important;
            padding: 6px 14px !important;
            border-radius: 6px !important;
            text-decoration: none !important;
            display: inline-flex !important;
            align-items: center !important;
            gap: 6px !important;
        }

        .btn-insert-link:hover {
            background: #047857 !important;
            box-shadow: 0 2px 8px rgba(5, 150, 105, 0.3) !important;
        }

        /* Validation Summary boxes */
        .val-summary-container {
            margin: 16px 0;
            padding: 14px 18px;
            border-radius: 10px;
            background: rgba(239, 68, 68, 0.1);
            border: 1px solid rgba(239, 68, 68, 0.3);
            color: #f87171;
            font-size: 13.5px;
        }

        .val-summary-container ul {
            margin: 8px 0 0 20px;
            padding: 0;
        }

        .val-summary-container li {
            margin-bottom: 4px;
        }

        /* Action bar */
        .btn-action-primary {
            background: linear-gradient(135deg, #3b82f6, #2563eb);
            color: #ffffff;
            border: none;
            padding: 8px 18px;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .btn-action-primary:hover {
            background: linear-gradient(135deg, #2563eb, #1d4ed8);
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
        }

        .status-pill {
            display: inline-block;
            padding: 6px 14px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            margin-top: 10px;
        }

        .status-success {
            background: rgba(16, 185, 129, 0.15);
            color: #34d399;
            border: 1px solid rgba(16, 185, 129, 0.3);
        }

        .legend-box {
            display: flex;
            align-items: center;
            gap: 16px;
            margin-top: 12px;
            font-size: 12.5px;
            color: var(--text-secondary);
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .legend-color {
            width: 14px;
            height: 14px;
            border-radius: 4px;
            display: inline-block;
        }

        .legend-red {
            background: #e11d48;
        }

        .legend-normal {
            background: #fff7e7;
            border: 1px solid #deba84;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Header Banner -->
            <div class="header-panel">
                <div class="badge-bar">
                    <span class="badge badge-blue">ASP.NET Web Forms</span>
                    <span class="badge badge-emerald">Presentation Slides: Pages 10 – 30</span>
                    <span class="badge badge-purple">Standalone Execution Mode</span>
                    <span class="badge badge-rose">GridView Master Lab</span>
                </div>
                <h1 class="title">GridView Properties, Events & CRUD Lab</h1>
                <p class="subtitle">
                    Demonstrating multi-culture currency formatting per row via <code>RowDataBound</code>, hidden culture column binding, runtime conditional formatting (highlighting rows where <code>Salary &gt; 70000</code>), <code>TemplateField</code> editing with <code>DropDownList</code>, <code>RequiredFieldValidator</code> controls, and full inline Insert/Update/Delete operations with <code>FooterTemplate</code>.
                </p>
                <div style="margin-top: 18px; display: flex; gap: 12px; align-items: center; flex-wrap: wrap;">
                    <asp:Button ID="btnResetData" runat="server" Text="&#8634; Reset Demo Records to Slide Defaults" CssClass="btn-action-primary" OnClick="btnResetData_Click" />
                    <asp:Label ID="lblTopMessage" runat="server" CssClass="status-pill status-success" Visible="false"></asp:Label>
                </div>
            </div>

            <!-- Quick Navigation Jump Links -->
            <div class="nav-tabs-bar">
                <a href="#section1" class="tab-btn active">&#128181; 1. Multi-Culture per Row (Pages 10–13)</a>
                <a href="#section2" class="tab-btn">&#128065; 2. Hidden Culture &amp; Highlighting &gt; 70k (Pages 14–19)</a>
                <a href="#section3" class="tab-btn">&#9997; 3. Inline CRUD &amp; Validators (Pages 20–30)</a>
            </div>

            <!-- SECTION 1: Pages 10-13 (Multi-Culture Currency Formatting based on Country) -->
            <div id="section1" class="card-section">
                <div class="card-header-area">
                    <div class="card-title-group">
                        <h2>
                            <span>&#128181;</span> Multi-Culture Currency Formatting per Row
                        </h2>
                        <p>
                            Each row belongs to an employee from a different country. In the <code>RowDataBound</code> event, the code inspects the <code>Country</code> column and dynamically applies the matching <code>CultureInfo</code>: <strong>US ($)</strong>, <strong>UK (&pound;)</strong>, <strong>India (&#8377;)</strong>, <strong>South Africa (R)</strong>, and <strong>Malaysia (RM)</strong>.
                        </p>
                    </div>
                    <span class="page-badge">Slides 10 – 13</span>
                </div>

                <div class="info-callout">
                    <strong>Code Implementation:</strong><br />
                    <code>string formattedString = string.Format(new CultureInfo("en-US"), "{0:c}", salary);</code><br />
                    <code>e.Row.Cells[2].Text = formattedString;</code>
                </div>

                <div class="gridview-wrapper">
                    <asp:GridView ID="GridView1" runat="server" 
                        AutoGenerateColumns="False" 
                        DataKeyNames="EmployeeId" 
                        BackColor="#DEBA84" 
                        BorderColor="#DEBA84" 
                        BorderStyle="None" 
                        BorderWidth="1px" 
                        CellPadding="3" 
                        CellSpacing="2" 
                        OnRowDataBound="GridView1_RowDataBound"
                        CssClass="aspnet-grid">
                        <Columns>
                            <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" InsertVisible="False" ReadOnly="True" SortExpression="EmployeeId" />
                            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                            <asp:BoundField DataField="Salary" HeaderText="Salary" SortExpression="Salary" />
                            <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" />
                        </Columns>
                        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                    </asp:GridView>
                </div>
            </div>

            <!-- SECTION 2: Pages 14-19 (Hidden CountryCulture Column & Conditional Row Highlighting) -->
            <div id="section2" class="card-section">
                <div class="card-header-area">
                    <div class="card-title-group">
                        <h2>
                            <span>&#128065;</span> Hidden Culture Column &amp; Conditional Row Highlighting (> 70,000)
                        </h2>
                        <p>
                            1. <strong>Hidden Column:</strong> <code>CountryCulture</code> (Cell index 4) contains culture codes (<code>en-US</code>, <code>en-IN</code>, etc.) and is hidden via <code>e.Row.Cells[4].Visible = false;</code> for both Header and Data rows.<br />
                            2. <strong>Conditional Formatting:</strong> If <code>AnnualSalary &gt; 70000</code>, the row is dynamically styled with a <strong>RED background</strong> and <strong>WHITE text</strong>.
                        </p>
                    </div>
                    <span class="page-badge">Slides 14 – 19</span>
                </div>

                <div class="info-callout">
                    <strong>Code Implementation:</strong><br />
                    <code>if (e.Row.RowType == DataControlRowType.Header) { e.Row.Cells[4].Visible = false; }</code><br />
                    <code>else if (e.Row.RowType == DataControlRowType.DataRow) {</code><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<code>e.Row.Cells[4].Visible = false;</code><br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<code>if (salary &gt; 70000) { e.Row.BackColor = Color.Red; e.Row.ForeColor = Color.White; }</code><br />
                    <code>}</code>
                </div>

                <div class="legend-box">
                    <div class="legend-item">
                        <span class="legend-color legend-normal"></span> Normal Salary (&le; 70,000)
                    </div>
                    <div class="legend-item">
                        <span class="legend-color legend-red"></span> High Salary (&gt; 70,000) &mdash; Highlighted Red
                    </div>
                </div>

                <div class="gridview-wrapper">
                    <asp:GridView ID="GridView2" runat="server" 
                        AutoGenerateColumns="False" 
                        DataKeyNames="EmployeeId" 
                        BackColor="#DEBA84" 
                        BorderColor="#DEBA84" 
                        BorderStyle="None" 
                        BorderWidth="1px" 
                        CellPadding="3" 
                        CellSpacing="2" 
                        OnRowDataBound="GridView2_RowDataBound"
                        CssClass="aspnet-grid">
                        <Columns>
                            <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" InsertVisible="False" ReadOnly="True" SortExpression="EmployeeId" />
                            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                            <asp:BoundField DataField="AnnualSalary" HeaderText="AnnualSalary" SortExpression="AnnualSalary" />
                            <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" />
                            <asp:BoundField DataField="CountryCulture" HeaderText="CountryCulture" SortExpression="CountryCulture" />
                        </Columns>
                        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                    </asp:GridView>
                </div>
            </div>

            <!-- SECTION 3: Pages 20-30 (Inline CRUD, TemplateFields, DropDownList, Validators & FooterTemplate) -->
            <div id="section3" class="card-section">
                <div class="card-header-area">
                    <div class="card-title-group">
                        <h2>
                            <span>&#9997;</span> GridView Inline Insert, Update, and Delete with TemplateFields &amp; Validators
                        </h2>
                        <p>
                            Features from <strong>Slides 20 to 30</strong>:<br />
                            &bull; <strong>TemplateField</strong> used for custom controls.<br />
                            &bull; <strong>DropDownList</strong> in <code>EditItemTemplate</code> and <code>FooterTemplate</code> for Gender selection bound to <code>Bind("Gender")</code>.<br />
                            &bull; <strong>RequiredFieldValidator</strong> with <code>InitialValue="Select Gender"</code> prevents submitting without choosing Male or Female.<br />
                            &bull; <strong>FooterTemplate</strong> with <code>LinkButton</code> (<code>lbInsert</code>) to insert new records directly into the grid.<br />
                            &bull; <strong>ValidationSummary</strong> controls separated by <code>ValidationGroup="Edit"</code> and <code>ValidationGroup="Insert"</code>.
                        </p>
                    </div>
                    <span class="page-badge">Slides 20 – 30</span>
                </div>

                <!-- Validation Summaries -->
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                    ForeColor="Red" 
                    HeaderText="&#9888; Please correct the following edit errors:" 
                    ValidationGroup="Edit" 
                    CssClass="val-summary-container" />

                <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server" 
                    ForeColor="Red" 
                    HeaderText="&#9888; Please correct the following insert errors:" 
                    ValidationGroup="Insert" 
                    CssClass="val-summary-container" />

                <asp:Label ID="lblCrudStatus" runat="server" CssClass="status-pill status-success" Visible="false"></asp:Label>

                <div class="gridview-wrapper">
                    <asp:GridView ID="GridView3" runat="server" 
                        AutoGenerateColumns="False" 
                        DataKeyNames="EmployeeId" 
                        ShowFooter="True" 
                        BackColor="#DEBA84" 
                        BorderColor="#DEBA84" 
                        BorderStyle="None" 
                        BorderWidth="1px" 
                        CellPadding="3" 
                        CellSpacing="2"
                        OnRowEditing="GridView3_RowEditing"
                        OnRowCancelingEdit="GridView3_RowCancelingEdit"
                        OnRowUpdating="GridView3_RowUpdating"
                        OnRowDeleting="GridView3_RowDeleting"
                        CssClass="aspnet-grid">
                        <Columns>
                            <%-- Command Field for Edit and Delete --%>
                            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ValidationGroup="Edit" />

                            <%-- EmployeeId TemplateField --%>
                            <asp:TemplateField HeaderText="EmployeeId" InsertVisible="False" SortExpression="EmployeeId">
                                <EditItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("EmployeeId") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("EmployeeId") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lbInsert" ValidationGroup="Insert" runat="server" OnClick="lbInsert_Click" CssClass="btn-insert-link">+ Insert</asp:LinkButton>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Name TemplateField with Validators --%>
                            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEditName" runat="server" 
                                        ErrorMessage="Name is a required field"
                                        ControlToValidate="TextBox1" Text="*" ForeColor="Red" ValidationGroup="Edit">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtName" runat="server" placeholder="Enter Full Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvInsertName" runat="server" 
                                        ErrorMessage="Name is a required field" 
                                        ControlToValidate="txtName" Text="*" ForeColor="Red"
                                        ValidationGroup="Insert">
                                    </asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Gender TemplateField with DropDownList and InitialValue Validator --%>
                            <asp:TemplateField HeaderText="Gender" SortExpression="Gender">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                        <asp:ListItem>Select Gender</asp:ListItem>
                                        <asp:ListItem>Male</asp:ListItem>
                                        <asp:ListItem>Female</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvEditGender" runat="server" 
                                        ErrorMessage="Gender is a required field" Text="*"
                                        ControlToValidate="DropDownList1" ForeColor="Red" 
                                        InitialValue="Select Gender" ValidationGroup="Edit">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Gender") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlInsertGender" runat="server">
                                        <asp:ListItem>Select Gender</asp:ListItem>
                                        <asp:ListItem>Male</asp:ListItem>
                                        <asp:ListItem>Female</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvInsertGender" runat="server" 
                                        ErrorMessage="Gender is a required field" Text="*"
                                        ControlToValidate="ddlInsertGender" ForeColor="Red" 
                                        InitialValue="Select Gender" ValidationGroup="Insert">
                                    </asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- City TemplateField with Validators --%>
                            <asp:TemplateField HeaderText="City" SortExpression="City">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEditCity" runat="server" 
                                        ErrorMessage="City is a required field" Text="*"
                                        ControlToValidate="TextBox3" ForeColor="Red" ValidationGroup="Edit">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("City") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtCity" runat="server" placeholder="Enter City"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvInsertCity" runat="server" 
                                        ErrorMessage="City is a required field" Text="*" 
                                        ControlToValidate="txtCity" ForeColor="Red" 
                                        ValidationGroup="Insert">
                                    </asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
