@page "/Patient/ViewDoctors"
@model HospitalManagement.Pages.Patient.ViewDoctorsModel
@{
    ViewData["Title"] = "View Doctors";
}

<h2>Available Doctors</h2>

<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Specialization</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var doctor in Model.Doctors)
        {
            <tr>
                <td>@doctor.Name</td>
                <td>@doctor.Specialization</td>
                <td>
                    <a asp-page="/Patient/DoctorProfile" asp-route-doctorId="@doctor.DoctorId" class="btn btn-info">View Profile</a>
                </td>
            </tr>
        }
    </tbody>
</table>
