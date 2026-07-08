@page "/Patient/BillsHistory"
@model HospitalManagement.Pages.Patient.BillsHistoryModel
@{
    ViewData["Title"] = "Bills History";
}

<h2>Bills History</h2>

<table class="table">
    <thead>
        <tr>
            <th>Amount</th>
            <th>Date</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var bill in Model.Bills)
        {
            <tr>
                <td>@bill.Amount</td>
                <td>@bill.Date</td>
            </tr>
        }
    </tbody>
</table>
