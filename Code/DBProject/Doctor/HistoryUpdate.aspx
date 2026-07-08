@page "/Doctor/HistoryUpdate"
@model HospitalManagement.Pages.Doctor.HistoryUpdateModel
@{
    ViewData["Title"] = "Update History";
}

<h2>Update Patient History</h2>

<form method="post">
    <div class="form-group">
        <label>Patient ID:</label>
        <input asp-for="History.PatientId" class="form-control" />
    </div>
    <div class="form-group">
        <label>Diagnosis:</label>
        <input asp-for="History.Diagnosis" class="form-control" />
    </div>
    <div class="form-group">
        <label>Treatment:</label>
        <input asp-for="History.Treatment" class="form-control" />
    </div>
    <button type="submit" class="btn btn-primary">Update History</button>
</form>
