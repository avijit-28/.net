<%@ Page Title="Expenses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Expenses.aspx.cs" Inherits="EmployeeExpenseTracker.Expenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Header with Back Button to Employees Page -->
    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h2 class="fw-bold text-dark mb-0">💳 Employee Expense Tracker</h2>
            <small class="text-muted">Manage employee expense claims with linked foreign keys</small>
        </div>
        <a href="Employees.aspx" class="btn btn-outline-dark px-3 py-2 fw-semibold rounded-pill shadow-sm">
            👥 Back to Employee Directory
        </a>
    </div>

    <!-- Validation Summary -->
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger"
        HeaderText="Please correct the following errors:" DisplayMode="BulletList" />

    <!-- Summary Dashboard Cards -->
    <div class="row g-3 mb-4">
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-primary border-4">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase mb-1">Total Expenses Claimed</h6>
                    <h3 class="fw-bold text-primary mb-0">
                        <asp:Label ID="lblTotalAmount" runat="server" Text="$0.00" />
                    </h3>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-success border-4">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase mb-1">Total Claims Count</h6>
                    <h3 class="fw-bold text-success mb-0">
                        <asp:Label ID="lblTotalCount" runat="server" Text="0" />
                    </h3>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-warning border-4">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase mb-1">Highest Single Claim</h6>
                    <h3 class="fw-bold text-warning mb-0">
                        <asp:Label ID="lblMaxExpense" runat="server" Text="$0.00" />
                    </h3>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- LEFT COLUMN: Form -->
        <div class="col-lg-5 mb-4">
            <div class="card shadow-sm border-0">
                <div class="card-header bg-dark text-white fw-semibold">
                    <asp:Label ID="lblFormTitle" runat="server" Text="➕ Add New Expense" />
                </div>
                <div class="card-body p-4">

                    <asp:HiddenField ID="hfExpenseId" runat="server" Value="0" />

                    <!-- 🔥 Employee DropDown (Foreign Key Selection) -->
                    <div class="mb-3">
                        <label class="form-label fw-bold">Select Employee (Foreign Key)</label>
                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="-- Select Employee --" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvEmployee" runat="server"
                            ControlToValidate="ddlEmployee" InitialValue=""
                            ErrorMessage="Please select an employee."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Category Dropdown -->
                    <div class="mb-3">
                        <label class="form-label fw-bold">Category</label>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="-- Select Category --" />
                            <asp:ListItem Value="Travel" Text="Travel" />
                            <asp:ListItem Value="Meals" Text="Meals" />
                            <asp:ListItem Value="Software" Text="Software" />
                            <asp:ListItem Value="Office Supplies" Text="Office Supplies" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCategory" runat="server"
                            ControlToValidate="ddlCategory" InitialValue=""
                            ErrorMessage="Please select an expense category."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Amount -->
                    <div class="mb-3">
                        <label class="form-label fw-bold">Amount ($)</label>
                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" TextMode="Number"
                            step="0.01" placeholder="0.00" />
                        <asp:RequiredFieldValidator ID="rfvAmount" runat="server"
                            ControlToValidate="txtAmount"
                            ErrorMessage="Amount is required."
                            ForeColor="Red" Display="Dynamic" />
                        <asp:RangeValidator ID="rvAmount" runat="server"
                            ControlToValidate="txtAmount"
                            MinimumValue="1" MaximumValue="50000" Type="Double"
                            ErrorMessage="Amount must be between $1 and $50,000."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Expense Date -->
                    <div class="mb-3">
                        <label class="form-label fw-bold">Expense Date</label>
                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                            ControlToValidate="txtDate"
                            ErrorMessage="Expense Date is required."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Payment Method -->
                    <div class="mb-3">
                        <label class="form-label fw-bold d-block">Payment Method</label>
                        <asp:RadioButtonList ID="rblPaymentMethod" runat="server" RepeatDirection="Horizontal"
                            CssClass="d-flex gap-3">
                            <asp:ListItem Value="Corporate Card" Text="Corporate Card" Selected="True" />
                            <asp:ListItem Value="Cash" Text="Cash" />
                            <asp:ListItem Value="Personal Card" Text="Personal Card" />
                        </asp:RadioButtonList>
                    </div>

                    <!-- Receipt Checkbox -->
                    <div class="form-check mb-4">
                        <asp:CheckBox ID="chkHasReceipt" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label fw-semibold" for="<%= chkHasReceipt.ClientID %>">Receipt
                            Attached</label>
                    </div>

                    <!-- Buttons -->
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Expense"
                            CssClass="btn btn-primary flex-grow-1" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear"
                            CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnClear_Click" />
                    </div>

                </div>
            </div>
        </div>

        <!-- RIGHT COLUMN: GridView Table -->
        <!-- RIGHT COLUMN: GridView Table with Fixed 5-Row Height & Vertical Scroll -->
        <div class="col-lg-7">
            <div class="card shadow-sm border-0">
                <div class="card-header bg-dark text-white fw-semibold d-flex justify-content-between align-items-center">
                    <span>📋 Submitted Expenses</span>
                   
                </div>

                <!-- Search & Category Filter -->
                <div class="card-body bg-light border-bottom py-2">
                    <div class="row g-2 align-items-center">
                        <div class="col-md-5">
                            <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control form-control-sm"
                                placeholder="🔍 Search employee name..." />
                        </div>
                        <div class="col-md-4">
                            <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select form-select-sm">
                                <asp:ListItem Value="" Text="All Categories" />
                                <asp:ListItem Value="Travel" Text="Travel" />
                                <asp:ListItem Value="Meals" Text="Meals" />
                                <asp:ListItem Value="Software" Text="Software" />
                                <asp:ListItem Value="Office Supplies" Text="Office Supplies" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3 d-flex gap-1">
                            <asp:Button ID="btnSearch" runat="server" Text="Filter"
                                CssClass="btn btn-sm btn-dark flex-grow-1"
                                CausesValidation="false" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset"
                                CssClass="btn btn-sm btn-outline-secondary"
                                CausesValidation="false" OnClick="btnResetFilter_Click" />
                        </div>
                    </div>
                </div>

                <!-- 🔥 YEH CONTAINER ADD KIYA HAI: Max 5 rows visible + Overflow-Y Scroll -->
                <div class="card-body p-0" style="max-height: 470px; overflow-y: auto; overflow-x: hidden;">
                    <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="False"
                        DataKeyNames="ExpenseId"
                        CssClass="table table-hover table-striped align-middle mb-0"
                        OnRowDeleting="gvExpenses_RowDeleting"
                        OnRowCommand="gvExpenses_RowCommand">

                        <%-- Table Header ko Scroll karne par bhi Upar Fix (Sticky) rakhne ke liye --%>
                        <HeaderStyle CssClass="table-light sticky-top shadow-sm" />

                        <Columns>
                            <asp:BoundField DataField="ExpenseId" HeaderText="#" />

                            <%-- Employee Info with ID Badge --%>
                            <asp:TemplateField HeaderText="Employee">
                                <ItemTemplate>
                                    <span class="badge bg-secondary font-monospace me-1">E<%# Eval("EmployeeId", "{0:D3}") %></span>
                                    <strong><%# Eval("EmployeeName") %></strong>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Category" HeaderText="Category" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="ExpenseDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="PaymentMethod" HeaderText="Payment" />

                            <%-- Receipt Badge --%>
                            <asp:TemplateField HeaderText="Receipt">
                                <ItemTemplate>
                                    <span class='badge <%# (bool)Eval("HasReceipt") ? "bg-success" : "bg-warning text-dark" %>'>
                                        <%# (bool)Eval("HasReceipt") ? "Yes" : "No" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Actions (Edit + Delete) --%>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CommandName="EditRow"
                                        CommandArgument='<%# Eval("ExpenseId") %>'
                                        CssClass="btn btn-sm btn-outline-primary me-1"
                                        CausesValidation="false">
                                Edit
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnDelete" runat="server"
                                        CommandName="Delete"
                                        CssClass="btn btn-sm btn-outline-danger"
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Are you sure you want to delete this expense?');">
                                Delete
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <EmptyDataTemplate>
                            <div class="p-4 text-center text-muted">
                                No expenses found. Add an expense using the form!
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
