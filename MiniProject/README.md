# TechMart - ASP.NET Web Forms E-Commerce Mini Project

A modern, responsive Product Catalog and Shopping Cart web application built with **ASP.NET Web Forms (.NET Framework 4.8)**, C#, and Bootstrap 5.

---

## 🌟 Key Features

1. **Master Page (`Site.Master`)**:
   - Modern responsive navigation bar with dynamic cart count badge.
   - Quick category navigation (Audio, Laptops, Gaming, Wearables, etc.).
   - Announcement banner and trust assurance badges.
   - Professional dark-themed footer.

2. **Product Catalog (`Default.aspx`)**:
   - Live search by product name, description, or category.
   - Filter by categories with auto-postback.
   - Sort products by Featured, Price (Low to High / High to Low), Customer Rating, and Name.
   - Quick category pill buttons on the hero banner.
   - Product cards showing discount badge, stock indicator, ratings, formatted prices, and "Add to Cart" directly from the catalog.

3. **Product Details View (`ProductDetails.aspx`)**:
   - Detailed product view via query string (e.g. `ProductDetails.aspx?id=1`).
   - Breadcrumb navigation.
   - High-resolution product images, specifications, stock count, and price comparisons.
   - Quantity selector with dynamic stock validation.
   - Instant "Add to Cart" feedback alert and related products recommendation list.

4. **Interactive Shopping Cart (`Cart.aspx`)**:
   - Dynamic item list showing thumbnails, unit price, quantity increment (`+`) and decrement (`-`) controls, and line item subtotals.
   - Remove individual items or clear the entire cart.
   - Real-time calculations: Subtotal, Estimated Tax (8%), Shipping ($10 flat rate or **FREE** on orders over $100), and Grand Total.
   - Friendly empty-state banner with a "Start Shopping" button.

5. **Customer Checkout (`Checkout.aspx`)**:
   - Shipping address and contact information form.
   - ASP.NET Server Validation controls (`RequiredFieldValidator`, `RegularExpressionValidator` for email format).
   - Payment method selection (Credit/Debit Card, UPI / Net Banking, Cash on Delivery).
   - Order review summary panel.
   - Auto-redirect protection if checkout is accessed with an empty cart.

6. **Order Confirmation & Receipt (`OrderSuccess.aspx`)**:
   - Generates unique Order ID (e.g., `TM-842910`).
   - Detailed breakdown of ordered items, taxes, shipping fee, and grand total.
   - Delivery address and payment method display.
   - Built-in "Print Receipt" feature optimized with print-specific CSS.

---

## 📁 Project Structure

```
D:\All\.net\MiniProject\
├── Content\
│   └── site.css                 # Centralized CSS stylesheet for the entire site
├── Models\
│   ├── Product.cs               # Product data model
│   ├── CartItem.cs              # Cart line item model
│   ├── Cart.cs                  # Shopping cart business logic & calculations
│   ├── Order.cs                 # Customer order model
│   ├── ProductRepository.cs     # Catalog data provider & search/filter methods
│   └── CartSessionHelper.cs     # Session state helper for Cart and Order
├── Site.Master                  # Master Page with responsive layout & navbar
├── Site.Master.cs               # Master Page code-behind (cart badge updates)
├── Site.Master.designer.cs
├── Default.aspx                 # Home / Product Catalog
├── Default.aspx.cs
├── Default.aspx.designer.cs
├── ProductDetails.aspx          # Single product details & related items
├── ProductDetails.aspx.cs
├── ProductDetails.aspx.designer.cs
├── Cart.aspx                    # Shopping cart management
├── Cart.aspx.cs
├── Cart.aspx.designer.cs
├── Checkout.aspx                # Customer info & order placement form
├── Checkout.aspx.cs
├── Checkout.aspx.designer.cs
├── OrderSuccess.aspx            # Order receipt & confirmation
├── OrderSuccess.aspx.cs
├── OrderSuccess.aspx.designer.cs
├── Global.asax                  # Application lifecycle handlers
├── Web.config                   # ASP.NET configuration (Unobtrusive validation mode)
└── MiniProject.csproj           # Visual Studio Project File
```

---

## 🚀 How to Run

### Method 1: Using Visual Studio
1. Open Visual Studio.
2. Open the solution or project file `D:\All\.net\MiniProject\MiniProject.csproj` (or `MiniProject.slnx`).
3. Press **F5** or click the **IIS Express (Play)** button.
4. Your browser will launch `Default.aspx` automatically.

### Method 2: Command Line (IIS Express)
You can also launch it directly from PowerShell:
```powershell
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"D:\All\.net\MiniProject" /port:56147
```
Then open `http://localhost:56147/Default.aspx` in your browser.
