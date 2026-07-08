@page "/Admin/AdminHome"
@model HospitalManagement.Pages.Admin.AdminHomeModel
@{
    ViewData["Title"] = "Admin Home";
}

<h1>Admin Home</h1>
<p>Welcome to the Hospital Management Admin Panel.</p>
<a asp-page="/Admin/AddStaff" class="btn btn-primary">Add Staff</a>
