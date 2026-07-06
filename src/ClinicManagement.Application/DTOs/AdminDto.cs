namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for department data.</summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>DTO for staff data.</summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public double? Salary { get; set; }
}

/// <summary>DTO for adding a new staff member.</summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>DTO for admin home page statistics.</summary>
public class AdminHomeDto
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}
