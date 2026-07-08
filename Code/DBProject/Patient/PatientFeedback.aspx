@page "/Patient/PatientFeedback"
@model HospitalManagement.Pages.Patient.PatientFeedbackModel
@{
    ViewData["Title"] = "Patient Feedback";
}

<h2>Patient Feedback</h2>

<form method="post">
    <div class="form-group">
        <label>Comments:</label>
        <textarea asp-for="Feedback.Comments" class="form-control"></textarea>
    </div>
    <div class="form-group">
        <label>Rating (1-5):</label>
        <input asp-for="Feedback.Rating" class="form-control" type="number" min="1" max="5" />
    </div>
    <button type="submit" class="btn btn-primary">Submit Feedback</button>
</form>
