@page "/Doctor/PreviousHistory"
@model HospitalManagement.Pages.Doctor.PreviousHistoryModel
@{
    ViewData["Title"] = "Previous History";
}

<h2>Previous Patient History</h2>

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
