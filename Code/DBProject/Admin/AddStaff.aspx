@page "/Admin/AddStaff"
@model HospitalManagement.Pages.Admin.AddStaffModel
@{
    ViewData["Title"] = "Add Staff";
}

<h2>Add Staff</h2>

<form method="post">
    <div>
        <label>Name:</label>
        <input asp-for="Staff.Name" class="form-control" />
    </div>
    <div>
        <label>Role:</label>
        <input asp-for="Staff.Role" class="form-control" />
    </div>
    <div>
        <label>Email:</label>
        <input asp-for="Staff.Email" class="form-control" />
    </div>
    <button type="submit" class="btn btn-primary">Add Staff</button>
</form>
