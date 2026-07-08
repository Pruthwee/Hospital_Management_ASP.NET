@page "/Doctor/PatientHistory"
@model HospitalManagement.Pages.Doctor.PatientHistoryModel
@{
    ViewData["Title"] = "Patient History";
}

<h2>Patient History</h2>

<table class="table">
    <thead>
        <tr>
            <th>Diagnosis</th>
            <th>Treatment</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var history in Model.Histories)
        {
            <tr>
                <td>@history.Diagnosis</td>
                <td>@history.Treatment</td>
            </tr>
        }
    </tbody>
</table>
