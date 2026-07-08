@page "/Patient/PatientHome"
@model HospitalManagement.Pages.Patient.PatientHomeModel
@{
    ViewData["Title"] = "Patient Home";
}

<h1>Patient Home</h1>
<p>Welcome to the Patient Portal.</p>
<a asp-page="/Patient/AppointmentTaker" class="btn btn-primary">Take Appointment</a>
