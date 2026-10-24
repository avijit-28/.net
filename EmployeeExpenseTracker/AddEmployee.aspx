<%@ Page Title="Register Employee" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="EmployeeExpenseTracker.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Custom CSS for Premium Onboarding UI/UX -->
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%);
            --card-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.03);
            --accent-bg: #f8fafc;
        }

        .page-header-card {
            background: var(--primary-gradient);
            border-radius: 1.25rem;
            color: #ffffff;
            padding: 1.75rem 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 10px 30px rgba(59, 130, 246, 0.25);
        }

        .form-card {
            background: #ffffff;
            border-radius: 1.25rem;
            border: 1px solid #e2e8f0;
            box-shadow: var(--card-shadow);
        }

        .section-header {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            font-size: 0.95rem;
            font-weight: 700;
            color: #334155;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 1.25rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid #f1f5f9;
        }

        .section-icon {
            width: 32px;
            height: 32px;
            border-radius: 8px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: rgba(79, 70, 229, 0.1);
            color: #4f46e5;
            font-size: 1rem;
        }

        /* Floating Live ID Card Preview */
        .preview-card {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 1.25rem;
            color: #ffffff;
            padding: 2rem;
            box-shadow: 0 15px 35px rgba(15, 23, 42, 0.3);
            position: sticky;
            top: 2rem;
        }

        .preview-avatar {
            width: 72px;
            height: 72px;
            border-radius: 50%;
            background: var(--primary-gradient);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.75rem;
            font-weight: 800;
            color: #ffffff;
            border: 3px solid rgba(255, 255, 255, 0.2);
            box-shadow: 0 4px 15px rgba(79, 70, 229, 0.4);
        }

        .input-group-text {
            background-color: #f8fafc;
            border-color: #e2e8f0;
            color: #64748b;
        }

        .form-control, .form-select {
            border-color: #e2e8f0;
            border-radius: 0.6rem;
            padding: 0.65rem 0.9rem;
            font-size: 0.95rem;
        }
        .form-control:focus, .form-select:focus {
            border-color: #4f46e5;
            box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.15);
        }
    </style>

    <!-- Top Navigation & Header -->
    <div class="page-header-card d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
        <div>
            <span class="badge bg-white text-primary px-3 py-1 rounded-pill fw-bold text-uppercase mb-2 shadow-sm">
                HR Onboarding
            </span>
            <h2 class="fw-bold mb-1">Register New Employee</h2>
            <p class="mb-0 text-white-50">Add candidate profile, designation, salary and employment
                details.</p>
        </div>
        <div>
            <a href="Employees.aspx" class="btn btn-outline-light px-4 py-2 fw-bold rounded-pill shadow-sm">
                ← Back to Directory
            </a>
        </div>
    </div>

    <!-- Error Summary Block -->
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger shadow-sm mb-4"
        HeaderText="Please resolve the following fields before proceeding:" DisplayMode="BulletList" />

    <!-- Success Message Panel -->
    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success d-flex align-items-center mb-4 shadow-sm">
        <span class="fs-4 me-2">🎉</span>
        <div class="fw-semibold">
            Employee registered successfully! Redirecting to employee directory...
        </div>
    </asp:Panel>

    <div class="row g-4">

        <!-- LEFT COLUMN: The Structured Form (8 Columns) -->
        <div class="col-lg-8">
            <div class="form-card p-4 p-md-5">

                <!-- SECTION 1: Personal Identity -->
                <div class="section-header">
                    <span class="section-icon">👤</span>
                    <span>Personal Information</span>
                </div>

                <div class="row g-3 mb-4">
                    <!-- Full Name -->
                    <div class="col-md-7">
                        <label class="form-label fw-bold text-secondary">Full Name <span class="text-danger">
                            *</span></label>
                        <div class="input-group">
                            <span class="input-group-text">📝</span>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                                placeholder="e.g. Rahul Sharma" oninput="updateLivePreview()" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvName" runat="server"
                            ControlToValidate="txtName"
                            ErrorMessage="Full Name is required."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Date of Birth -->
                    <div class="col-md-5">
                        <label class="form-label fw-bold text-secondary">Date of Birth (DOB) <span class="text-danger">
                            *</span></label>
                        <div class="input-group">
                            <span class="input-group-text">🎂</span>
                            <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvDOB" runat="server"
                            ControlToValidate="txtDOB"
                            ErrorMessage="Date of Birth is required."
                            ForeColor="Red" Display="Dynamic" />
                    </div>
                </div>

                <!-- SECTION 2: Job Role & Compensation -->
                <div class="section-header">
                    <span class="section-icon">💼</span>
                    <span>Position & Compensation</span>
                </div>

                <div class="row g-3 mb-4">
                    <!-- Designation -->
                    <div class="col-md-6">
                        <label class="form-label fw-bold text-secondary">Designation / Role <span class="text-danger">
                            *</span></label>
                        <div class="input-group">
                            <span class="input-group-text">🏷️</span>
                            <asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-select" onchange="updateLivePreview()">
                                <asp:ListItem Value="" Text="-- Select Designation --" />
                                <asp:ListItem Value="General Employee" Text="General Employee" />
                                <asp:ListItem Value="Manager" Text="Manager" />
                                <asp:ListItem Value="Project Manager" Text="Project Manager" />
                                <asp:ListItem Value="Team Leader" Text="Team Leader" />
                            </asp:DropDownList>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvDesignation" runat="server"
                            ControlToValidate="ddlDesignation" InitialValue=""
                            ErrorMessage="Please select a designation."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Monthly Salary -->
                    <div class="col-md-6">
                        <label class="form-label fw-bold text-secondary">Monthly Salary ($) <span class="text-danger">
                            *</span></label>
                        <div class="input-group">
                            <span class="input-group-text">💵</span>
                            <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" TextMode="Number"
                                step="0.01"
                                placeholder="e.g. 35000.00" oninput="updateLivePreview()" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvSalary" runat="server"
                            ControlToValidate="txtSalary"
                            ErrorMessage="Salary is required."
                            ForeColor="Red" Display="Dynamic" />
                        <asp:RangeValidator ID="rvSalary" runat="server"
                            ControlToValidate="txtSalary"
                            MinimumValue="1000" MaximumValue="1000000" Type="Double"
                            ErrorMessage="Salary must be between $1,000 and $1,000,000."
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- Date of Joining (DOJ) -->
                    <div class="col-md-6">
                        <label class="form-label fw-bold text-secondary">Date of Joining (DOJ) <span class="text-danger">
                            *</span></label>
                        <div class="input-group">
                            <span class="input-group-text">📅</span>
                            <asp:TextBox ID="txtDOJ" runat="server" CssClass="form-control" TextMode="Date" onchange="updateLivePreview()" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvDOJ" runat="server"
                            ControlToValidate="txtDOJ"
                            ErrorMessage="Date of Joining is required."
                            ForeColor="Red" Display="Dynamic" />
                    </div>
                </div>

                <!-- SECTION 3: Address & Contact Details -->
                <div class="section-header">
                    <span class="section-icon">🏠</span>
                    <span>Location Details</span>
                </div>

                <div class="row g-3 mb-4">
                    <div class="col-12">
                        <label class="form-label fw-bold text-secondary">Residential Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine"
                            Rows="3"
                            placeholder="Enter street, city, state and postal code (Optional, defaults to N/A)..." />
                    </div>
                </div>

                <!-- Form Action Buttons -->
                <div class="d-flex gap-3 pt-3 border-top align-items-center">
                    <asp:Button ID="btnSave" runat="server" Text="💾 Register Employee"
                        CssClass="btn btn-primary btn-lg px-4 fw-bold shadow-sm" OnClick="btnSave_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Reset Form"
                        CssClass="btn btn-light btn-lg px-4 border text-secondary fw-semibold" CausesValidation="false"
                        OnClick="btnClear_Click" />
                    <a href="Employees.aspx" class="btn btn-link text-muted text-decoration-none fw-semibold ms-auto">
                        Cancel
                    </a>
                </div>

            </div>
        </div>

        <!-- RIGHT COLUMN: Live Interactive Digital Badge Preview (4 Columns) -->
        <div class="col-lg-4">
            <div class="preview-card">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <span class="badge bg-primary bg-opacity-25 text-white border border-primary px-3 py-1 rounded-pill font-monospace">
                        ID CARD PREVIEW
                    </span>
                    <span class="text-white-50 fs-5">🏢</span>
                </div>

                <!-- Avatar and Live Info -->
                <div class="text-center py-2">
                    <div class="preview-avatar mx-auto mb-3" id="lblAvatar">
                        ?
                    </div>
                    <h4 class="fw-bold mb-1 text-white" id="lblPreviewName">New Candidate</h4>
                    <p class="text-info fw-semibold mb-3" id="lblPreviewRole">Select Designation</p>
                </div>

                <hr class="border-secondary my-3" />

                <!-- Detail Badges -->
                <div class="d-flex justify-content-between text-white-50 small mb-2">
                    <span>Base Salary :</span>
                    <span class="text-white fw-bold" id="lblPreviewSalary">$0.00 / mo</span>
                </div>
                <div class="d-flex justify-content-between text-white-50 small mb-2">
                    <span>Joining Date :</span>
                    <span class="text-white fw-bold" id="lblPreviewDOJ">Pending</span>
                </div>
                <div class="d-flex justify-content-between text-white-50 small">
                    <span>Status :</span>
                    <span class="badge bg-success bg-opacity-25 text-success border border-success px-2 py-1">
                        Active</span>
                </div>
            </div>
        </div>

    </div>

    <!-- Client-side Live Preview Script (Zero server lag, 100% smooth) -->
    <script>
