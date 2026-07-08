@page "/Admin/DoctorRegistrationForm"
@model HospitalManagement.Pages.Admin.DoctorRegistrationFormModel
@{
    ViewData["Title"] = "Doctor Registration";
}

<h2>Doctor Registration</h2>

<form method="post">
    <div class="form-group">
        <label>Name:</label>
        <input asp-for="Doctor.Name" class="form-control" />
    </div>
    <div class="form-group">
        <label>Specialization:</label>
        <input asp-for="Doctor.Specialization" class="form-control" />
    </div>
    <div class="form-group">
        <label>Email:</label>
        <input asp-for="Doctor.Email" class="form-control" />
    </div>
    <button type="submit" class="btn btn-primary">Register Doctor</button>
</form>
