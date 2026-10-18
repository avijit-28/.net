<%@ Page Title="Expenses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Expenses.aspx.cs" Inherits="EmployeeExpenseTracker.Expenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="row mb-4">
        <div class="col-12">
            <h2 class="fw-bold text-secondary">Expense Dashboard & Entry</h2>
            <hr />
        </div>
    </div>

    <!-- Phase 4: Validation Summary Block (Errors yahan list honge) -->
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" 
        HeaderText="Please correct the following errors:" DisplayMode="BulletList" />

    <!-- Summary Dashboard Cards -->
    <div class="row g-3 mb-4">
        <div class="col-md-4">
            <div class="card shadow-sm border-primary">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase mb-1">Total Expenses Claimed</h6>
                    <h3 class="fw-bold text-primary mb-0">
                        <asp:Label ID="lblTotalAmount" runat="server" Text="$0.00" />
                    </h3>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-success">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase mb-1">Total Claims Count</h6>
                    <h3 class="fw-bold text-success mb-0">
                        <asp:Label ID="lblTotalCount" runat="server" Text="0" />
                    </h3>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-warning">
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
        <!-- LEFT COLUMN: Phase 3 Expense Entry Form -->
        <div class="col-lg-5 mb-4">
            <div class="card shadow-sm">
                <div class="card-header bg-dark text-white fw-semibold">
                    ➕ Add New Expense
                </div>
                <div class="card-body">

                    <!-- Employee Name -->
                    <div class="mb-3">
                        <label class="form-label fw-bold">Employee Name</label>
                        <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" placeholder="e.g. Alex Rivera" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                            ControlToValidate="txtEmployeeName" 
                            ErrorMessage="Employee Name is required." 
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
                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00" />
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
                        <asp:RadioButtonList ID="rblPaymentMethod" runat="server" RepeatDirection="Horizontal" CssClass="d-flex gap-3">
                            <asp:ListItem Value="Corporate Card" Text="Corporate Card" Selected="True" />
                            <asp:ListItem Value="Cash" Text="Cash" />
                            <asp:ListItem Value="Personal Card" Text="Personal Card" />
                        </asp:RadioButtonList>
                    </div>

                    <!-- Receipt Checkbox -->
                    <div class="form-check mb-4">
                        <asp:CheckBox ID="chkHasReceipt" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label fw-semibold" for="<%= chkHasReceipt.ClientID %>">Receipt Attached</label>
                    </div>

                    <!-- Action Buttons -->
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Expense" 
                            CssClass="btn btn-primary flex-grow-1" OnClick="btnSubmit_Click" />
                        
                        <!-- CausesValidation='false' zaroori hai taki Clear dabane par validation error na aaye -->
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnClear_Click" />
                    </div>

                </div>
            </div>
        </div>

        <!-- RIGHT COLUMN: Phase 6 GridView Data Table -->
        <div class="col-lg-7">
            <div class="card shadow-sm">
                <div class="card-header bg-dark text-white fw-semibold">
                    📋 Submitted Expenses
                </div>
                <div class="card-body p-0">
                    <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="False" 
                        DataKeyNames="Id" 
                        CssClass="table table-hover table-striped align-middle mb-0" 
                        OnRowDeleting="gvExpenses_RowDeleting" >
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="#" />
                            <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                            <asp:BoundField DataField="Category" HeaderText="Category" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="ExpenseDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="PaymentMethod" HeaderText="Payment" />
                            
                            <%-- Badge for Receipt --%>
                            <asp:TemplateField HeaderText="Receipt">
                                <ItemTemplate>
                                    <span class='badge <%# (bool)Eval("HasReceipt") ? "bg-success" : "bg-warning text-dark" %>'>
                                        <%# (bool)Eval("HasReceipt") ? "Yes" : "No" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Actions (Delete Button) --%>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
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
                                No expenses found. Add your first expense using the form!
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>