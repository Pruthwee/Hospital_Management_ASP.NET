@page "/Doctor/DoctorHome"
@model HospitalManagement.Pages.Doctor.DoctorHomeModel
@{
    ViewData["Title"] = "Doctor Home";
}

<h1>Doctor Home</h1>
<p>Welcome to the Doctor's Portal.</p>
<a asp-page="/Doctor/Bill" class="btn btn-primary">Create Bill</a>
