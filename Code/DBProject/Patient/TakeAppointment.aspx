<%-- NOTE: This file has been superseded by TakeAppointment.cshtml (ASP.NET Core Razor Pages).
     Migration: <%@ Page %> directive -> @page / @model directives in Razor Pages
     Migration: MasterPageFile -> Layout = "~/Patient/_PatientLayout.cshtml"
     Migration: <asp:Label> (TDept) -> model property rendered in Razor view
     Migration: <asp:GridView> (TDeptGrid) with OnRowCommand -> HTML table with async form POST handler
     Migration: EnableViewState="False" -> Stateless by default in Razor Pages
     Migration: AutoGenerateSelectButton -> HTML form button per row
     Migration: TDeptGrid.DataBind() (synchronous) -> OnGetAsync() / OnPostSelectDepartmentAsync() (async/await)
     ViewState and postback patterns eliminated; stateless horizontal scaling enabled for Azure Container Apps.

     cr-dotnet-1034: Synchronous Data Binding in GridView Controls
     Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
       <asp:GridView ID="TDeptGrid"> with synchronous DataBind() replaced by HTML table
       rendered from DataTable loaded via async DAL method (getdeptInfoAsync) in OnGetAsync(),
       preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master" AutoEventWireup="true" CodeBehind="TakeAppointment.aspx.cs" Inherits="DBProject.TakeAppointment" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

            <title>Departments</title>


</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <!------------------Styling------------------>
    <link rel="stylesheet" href="/assets/css/grid-view.css"/>


    <h1><strong style="margin:23%">Select a Department to view its Doctors</strong></h1>
    <br /><br />

    <asp:Label ID="TDept" runat="server"></asp:Label>
    <br /><br />

    <asp:GridView ID="TDeptGrid" runat="server" class = "GridView-d" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="1000px"
        EnableViewState ="False"
        AutoGenerateSelectButton="True" 
        OnRowCommand="TDeptGrid_RowCommand" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px">
     
        

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
