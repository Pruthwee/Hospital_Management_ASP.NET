@page "/Patient/CurrentAppointment"
@model HospitalManagement.Pages.Patient.CurrentAppointmentModel
@{
    ViewData["Title"] = "Current Appointment";
}

<h2>Current Appointment</h2>

@if (Model.Appointment != null)
{
    <p>Your next appointment is on: @Model.Appointment.Date</p>
}
else
{
    <p>No approved appointment found.</p>
}
