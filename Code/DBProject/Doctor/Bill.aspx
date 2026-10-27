<%--
    NOTE: This Web Forms page has been superseded by Doctor/Bill.cshtml (ASP.NET Core Razor Pages).
    Migration: <%@ Page MasterPageFile="..." %> -> @page + Layout = "~/Doctor/_DoctorLayout.cshtml"
    Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    Migration: <asp:Label> -> @Model.BillAmount Razor expression
    Migration: <asp:Button OnClick="..."> -> <button type="submit" asp-page-handler="...">
    Migration: Session["key"] (System.Web) -> HttpContext.Session.GetInt32("key") (ASP.NET Core)
    Migration: Response.Redirect() -> RedirectToPage() in PageModel
    Migration: ViewState eliminated — stateless Razor Page model binding used instead
    This file is retained for reference only.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master" AutoEventWireup="true" CodeBehind="Bill.aspx.cs" Inherits="doctor.bill" %>
