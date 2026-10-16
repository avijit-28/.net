<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="WebApplication4.AllGridProp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GridView Topics (Pages 10 - 30)</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f6f9;
            margin: 40px 20px;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            background: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            padding: 30px;
        }
        h1 {
            color: #2c3e50;
            margin-top: 0;
            font-size: 24px;
            border-bottom: 2px solid #3498db;
            padding-bottom: 10px;
        }
        p {
            color: #666;
            font-size: 14px;
            line-height: 1.6;
        }
        .topic-list {
            list-style: none;
            padding: 0;
            margin: 25px 0 0 0;
        }
        .topic-item {
            border: 1px solid #e1e8ed;
            border-radius: 6px;
            margin-bottom: 14px;
            padding: 16px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            transition: all 0.2s ease;
        }
        .topic-item:hover {
            border-color: #3498db;
            background-color: #f8fafc;
            transform: translateX(4px);
        }
        .topic-info h3 {
            margin: 0 0 5px 0;
            font-size: 16px;
            color: #2c3e50;
        }
        .topic-info p {
            margin: 0;
            font-size: 13px;
            color: #7f8c8d;
        }
        .slide-tag {
            display: inline-block;
            background: #ebf5fb;
            color: #2980b9;
            font-size: 11px;
            font-weight: bold;
            padding: 2px 8px;
            border-radius: 4px;
            margin-bottom: 4px;
        }
        .btn-open {
            background-color: #3498db;
            color: #ffffff;
            text-decoration: none;
            padding: 8px 18px;
            border-radius: 4px;
            font-size: 13px;
            font-weight: bold;
            white-space: nowrap;
        }
        .btn-open:hover {
            background-color: #2980b9;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>GridView Topics (Slides 10 &ndash; 30)</h1>
            <p>Each topic from the presentation is designed into its own simple, focused <code>.aspx</code> file. Click on any topic below to open and execute that specific file:</p>

            <ul class="topic-list">
                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 10 &ndash; 13</span>
                        <h3>1. Multi-Culture Currency Formatting per Row</h3>
                        <p>Format currency dynamically in <code>RowDataBound</code> based on Country ($, &pound;, &#8377;, R, RM).</p>
                    </div>
                    <a href="GridMultiCulture.aspx" class="btn-open">Open Page &rarr;</a>
                </li>

                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 14 &ndash; 16</span>
                        <h3>2. Hidden Culture Column Formatting</h3>
                        <p>Format currency using a <code>CountryCulture</code> column and hide the column in Header and DataRow.</p>
                    </div>
                    <a href="GridHiddenCulture.aspx" class="btn-open">Open Page &rarr;</a>
                </li>

                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 17 &ndash; 19</span>
                        <h3>3. Design-Time Styles &amp; Conditional Formatting</h3>
                        <p>Set design-time styles and highlight rows with <code>AnnualSalary &gt; 70,000</code> in Red at runtime.</p>
                    </div>
                    <a href="GridConditionalFormatting.aspx" class="btn-open">Open Page &rarr;</a>
                </li>

                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 20 &ndash; 24</span>
                        <h3>4. TemplateField with DropDownList for Editing</h3>
                        <p>Convert column to TemplateField and use DropDownList in <code>EditItemTemplate</code> bound to Gender.</p>
                    </div>
                    <a href="GridTemplateField.aspx" class="btn-open">Open Page &rarr;</a>
                </li>

                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 25 &ndash; 27</span>
                        <h3>5. Validation Controls &amp; ValidationSummary</h3>
                        <p>Add RequiredFieldValidator with <code>InitialValue="Select Gender"</code> and ValidationSummary.</p>
                    </div>
                    <a href="GridValidation.aspx" class="btn-open">Open Page &rarr;</a>
                </li>

                <li class="topic-item">
                    <div class="topic-info">
                        <span class="slide-tag">Slides 27 &ndash; 38</span>
                        <h3>6. Inline Insert, Update, and Delete with Footer</h3>
                        <p>Complete CRUD using <code>ShowFooter="True"</code>, <code>lbInsert_Click</code>, and ValidationGroups.</p>
                    </div>
                    <a href="GridInsertUpdateDelete.aspx" class="btn-open">Open Page &rarr;</a>
                </li>
            </ul>
        </div>
    </form>
</body>
</html>
