<%-- NOTE: This file has been superseded by AppointmentTaker.cshtml (ASP.NET Core Razor Pages).
     Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
     Migration: MasterPageFile -> Layout = "~/Patient/_PatientLayout.cshtml"
     Migration: <asp:GridView> with AutoGenerateSelectButton / OnRowCommand -> HTML table with named POST handler
     Migration: <asp:Label> (PAppointment) for status text -> @Model.StatusMessage in Razor view
     Migration: EnableViewState="False" -> Stateless by default in Razor Pages
     ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.
     
     cr-dotnet-1034: Synchronous Data Binding in GridView Controls
     Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
       Line 27: <asp:GridView ID="PAppointmentGrid" ... AutoGenerateSelectButton="true" OnRowCommand="PAppointmentGrid_RowCommand">
         -> Replaced by DataTable? FreeSlots property in AppointmentTakerModel, bound via async OnGetAsync()
            using getFreeSlotsAsync() async DAL method.
       Line 55: PAppointmentGrid.DataSource = dt; PAppointmentGrid.DataBind() (synchronous) ->
         -> await getFreeSlotsAsync() in AppointmentTakerModel.OnGetAsync() (async/await)
       GridView server control replaced with HTML table in AppointmentTaker.cshtml.
       Async data access prevents thread pool exhaustion and enables efficient auto-scaling in Azure.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master" AutoEventWireup="true" CodeBehind="AppointmentTaker.aspx.cs" Inherits="DBProject.AppointmentTaker" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Appointment Taker</title>

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!------------------Styling------------------>
    <link rel="stylesheet" href="/assets/css/grid-view.css"/>


    <h1><strong style="margin:37%">Free Time Slots</strong></h1>
    <br /><br />

    <asp:Label ID="PAppointment" runat="server"></asp:Label>
    <br /><br />

    <asp:GridView ID="PAppointmentGrid" runat="server" class = "GridView-d" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="800px"
         AutoGenerateSelectButton="true" OnRowCommand="PAppointmentGrid_RowCommand" AutoGenerateColumns ="true"  EnableViewState ="False" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
        >

        <AlternatingRowStyle BackColor="White" />
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
          
        <Columns>
            <asp:TemplateField HeaderText = "No." ItemStyle-Width="50">
                <ItemTemplate>
                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                </ItemTemplate>

                <ItemStyle Width="50px"></ItemStyle>
            </asp:TemplateField>

        
        </Columns>


    </asp:GridView>

    <br /><br />

</asp:Content>
