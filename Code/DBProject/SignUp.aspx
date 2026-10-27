<%--
    NOTE: This file has been superseded by SignUp.cshtml (ASP.NET Core Razor Pages).
    The Web Forms markup below is retained for reference only.
    Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
    Migration: <form runat="server"> -> <form method="post"> with Tag Helpers
    Migration: <asp:TextBox> server controls -> <input> HTML elements with asp-for Tag Helpers
    Migration: <asp:Button> server controls -> <button type="submit"> with asp-page-handler
    Migration: OnClientClick="return validateL();" -> client-side validation retained in Razor view
    Migration: Session["idoriginal"] -> HttpContext.Session.SetInt32("idoriginal", id)
    Migration: Response.Redirect("~/Patient/PatientHome.aspx") -> RedirectToPage("/Patient/PatientHome")
    Migration: Response.Write("<script>alert(...)") -> ErrorMessage model property rendered in view
    Migration: IsPostBack check -> OnGet() / OnPost() / OnPostLogin() / OnPostSignup() separation
    ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.
--%>
<%--
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="DBProject.SignUp" %>
--%>
