<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UserDetailsWithoutObjectSource.aspx.cs"
    Inherits="AddSearch.UserDetailsWithoutObjectSource" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Management - Custom Paged</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
        :root {
            --primary: #2563eb;
            --primary-hover: #1d4ed8;
            --emerald: #059669;
            --emerald-hover: #047857;
            --rose: #e11d48;
            --amber: #d97706;
            --slate-50: #f8fafc;
            --slate-100: #f1f5f9;
            --slate-200: #e2e8f0;
            --slate-600: #475569;
            --slate-700: #334155;
            --slate-800: #1e293b;
            --slate-900: #0f172a;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Plus Jakarta Sans', system-ui, -apple-system, sans-serif;
            background: linear-gradient(135deg, #f0f4ff 0%, #f8fafc 50%, #eff6ff 100%);
            color: var(--slate-800);
            min-height: 100vh;
            padding: 30px 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .header-card {
            background: #ffffff;
            border-radius: 16px;
            padding: 24px 30px;
            margin-bottom: 24px;
            box-shadow: 0 4px 20px -2px rgba(0, 0, 0, 0.05);
            border: 1px solid var(--slate-200);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .header-title h1 {
            font-size: 24px;
            font-weight: 800;
            color: var(--slate-900);
            letter-spacing: -0.02em;
        }

        .header-title p {
            color: var(--slate-600);
            font-size: 13.5px;
            margin-top: 4px;
        }

        .table-card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px -4px rgba(0, 0, 0, 0.06);
            border: 1px solid var(--slate-200);
            overflow: hidden;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        .gridview-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

        .gridview-table th {
            background: var(--slate-100);
            color: var(--slate-700);
            font-weight: 700;
            text-transform: uppercase;
            font-size: 11.5px;
            letter-spacing: 0.05em;
            padding: 14px 16px;
            text-align: left;
            border-bottom: 2px solid var(--slate-200);
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 13px 16px;
            border-bottom: 1px solid var(--slate-200);
            vertical-align: middle;
        }

        .gridview-table tr:hover:not(.gridview-footer) {
            background: #f8fafc;
        }

        .gridview-footer {
            background: #f0fdf4 !important;
            border-top: 2px solid #86efac;
        }

        .gridview-footer td {
            padding: 14px 16px;
        }

        .form-input {
            width: 100%;
            padding: 8px 12px;
            border: 1.5px solid var(--slate-200);
            border-radius: 8px;
            font-size: 13px;
            font-family: inherit;
            color: var(--slate-800);
            background: #ffffff;
            transition: all 0.2s;
        }

        .form-input:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
            outline: none;
        }

        .actions-cell {
            white-space: nowrap;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-action-edit {
            padding: 6px 12px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 12.5px;
            text-decoration: none;
            background: #eff6ff;
            color: var(--primary);
            border: 1px solid #bfdbfe;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-action-edit:hover {
            background: var(--primary);
            color: #ffffff;
        }

        .btn-action-delete {
            padding: 6px 12px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 12.5px;
            text-decoration: none;
            background: #fff1f2;
            color: var(--rose);
            border: 1px solid #fecdd3;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-action-delete:hover {
            background: var(--rose);
            color: #ffffff;
        }

        .btn-action-update {
            padding: 6px 12px;
            border-radius: 6px;
            font-weight: 700;
            font-size: 12.5px;
            text-decoration: none;
            background: #ecfdf5;
            color: var(--emerald);
            border: 1px solid #a7f3d0;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-action-update:hover {
            background: var(--emerald);
            color: #ffffff;
        }

        .btn-action-cancel {
            padding: 6px 12px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 12.5px;
            text-decoration: none;
            background: #f1f5f9;
            color: var(--slate-600);
            border: 1px solid var(--slate-200);
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-action-cancel:hover {
            background: var(--slate-200);
            color: var(--slate-800);
        }

        .btn-insert {
            padding: 8px 16px;
            border-radius: 8px;
            font-weight: 700;
            font-size: 13px;
            text-decoration: none;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff;
            border: none;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            box-shadow: 0 2px 6px rgba(16, 185, 129, 0.25);
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-insert:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            box-shadow: 0 4px 10px rgba(16, 185, 129, 0.35);
        }

        .pager-container {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 16px;
            background: #fafafa;
            border-top: 1px solid var(--slate-200);
        }

        .pager-btn {
            padding: 6px 12px;
            border-radius: 6px;
            text-decoration: none;
            font-weight: 700;
            font-size: 13px;
            border: 1px solid var(--slate-200);
            background: #ffffff;
            color: var(--slate-700);
            transition: all 0.2s;
        }

        .pager-btn:hover {
            border-color: var(--primary);
            color: var(--primary);
            background: #eff6ff;
        }

        .pager-btn-active {
            background: var(--primary) !important;
            color: #ffffff !important;
            border-color: var(--primary) !important;
            cursor: default;
        }

        /* Centered Custom UI Modal Popup */
        .modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(15, 23, 42, 0.65);
            backdrop-filter: blur(4px);
            z-index: 999999;
            display: flex;
            align-items: center;
            justify-content: center;
            opacity: 0;
            pointer-events: none;
            transition: opacity 0.25s cubic-bezier(0.16, 1, 0.3, 1);
        }

        .modal-overlay.active {
            opacity: 1;
            pointer-events: auto;
        }

        .modal-box {
            background: #ffffff;
            border-radius: 20px;
            width: 90%;
            max-width: 440px;
            padding: 28px 24px;
            text-align: center;
            box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.3);
            border: 1px solid var(--slate-200);
            transform: scale(0.92) translateY(12px);
            transition: transform 0.25s cubic-bezier(0.16, 1, 0.3, 1);
        }

        .modal-overlay.active .modal-box {
            transform: scale(1) translateY(0);
        }

        .modal-icon-wrap {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            margin: 0 auto 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 26px;
        }

        .modal-icon-wrap.type-delete {
            background: #ffe4e6;
            color: #e11d48;
            box-shadow: 0 0 0 6px #fff1f2;
        }

        .modal-icon-wrap.type-update {
            background: #eff6ff;
            color: #2563eb;
            box-shadow: 0 0 0 6px #f0f7ff;
        }

        .modal-icon-wrap.type-insert {
            background: #dcfce7;
            color: #15803d;
            box-shadow: 0 0 0 6px #ecfdf5;
        }

        .modal-title {
            font-size: 19px;
            font-weight: 800;
            color: var(--slate-900);
            margin-bottom: 8px;
        }

        .modal-msg {
            font-size: 13.5px;
            color: var(--slate-600);
            line-height: 1.5;
            margin-bottom: 24px;
        }

        .modal-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
        }

        .modal-btn {
            padding: 10px 20px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 13.5px;
            cursor: pointer;
            border: none;
            transition: all 0.2s;
            flex: 1;
        }

        .modal-btn-cancel {
            background: #f1f5f9;
            color: #475569;
            border: 1px solid #cbd5e1;
        }

        .modal-btn-cancel:hover {
            background: #e2e8f0;
        }

        .modal-btn-confirm-delete {
            background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(239, 68, 68, 0.35);
        }

        .modal-btn-confirm-delete:hover {
            background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
        }

        .modal-btn-confirm-update {
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.35);
        }

        .modal-btn-confirm-update:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
        }

        .modal-btn-confirm-insert {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.35);
        }

        .modal-btn-confirm-insert:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            
            <div class="header-card">
                <div class="header-title">
                    <h1>User Management (Custom Server-Side Paging)</h1>
                    <p>Direct SQL access without ObjectDataSource &bull; Stays on current page after Insert, Update, or Delete</p>
                </div>
            </div>

            <div class="table-card">
                <div class="table-responsive">
                    <asp:GridView ID="GridView1"
                        runat="server"
                        AutoGenerateColumns="False"
                        DataKeyNames="UserId"
                        ShowFooter="True"
                        OnRowCommand="GridView1_RowCommand" 
                        AllowPaging="False"
                        GridLines="None"
                        CssClass="gridview-table">
                        <FooterStyle CssClass="gridview-footer" />
                        
                        <Columns>
                            <%-- User ID --%>
                            <asp:TemplateField HeaderText="User ID" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label ID="lblUserId" runat="server" Text='<%# Eval("UserId") %>' Font-Bold="True" ForeColor="#475569"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtUserId" runat="server" CssClass="form-input" placeholder="ID (or blank)"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- User Name --%>
                            <asp:TemplateField HeaderText="User Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("UserName") %>' Font-Bold="True"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtUserName" runat="server" Text='<%# Eval("UserName") %>' CssClass="form-input"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtUserNameFooter" runat="server" CssClass="form-input" placeholder="Full Name *"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Password --%>
                            <asp:TemplateField HeaderText="Password">
                                <ItemTemplate>
                                    <span>&bull;&bull;&bull;&bull;&bull;&bull;</span>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtPassword" runat="server" Text='<%# Eval("Password") %>' CssClass="form-input"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtPasswordFooter" runat="server" CssClass="form-input" placeholder="Password"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Email --%>
                            <asp:TemplateField HeaderText="Email Address">
                                <ItemTemplate>
                                    <%# Eval("Email") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEmail" runat="server" Text='<%# Eval("Email") %>' CssClass="form-input"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtEmailFooter" runat="server" CssClass="form-input" placeholder="email@domain.com"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Mobile No --%>
                            <asp:TemplateField HeaderText="Mobile">
                                <ItemTemplate>
                                    <%# Eval("MobileNo") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMobileNo" runat="server" Text='<%# Eval("MobileNo") %>' CssClass="form-input"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtMobileNoFooter" runat="server" CssClass="form-input" placeholder="10-digit Mobile"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- City --%>
                            <asp:TemplateField HeaderText="City">
                                <ItemTemplate>
                                    <%# Eval("City") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtCity" runat="server" Text='<%# Eval("City") %>' CssClass="form-input"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtCityFooter" runat="server" CssClass="form-input" placeholder="City"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Actions --%>
                            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="180px">
                                <ItemTemplate>
                                    <div class="actions-cell">
                                        <asp:LinkButton ID="lbEdit" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-edit" CausesValidation="false">
                                            &#9998; Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lbDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-delete" CausesValidation="false">
                                            &#128465; Delete
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <div class="actions-cell">
                                        <asp:LinkButton ID="lbUpdate" runat="server" CommandName="UpdateRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-update">
                                            &#10004; Save
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lbCancel" runat="server" CommandName="CancelUpdate" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-cancel" CausesValidation="false">
                                            &#10006; Cancel
                                        </asp:LinkButton>
                                    </div>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lbInsert" runat="server" CommandName="InsertRow" CssClass="btn-insert">
                                        &#10010; Add User
                                    </asp:LinkButton>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="pager-container">
                    <span style="font-size:12px;font-weight:700;color:#64748b;margin-right:8px;">PAGE:</span>
                    <asp:Repeater ID="repeaterPaging" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="pagingLinkButton" runat="server"
                                Text='<%# Eval("Text") %>'
                                CommandArgument='<%# Eval("Value") %>'
                                CssClass='<%# Convert.ToBoolean(Eval("Enabled")) ? "pager-btn" : "pager-btn pager-btn-active" %>'
                                OnClick="linkButton_Click">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>

        <!-- Centered UI Confirmation Modal -->
        <div id="confirmModalOverlay" class="modal-overlay">
            <div class="modal-box">
                <div id="modalIconWrap" class="modal-icon-wrap type-delete">&#128465;</div>
                <h3 id="modalTitle" class="modal-title">Confirm Action</h3>
                <p id="modalMsg" class="modal-msg">Are you sure you want to perform this action?</p>
                <div class="modal-actions">
                    <button type="button" id="btnModalCancel" class="modal-btn modal-btn-cancel">Cancel</button>
                    <button type="button" id="btnModalConfirm" class="modal-btn modal-btn-confirm-delete">Confirm</button>
                </div>
            </div>
        </div>

        <script>
            (function () {
                var overlay = document.getElementById('confirmModalOverlay');
                var iconWrap = document.getElementById('modalIconWrap');
                var titleElem = document.getElementById('modalTitle');
                var msgElem = document.getElementById('modalMsg');
                var btnCancel = document.getElementById('btnModalCancel');
                var btnConfirm = document.getElementById('btnModalConfirm');

                var pendingAction = null;

                function showModal(opts) {
                    titleElem.textContent = opts.title;
                    msgElem.innerHTML = opts.message;
                    btnConfirm.textContent = opts.confirmText;

                    iconWrap.className = 'modal-icon-wrap type-' + opts.type;
                    if (opts.type === 'delete') {
                        iconWrap.innerHTML = '&#128465;';
                        btnConfirm.className = 'modal-btn modal-btn-confirm-delete';
                    } else if (opts.type === 'update') {
                        iconWrap.innerHTML = '&#9998;';
                        btnConfirm.className = 'modal-btn modal-btn-confirm-update';
                    } else {
                        iconWrap.innerHTML = '&#10010;';
                        btnConfirm.className = 'modal-btn modal-btn-confirm-insert';
                    }

                    pendingAction = opts.onConfirm;
                    overlay.classList.add('active');
                }

                function closeModal() {
                    overlay.classList.remove('active');
                    pendingAction = null;
                }

                btnCancel.addEventListener('click', closeModal);
                overlay.addEventListener('click', function (e) {
                    if (e.target === overlay) closeModal();
                });

                btnConfirm.addEventListener('click', function () {
                    var action = pendingAction;
                    closeModal();
                    if (typeof action === 'function') action();
                });

                // Global capture-phase click interceptor strictly for Insert, Update, and Delete
                document.addEventListener('click', function (e) {
                    var target = e.target.closest('a, button, input[type="submit"]');
                    if (!target) return;

                    if (target._modalConfirmed) {
                        delete target._modalConfirmed;
                        return; // Let postback execute naturally
                    }

                    if (target.closest('#confirmModalOverlay')) return;

                    var text = (target.textContent || target.value || '').trim();
                    var textLower = text.toLowerCase();
                    var href = target.getAttribute('href') || '';
                    var idAttr = (target.getAttribute('id') || '').toLowerCase();

                    // STRICT EXCLUSIONS: Pager buttons, Edit button, Cancel button
                    if (target.classList.contains('pager-btn') ||
                        target.closest('.pager-container') ||
                        idAttr.indexOf('paginglinkbutton') > -1 ||
                        /^\d+$/.test(text)) {
                        return;
                    }

                    if (target.classList.contains('btn-action-edit') || textLower === 'edit' || textLower.indexOf('edit') > -1) {
                        return;
                    }

                    if (target.classList.contains('btn-action-cancel') || textLower === 'cancel' || textLower.indexOf('cancel') > -1) {
                        return;
                    }

                    // STRICT INCLUSIONS: ONLY Delete, Update, and Insert
                    var isDelete = target.classList.contains('btn-action-delete') || textLower.indexOf('delete') > -1 || href.indexOf('DeleteRow') > -1;
                    var isUpdate = target.classList.contains('btn-action-update') || textLower.indexOf('save') > -1 || textLower.indexOf('update') > -1 || href.indexOf('UpdateRow') > -1;
                    var isInsert = target.classList.contains('btn-insert') || textLower.indexOf('add user') > -1 || textLower.indexOf('insert') > -1 || href.indexOf('InsertRow') > -1;

                    if (!isDelete && !isUpdate && !isInsert) {
                        return;
                    }

                    e.preventDefault();
                    e.stopPropagation();
                    e.stopImmediatePropagation();

                    var type = isDelete ? 'delete' : (isUpdate ? 'update' : 'insert');
                    var title = isDelete ? 'Confirm Deletion' : (isUpdate ? 'Confirm Update' : 'Confirm Insert');
                    var confirmBtnText = isDelete ? 'Yes, Delete' : (isUpdate ? 'Yes, Save Changes' : 'Yes, Add User');
                    var message = '';

                    if (isDelete) {
                        message = 'Are you sure you want to <strong>DELETE</strong> this user?<br/><span style="font-size:12px;color:#ef4444;margin-top:6px;display:inline-block;">This action is permanent and will remove the record from SQL Server.</span>';
                    } else if (isUpdate) {
                        message = 'Are you sure you want to <strong>SAVE changes</strong> to this user in SQL Server?';
                    } else {
                        message = 'Are you sure you want to <strong>INSERT</strong> this new user into SQL Server?';
                    }

                    showModal({
                        title: title,
                        message: message,
                        type: type,
                        confirmText: confirmBtnText,
                        onConfirm: function () {
                            target._modalConfirmed = true;
                            if (typeof target.click === 'function') {
                                target.click();
                            } else if (target.tagName === 'A' && href && href.startsWith('javascript:')) {
                                var script = href.substring(11);
                                (new Function(script))();
                            }
                        }
                    });
                }, true);
            })();
        </script>
    </form>
</body>
</html>