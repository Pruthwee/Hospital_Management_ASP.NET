@page "/Patient/DoctorProfile"
@model HospitalManagement.Pages.Patient.DoctorProfileModel
@{
    ViewData["Title"] = "Doctor Profile";
}

<h2>Doctor Profile</h2>

@if (Model.Doctor != null)
{
    <p>Name: @Model.Doctor.Name</p>
    <p>Specialization: @Model.Doctor.Specialization</p>
    <p>Email: @Model.Doctor.Email</p>
}
else
{
    <p>Doctor not found.</p>
}
