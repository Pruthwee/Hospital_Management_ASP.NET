@page "/Patient/AppointmentTaker"
@model HospitalManagement.Pages.Patient.AppointmentTakerModel
@{
    ViewData["Title"] = "Take Appointment";
}

<h2>Take Appointment</h2>

<form method="post">
    <div class="form-group">
        <label>Doctor ID:</label>
        <input asp-for="Request.DoctorId" class="form-control" />
    </div>
    <div class="form-group">
        <label>Date:</label>
        <input asp-for="Request.Date" class="form-control" type="date" />
    </div>
    <button type="submit" class="btn btn-primary">Request Appointment</button>
</form>
