<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="MiniProject.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- Breadcrumb -->
        <nav aria-label="breadcrumb" class="mb-4">
            <ol class="breadcrumb">
                <li class="breadcrumb-item"><a href="Default.aspx">Home</a></li>
                <li class="breadcrumb-item"><a href="Cart.aspx">Cart</a></li>
                <li class="breadcrumb-item active" aria-current="page">Checkout</li>
            </ol>
        </nav>

        <h2 class="fw-extrabold text-dark mb-4">
            <i class="bi bi-credit-card-2-front me-2 text-primary"></i>Checkout & Shipping
        </h2>

        <div class="row g-4">
            <!-- Left: Checkout Form -->
            <div class="col-lg-8">
                <div class="checkout-card mb-4">
                    <h5 class="fw-bold text-dark mb-3">
                        <i class="bi bi-person-circle text-primary me-2"></i>Contact Information
                    </h5>

                    <div class="row g-3 mb-4">
                        <div class="col-md-6">
                            <label class="form-label fw-semibold text-dark">Full Name <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Placeholder="e.g. John Doe"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" 
                                ErrorMessage="Full name is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-semibold text-dark">Email Address <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" Placeholder="e.g. john@example.com"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                                ErrorMessage="Email is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" 
                                ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" 
                                ErrorMessage="Please enter a valid email address." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-semibold text-dark">Phone Number <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" Placeholder="e.g. +1 555-0199"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" 
                                ErrorMessage="Phone number is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>
                    </div>

                    <h5 class="fw-bold text-dark mb-3 pt-3 border-top">
                        <i class="bi bi-geo-alt-fill text-primary me-2"></i>Shipping Address
                    </h5>

                    <div class="row g-3 mb-4">
                        <div class="col-12">
                            <label class="form-label fw-semibold text-dark">Street Address <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" Placeholder="Apartment, suite, unit, building, street..."></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" 
                                ErrorMessage="Shipping address is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-semibold text-dark">City <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" Placeholder="e.g. New York"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" 
                                ErrorMessage="City is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-semibold text-dark">Zip / Postal Code <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtPostalCode" runat="server" CssClass="form-control" Placeholder="e.g. 10001"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPostal" runat="server" ControlToValidate="txtPostalCode" 
                                ErrorMessage="Postal code is required." Display="Dynamic" ForeColor="Red" CssClass="small mt-1 d-block" ValidationGroup="CheckoutGroup" />
                        </div>
                    </div>

                    <h5 class="fw-bold text-dark mb-3 pt-3 border-top">
                        <i class="bi bi-wallet2 text-primary me-2"></i>Payment Method
                    </h5>

                    <div class="p-3 bg-light rounded-3 mb-2">
                        <asp:RadioButtonList ID="rblPayment" runat="server" CssClass="payment-option-list">
                            <asp:ListItem Value="Credit/Debit Card" Selected="True"> Credit / Debit Card (Visa, MasterCard, Amex)</asp:ListItem>
                            <asp:ListItem Value="UPI / Net Banking"> UPI / Instant Net Banking</asp:ListItem>
                            <asp:ListItem Value="Cash on Delivery"> Cash on Delivery (COD)</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <!-- Right: Order Summary -->
            <div class="col-lg-4">
                <div class="summary-card">
                    <h5 class="fw-bold text-dark mb-3 pb-2 border-bottom">Review Items</h5>

                    <!-- Order items list -->
                    <div class="mb-4" style="max-height: 280px; overflow-y: auto;">
                        <asp:Repeater ID="rptCheckoutItems" runat="server">
                            <ItemTemplate>
                                <div class="d-flex align-items-center gap-3 mb-3 pb-3 border-bottom">
                                    <img src='<%# Eval("Product.ImageUrl") %>' alt='<%# Eval("Product.Name") %>' class="checkout-item-img" />
                                    <div class="flex-grow-1">
                                        <div class="fw-semibold text-dark small text-truncate" style="max-width: 170px;">
                                            <%# Eval("Product.Name") %>
                                        </div>
                                        <div class="text-muted small">Qty: <%# Eval("Quantity") %> &times; $<%# string.Format("{0:N2}", Eval("Product.Price")) %></div>
                                    </div>
                                    <div class="fw-bold text-dark small">
                                        $<%# string.Format("{0:N2}", Eval("Subtotal")) %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="d-flex justify-content-between mb-2 text-muted small">
                        <span>Subtotal</span>
                        <span class="text-dark fw-medium">$<asp:Label ID="lblSubtotal" runat="server"></asp:Label></span>
                    </div>

                    <div class="d-flex justify-content-between mb-2 text-muted small">
                        <span>Shipping</span>
                        <span class="text-dark fw-medium"><asp:Label ID="lblShipping" runat="server"></asp:Label></span>
                    </div>

                    <div class="d-flex justify-content-between mb-3 text-muted small">
                        <span>Estimated Tax (8%)</span>
                        <span class="text-dark fw-medium">$<asp:Label ID="lblTax" runat="server"></asp:Label></span>
                    </div>

                    <div class="border-top pt-3 mb-4">
                        <div class="d-flex justify-content-between align-items-center">
                            <span class="fs-5 fw-bold text-dark">Total Due</span>
                            <span class="fs-4 fw-extrabold text-primary">$<asp:Label ID="lblGrandTotal" runat="server"></asp:Label></span>
                        </div>
                    </div>

                    <asp:Button ID="btnPlaceOrder" runat="server" Text="Confirm & Place Order" 
                        ValidationGroup="CheckoutGroup" OnClick="btnPlaceOrder_Click" 
                        CssClass="btn btn-primary btn-lg w-100 fw-bold shadow-sm mb-3" />

                    <div class="text-center small text-muted">
                        <i class="bi bi-shield-lock-fill text-success me-1"></i> Safe & Encrypted Transaction
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
