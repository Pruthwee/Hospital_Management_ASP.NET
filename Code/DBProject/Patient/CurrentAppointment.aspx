<%-- NOTE: This file has been superseded by CurrentAppointment.cshtml (ASP.NET Core Razor Pages).
     Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
     Migration: MasterPageFile -> Layout = "~/Patient/_PatientLayout.cshtml"
     Migration: <asp:Label> (Appointment, ADoctor, ATimings) -> model properties rendered in Razor view
     Migration: EnableViewState="False" -> Stateless by default in Razor Pages
     ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master" AutoEventWireup="true" CodeBehind="CurrentAppointment.aspx.cs" Inherits="DBProject.CurrentAppointment" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Current Appointment</title>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <h1><strong style="margin:30%">Current Appointments</strong></h1>
    <br /><br />

    <div style="margin-left: 70px">

    <asp:Label ID="Appointment" runat="server"  Font-Bold="true" Font-Size="Medium"></asp:Label>
    <br /><br />

    <asp:Label ID="ADoctor" runat="server"  Font-Bold="true" Font-Size="Medium"></asp:Label>
    <br /><br />

    <asp:Label ID="ATimings" runat="server"  Font-Bold="true" Font-Size="Medium"></asp:Label>
    <br /><br />

    </div>

</asp:Content>
