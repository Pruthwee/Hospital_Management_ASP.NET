@page "/Admin/ManageClinic"
@model HospitalManagement.Pages.Admin.ManageClinicModel
@{
    ViewData["Title"] = "Manage Clinic";
}

<h2>Manage Clinic</h2>

<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Location</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var clinic in Model.Clinics)
        {
            <tr>
                <td>@clinic.Name</td>
                <td>@clinic.Location</td>
                <td>
                    <form method="post" asp-route-id="@clinic.ClinicId">
                        <button type="submit" class="btn btn-danger">Delete</button>
                    </form>
                </td>
            </tr>
        }
    </tbody>
</table>
