<%@ Page Title="Order Confirmed" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderSuccess.aspx.cs" Inherits="MiniProject.OrderSuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-3">
        <!-- Order Not Found Panel -->
        <asp:Panel ID="pnlNoOrder" runat="server" Visible="false" CssClass="text-center py-5">
            <i class="bi bi-receipt text-muted" style="font-size: 4rem;"></i>
            <h3 class="fw-bold mt-3 text-dark">No Active Order Found</h3>
            <p class="text-muted">You do not have an active order session right now.</p>
            <a href="Default.aspx" class="btn btn-primary mt-2">Go to Product Catalog</a>
        </asp:Panel>

        <!-- Order Success Details Panel -->
        <asp:Panel ID="pnlOrderDetails" runat="server">
            <div class="row justify-content-center">
                <div class="col-lg-9">
                    <div class="success-card">
                        <!-- Top Header -->
                        <div class="text-center mb-4 pb-3 border-bottom">
                            <div class="success-icon-circle">
                                <i class="bi bi-check-lg"></i>
                            </div>
                            <h2 class="fw-extrabold text-dark mb-1">Thank You For Your Order!</h2>
                            <p class="text-muted mb-3">
                                Your order has been placed successfully and is being prepared for shipment.
                            </p>
                            <span class="badge bg-primary fs-6 px-4 py-2 rounded-pill">
                                Order ID: <asp:Label ID="lblOrderId" runat="server"></asp:Label>
                            </span>
                        </div>

                        <!-- Customer & Delivery Meta -->
                        <div class="row g-4 mb-4">
                            <div class="col-sm-6">
                                <div class="p-3 bg-light rounded-3 h-100">
                                    <h6 class="fw-bold text-dark mb-2"><i class="bi bi-geo-alt text-primary me-2"></i>Shipping Details</h6>
                                    <p class="mb-1 fw-semibold text-dark"><asp:Label ID="lblCustomerName" runat="server"></asp:Label></p>
                                    <p class="mb-1 text-muted small"><asp:Label ID="lblShippingAddress" runat="server"></asp:Label></p>
                                    <p class="mb-1 text-muted small"><asp:Label ID="lblCityZip" runat="server"></asp:Label></p>
                                    <p class="mb-0 text-muted small"><i class="bi bi-telephone me-1"></i><asp:Label ID="lblPhone" runat="server"></asp:Label></p>
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="p-3 bg-light rounded-3 h-100">
                                    <h6 class="fw-bold text-dark mb-2"><i class="bi bi-info-circle text-primary me-2"></i>Order Information</h6>
                                    <p class="mb-1 text-muted small"><strong>Date:</strong> <asp:Label ID="lblOrderDate" runat="server"></asp:Label></p>
                                    <p class="mb-1 text-muted small"><strong>Email:</strong> <asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                                    <p class="mb-1 text-muted small"><strong>Payment Method:</strong> <asp:Label ID="lblPaymentMethod" runat="server"></asp:Label></p>
                                    <p class="mb-0 text-muted small"><strong>Status:</strong> <span class="badge bg-success-subtle text-success">Confirmed</span></p>
                                </div>
                            </div>
                        </div>

                        <!-- Ordered Items Table -->
                        <h5 class="fw-bold text-dark mb-3">Ordered Items</h5>
                        <div class="table-responsive mb-4">
                            <table class="table receipt-table align-middle">
                                <thead>
                                    <tr>
                                        <th>Product</th>
                                        <th class="text-center">Price</th>
                                        <th class="text-center">Quantity</th>
                                        <th class="text-end">Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptOrderItems" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <div class="fw-bold text-dark"><%# Eval("Product.Name") %></div>
                                                    <div class="small text-muted"><%# Eval("Product.Category") %></div>
                                                </td>
                                                <td class="text-center">$<%# string.Format("{0:N2}", Eval("Product.Price")) %></td>
                                                <td class="text-center"><%# Eval("Quantity") %></td>
                                                <td class="text-end fw-semibold text-dark">$<%# string.Format("{0:N2}", Eval("Subtotal")) %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>

                        <!-- Financial Totals -->
                        <div class="row justify-content-end mb-4">
                            <div class="col-md-5">
                                <div class="d-flex justify-content-between mb-2 text-muted small">
                                    <span>Subtotal:</span>
                                    <span class="text-dark fw-medium">$<asp:Label ID="lblSubtotal" runat="server"></asp:Label></span>
                                </div>
                                <div class="d-flex justify-content-between mb-2 text-muted small">
                                    <span>Shipping Fee:</span>
                                    <span class="text-dark fw-medium"><asp:Label ID="lblShipping" runat="server"></asp:Label></span>
                                </div>
                                <div class="d-flex justify-content-between mb-3 text-muted small">
                                    <span>Estimated Tax:</span>
                                    <span class="text-dark fw-medium">$<asp:Label ID="lblTax" runat="server"></asp:Label></span>
                                </div>
                                <div class="d-flex justify-content-between pt-2 border-top">
                                    <span class="fw-bold text-dark fs-5">Total Paid:</span>
                                    <span class="fw-extrabold text-primary fs-5">$<asp:Label ID="lblGrandTotal" runat="server"></asp:Label></span>
                                </div>
                            </div>
                        </div>

                        <!-- Actions Bar -->
                        <div class="d-flex flex-wrap justify-content-between align-items-center pt-3 border-top gap-2 btn-no-print">
                            <a href="Default.aspx" class="btn btn-outline-secondary">
                                <i class="bi bi-arrow-left me-1"></i> Continue Shopping
                            </a>
                            <button type="button" class="btn btn-primary" onclick="window.print();">
                                <i class="bi bi-printer me-1"></i> Print Receipt
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
