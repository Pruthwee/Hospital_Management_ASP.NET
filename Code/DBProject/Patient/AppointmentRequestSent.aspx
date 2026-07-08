@page "/Patient/AppointmentRequestSent"
@model HospitalManagement.Pages.Patient.AppointmentRequestSentModel
@{
    ViewData["Title"] = "Request Sent";
}

<h2>Appointment Request Sent</h2>
<p>Your request has been sent successfully. Please wait for the doctor's approval.</p>
<a asp-page="/Patient/PatientHome" class="btn btn-primary">Back to Home</a>