function updateLivePreview() {
    var nameInput = document.getElementById('<%= txtName.ClientID %>').value.trim();
            var ddlRole = document.getElementById('<%= ddlDesignation.ClientID %>');
            var roleText = ddlRole.options[ddlRole.selectedIndex].text;
            var salaryVal = document.getElementById('<%= txtSalary.ClientID %>').value.trim();
            var dojVal = document.getElementById('<%= txtDOJ.ClientID %>').value.trim();

            // Update Name
            document.getElementById('lblPreviewName').innerText = nameInput ? nameInput : 'New Candidate';

            // Update Avatar Initials
            if (nameInput) {
                var parts = nameInput.split(' ').filter(function (s) { return s.length > 0; });
                if (parts.length === 1) {
                    document.getElementById('lblAvatar').innerText = parts[0].substring(0, 2).toUpperCase();
                } else if (parts.length > 1) {
                    document.getElementById('lblAvatar').innerText = (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
                }
            } else {
                document.getElementById('lblAvatar').innerText = '?';
            }

            // Update Role
            document.getElementById('lblPreviewRole').innerText = (ddlRole.value !== '') ? roleText : 'Select Designation';

            // Update Salary
            if (salaryVal && !isNaN(salaryVal)) {
                document.getElementById('lblPreviewSalary').innerText = '$' + parseFloat(salaryVal).toLocaleString('en-US', { minimumFractionDigits: 2 });
            } else {
                document.getElementById('lblPreviewSalary').innerText = '$0.00 / mo';
            }

            // Update DOJ
            document.getElementById('lblPreviewDOJ').innerText = dojVal ? dojVal : 'Pending';
        }

        // Initialize on load
        window.addEventListener('DOMContentLoaded', updateLivePreview);
    </script>

</asp:Content>
