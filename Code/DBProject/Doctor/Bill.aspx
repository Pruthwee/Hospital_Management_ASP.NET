@page "/Doctor/Bill"
@model HospitalManagement.Pages.Doctor.BillModel
@{
    ViewData["Title"] = "Create Bill";
}

<h2>Create Bill</h2>

<form method="post">
    <div class="form-group">
        <label>Patient ID:</label>
        <input asp-for="Bill.PatientId" class="form-control" />
    </div>
    <div class="form-group">
        <label>Amount:</label>
        <input asp-for="Bill.Amount" class="form-control" />
    </div>
    <div class="form-group">
        <label>Date:</label>
        <input asp-for="Bill.Date" class="form-control" type="date" />
    </div>
    <button type="submit" class="btn btn-primary">Save Bill</button>
</form>
