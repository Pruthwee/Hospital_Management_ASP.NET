@page "/SignUp"
@model HospitalManagement.Pages.SignUpModel
@{
    ViewData["Title"] = "Sign Up";
}

<h2>Create Account</h2>

<form method="post">
    <div class="form-group">
        <label>Username:</label>
        <input asp-for="User.Username" class="form-control" />
    </div>
    <div class="form-group">
        <label>Password:</label>
        <input asp-for="User.Password" class="form-control" type="password" />
    </div>
    <div class="form-group">
        <label>Role:</label>
        <select asp-for="User.Role" class="form-control">
            <option value="Patient">Patient</option>
            <option value="Doctor">Doctor</option>
            <option value="Admin">Admin</option>
        </select>
    </div>
    <button type="submit" class="btn btn-primary">Sign Up</button>
</form>
