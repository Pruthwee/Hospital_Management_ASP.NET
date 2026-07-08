@page "/Patient/PatientNotifications"
@model HospitalManagement.Pages.Patient.PatientNotificationsModel
@{
    ViewData["Title"] = "Notifications";
}

<h2>Your Notifications</h2>

<ul class="list-group">
    @foreach (var note in Model.Notifications)
    {
        <li class="list-group-item">
            <strong>@note.Date.ToShortDateString():</strong> @note.Message
        </li>
    }
</ul>
