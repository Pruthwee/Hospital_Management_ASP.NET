<%-- NOTE: This file has been superseded by HistoryUpdate.cshtml (ASP.NET Core Razor Pages).
     Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
     Migration: MasterPageFile -> Layout = "~/Doctor/_DoctorLayout.cshtml"
     Migration: <asp:Content> -> @RenderBody() / @RenderSection() in Razor layout
     Migration: <asp:TextBox> server controls -> HTML <input> elements with model binding
     Migration: <asp:Button OnClick="..."> -> <button asp-page-handler="..."> Tag Helpers
     ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master" AutoEventWireup="true" CodeBehind="HistoryUpdate.aspx.cs" Inherits="doctor.Historyupdate" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<title>Update History</title>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Cp1" runat="server">

<h1>Update history  </h1>

<h4>Disease:</h4><asp:TextBox ID="Disease" runat="server"></asp:TextBox>
   <h4>Progress:</h4> <asp:TextBox ID="progress" runat="server"></asp:TextBox>
    <h4>Prescription</h4><asp:TextBox ID="Prescription" runat="server"></asp:TextBox>
    <br />
      <br />
      <br />

    <asp:Button ID="submit" runat="server" Text="Accept & Save" Onclick="saveindatabase" Font-Bold="true" />  
    <asp:Button ID="Bill" runat="server" Text="Generate Bill" OnClick="generate_bill" Font-Bold ="true" />

</asp:Content>
