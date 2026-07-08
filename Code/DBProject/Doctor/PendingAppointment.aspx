@page "/Doctor/PendingAppointment"
@model HospitalManagement.Pages.Doctor.PendingAppointmentModel
@{
    ViewData["Title"] = "Pending Appointments";
}

<h2>Pending Appointments</h2>

<table class="table">
    <thead>
        <tr>
            <th>Patient ID</th>
            <th>Date</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var appt in Model.Appointments)
        {
            <tr>
                <td>@appt.PatientId</td>
                <td>@appt.Date</td>
                <td>
                    <form method="post" asp-route-id="@appt.AppointmentId">
                        <button type="submit" class="btn btn-success">Approve</button>
                    </form>
                </td>
            </tr>
        }
    </tbody>
</table>
