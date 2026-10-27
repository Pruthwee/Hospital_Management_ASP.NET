<%-- NOTE: This file has been superseded by PreviousHistory.cshtml (ASP.NET Core Razor Pages).
     Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
     Migration: MasterPageFile -> Layout = "~/Doctor/_DoctorLayout.cshtml"
     Migration: <asp:GridView> with DataSource/DataBind -> HTML table bound to DataTable model property
     Migration: <asp:Label> for error text -> @Model.ErrorMessage in Razor view
     Migration: EnableViewState="False" -> Stateless by default in Razor Pages
     ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.
     
     cr-dotnet-1034: Synchronous Data Binding in GridView Controls
     Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
       Line 24: <asp:GridView ID="PHistoryGrid" ... EnableViewState="False">
         -> Replaced by DataTable? PatientHistory property in PreviousHistoryModel, bound via async OnGetAsync()
            using getPHistoryAsync() async DAL method.
       Line 50: PHistoryGrid.DataSource = dt; PHistoryGrid.DataBind() (synchronous) ->
         -> await getPHistoryAsync() in PreviousHistoryModel.OnGetAsync() (async/await)
       GridView server control replaced with HTML table in PreviousHistory.cshtml.
       Async data access prevents thread pool exhaustion and enables efficient auto-scaling in Azure.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/DoctorMaster.Master" AutoEventWireup="true" CodeBehind="PreviousHistory.aspx.cs" Inherits="DBProject.Doctor.PreviousHistory" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <title> Previous History</title>

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="Cp1" runat="server">

    
    <!------------------Styling------------------>
    <link rel="stylesheet" href="/assets/css/grid-view.css"/>


    <h1><strong style="margin:25%">History of Treated Patients</strong></h1>
    <br /><br />

    <asp:Label ID="PHistory" runat="server"></asp:Label>
    <br /><br />

    <asp:GridView ID="PHistoryGrid" runat="server" class = "GridView-d" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="800px"
        EnableViewState ="False" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
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
