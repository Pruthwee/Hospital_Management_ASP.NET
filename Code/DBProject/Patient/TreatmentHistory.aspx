@page "/Patient/TreatmentHistory"
@model HospitalManagement.Pages.Patient.TreatmentHistoryModel
@{
    ViewData["Title"] = "Treatment History";
}

<h2>Treatment History</h2>

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
