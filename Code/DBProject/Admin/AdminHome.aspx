<%--
    NOTE: This Web Forms page has been superseded by Admin/AdminHome.cshtml (ASP.NET Core Razor Pages).
    Migration: <%@ Page MasterPageFile="..." %> -> @page + Layout = "~/Admin/_AdminLayout.cshtml"
    Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    Migration: <asp:GridView> -> HTML table rendered from PageModel DataTable properties
    Migration: <asp:Label> -> @Model.PropertyName Razor expression
    Migration: ViewState eliminated — stateless Razor Page model binding used instead
    Migration: Postback pattern eliminated — HTTP GET handler (OnGet) used instead
    This file is retained for reference only.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminHome.aspx.cs" Inherits="DBProject.AdminHome" %>
