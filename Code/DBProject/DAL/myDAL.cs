using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using Dapper;

 
namespace DBProject.DAL
{
	//Database Layer of 3 tier architecture
	// Cloud-ready: Uses Dapper with connection factory pattern compatible with Amazon RDS Proxy.
	// Connection string is read from environment variable DB_CONNECTION_STRING (injected at runtime),
	// falling back to Web.config for local development.
	public class myDAL
    {
		// Cloud-ready: Connection string sourced from environment variable for AWS deployment.
		// RDS Proxy endpoint should be set via DB_CONNECTION_STRING environment variable.
        private static string GetConnectionString()
        {
            // Prefer environment variable (set by AWS ECS/EKS task definition or Elastic Beanstalk)
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
            {
                return envConnStr;
            }
            // Fallback to Web.config / app config for local development
            return System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon1"].ConnectionString;
        }

        // Factory method: creates a new SqlConnection for use with Dapper (RDS Proxy compatible).
        // RDS Proxy manages connection pooling at the infrastructure level.
        private static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }




		//-----------------------------------------------------------------------------------//
		//																					 //
		//									SIGNUP											 //
		//																					 //
		//-----------------------------------------------------------------------------------//



		/*CHECKS WHETHER IT IS A VALID USER AND RETURN ITS TYPE*/
		public int validateLogin (string Email, string Password, ref int type , ref int id)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@email", Email, DbType.String, size: 30);
                    parameters.Add("@password", Password, DbType.String, size: 20);
                    parameters.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@type", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute("Login", parameters, commandType: CommandType.StoredProcedure);

                    int status = parameters.Get<int>("@status");
                    type = parameters.Get<int>("@type");
                    id = parameters.Get<int>("@ID");

                    return status;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
        }

        




		/*THIS FUNCTION WILL VALIDATE ALL THE INFORMAIION OF OF USER (PATIENT)*/
        public int validateUser (string Name, string BirthDate, string Email , string Password , string PhoneNo , string gender , string Address, ref int id)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@name", Name, DbType.String, size: 20);
                    parameters.Add("@address", Address, DbType.String, size: 40);
                    parameters.Add("@gender", gender, DbType.String, size: 1);
                    parameters.Add("@date", BirthDate, DbType.Date);
                    parameters.Add("@email", Email, DbType.String, size: 30);
                    parameters.Add("@password", Password, DbType.String, size: 20);
                    parameters.Add("@phone", PhoneNo, DbType.StringFixedLength, size: 15);
                    parameters.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute("PatientSignup", parameters, commandType: CommandType.StoredProcedure);

                    int status = parameters.Get<int>("@status");
                    if (status != 0)
                    {
                        id = parameters.Get<int>("@ID");
                    }
                    return status;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
        }







        //-----------------------------------------------------------------------------------//
        //                                                                                   //
        //                                       ADMIN                                       //
        //                                                                                   //
        //-----------------------------------------------------------------------------------//



        /*THIS FUNCTION CHECKS WHEATHER EMAIL OF A DOCTOR ALREADY EXISTS IN THE DATABASE */

        public int DoctorEmailAlreadyExist(string Email)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Email", Email, DbType.String, size: 30);
                parameters.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute("CheckDoctorEmail", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@status");
            }
        }







        /*THIS FUNCTION WILL ADD THE DOCTOR TO THE DATA BASE */
        public void AddDoctor(string Name, string Email, string Password, string BirthDate, int dept, string Phone, char gender, string Address, int exp, int salary, int Charges_per_visit, string spec, string qual)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Name", Name, DbType.String, size: 30);
                parameters.Add("@Email", Email, DbType.String, size: 30);
                parameters.Add("@Password", Password, DbType.String, size: 30);
                parameters.Add("@BirthDate", BirthDate, DbType.Date);
                parameters.Add("@dept", dept, DbType.Int32);
                parameters.Add("@gender", gender.ToString(), DbType.String, size: 1);
                parameters.Add("@Address", Address, DbType.String, size: 30);
                parameters.Add("@Exp", exp, DbType.Int32);
                parameters.Add("@Salary", salary, DbType.Int32);
                parameters.Add("@charges", Charges_per_visit, DbType.Int32);
                parameters.Add("@phone", Phone, DbType.String, size: 30);
                parameters.Add("@spec", spec, DbType.String, size: 30);
                parameters.Add("@qual", qual, DbType.String, size: 30);

                con.Execute("AddDoctor", parameters, commandType: CommandType.StoredProcedure);
            }
        }





        /*THIS FUNCTION WILL ADD STAFF TO THE DATA BASE*/
        public int AddStaff(string Name, string BirthDate, string Phone, char gender, string Address, int salary, string Qual, string Designation)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Name", Name, DbType.String, size: 30);
                    parameters.Add("@BirthDate", BirthDate, DbType.Date);
                    parameters.Add("@Phone", Phone, DbType.String, size: 30);
                    parameters.Add("@gender", gender.ToString(), DbType.String, size: 1);
                    parameters.Add("@salary", salary, DbType.Int32);
                    parameters.Add("@Designation", Designation, DbType.String, size: 30);
                    parameters.Add("@Qualification", Qual, DbType.String, size: 1);
                    parameters.Add("@Address", Address, DbType.String, size: 50);

                    con.Execute("AddStaff", parameters, commandType: CommandType.StoredProcedure);
                    return 1;
                }
                catch
                {
                    return -1;
                }
            }
        }







        /*THIS FUNCTION WILL RUN MULTIPLE QUERIES AND GET ALL THE INFORMATION NEEDED TO DISPLAY AT ADMIN HOME*/
        public void GetAdminHomeInformation(ref DataTable[] arrTable)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Total_Patient", con);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(arrTable[0]);

                cmd.CommandText = "SELECT * FROM Total_Doctors";
                Adapter.Fill(arrTable[1]);

                cmd.CommandText = "SELECT * FROM Income";
                Adapter.Fill(arrTable[2]);

                cmd.CommandText = "SELECT * FROM Department_View";
                Adapter.Fill(arrTable[3]);

                cmd.CommandText = "SELECT * FROM Appointment_view";
                Adapter.Fill(arrTable[4]);
            }
        }






        /*THIS FUNCTION IS INTENDED TO DELETE DOCTOR BUT SECRETLY IT ONLY UPDATE THE STATUS*/
        public int DeleteDoctor(int id)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", id, DbType.Int32);
                    con.Execute("DeleteDoctor", parameters, commandType: CommandType.StoredProcedure);
                    return 1;
                }
                catch
                {
                    return -1;
                }
            }
        }



        /*THIS FUNCTION WILL DELLETE STAFF FROM THE DOCTOR */
        public int DeleteStaff(int id)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", id, DbType.Int32);
                    con.Execute("DELETESTAFF", parameters, commandType: CommandType.StoredProcedure);
                    return 1;
                }
                catch
                {
                    return -1;
                }
            }
        }


        /*LOADS THE TABLE OF DOCTOR / SPECIFIED DOCTORS ON THE BASIS OF SEARCH QUERY*/
        public void LoadDoctor(ref DataTable table, String SearchQuery)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                SqlCommand cmd;
                if (SearchQuery == "")
                {
                    cmd = new SqlCommand(
                    "SELECT Doctor.DoctorID as ID , Doctor.Name , D.DeptName as Department FROM Doctor JOIN Department D ON D.DeptNo = Doctor.DeptNo" +
                    " WHERE Doctor.Status = 1",
                    con);
                }
                else
                {
                    cmd = new SqlCommand(
                    "SELECT a.DoctorID as ID,  a.Name, D.DeptName as Department FROM department D join (SELECT * FROM Doctor WHERE Doctor.Status = 1 AND Doctor.Name like  '%' + @DName + '%')  a ON a.DeptNo = D.DeptNo",
                    con);
                    cmd.Parameters.AddWithValue("@DName", SearchQuery);
                }

                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(table);
            }
        }






        /*LOADS THE TABLE OF PATIENT ON THE BASIS OF SEARCH QUERY*/
        /*FOR EMPTY QUERY RETURN ALL INFORMATION OTHERWISE RETURN ONLY REQUIRED TUPLE*/
        public void LoadPatient(ref DataTable table, String SearchQuery)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                SqlCommand cmd;
                if (SearchQuery == "")
                {
                    cmd = new SqlCommand("SELECT * FROM PATIENT_VIEW", con);
                }
                else
                {
                    cmd = new SqlCommand("SELECT Patient.PatientID, Patient.Name, Patient.Phone from Patient" +
                    " WHERE patient.name like '%' + @SName + '%' ", con);
                    cmd.Parameters.AddWithValue("@SName", SearchQuery.Trim());
                }

                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(table);
            }
        }





        /*LOADS THE TABLE OF OTHER STAFF ON THE BASIS OF SEARCH QUERY*/
        /*IF THE QUERY IS EMPTY THEN LOAD ALL STAFF MEMBERS OTHER WISE ONLY SPECIFIED*/
        public void LoadOtherStaff(ref DataTable table, String SearchQuery)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                SqlCommand cmd;
                if (SearchQuery == "")
                {
                    cmd = new SqlCommand("SELECT * FROM STAFF_VIEW", con);
                }
                else
                {
                    cmd = new SqlCommand("SELECT StaffID as ID , Name , Designation from OtherStaff WHERE Name like '%' + @pName + '%'", con);
                    cmd.Parameters.AddWithValue("@PName", SearchQuery.Trim());
                }

                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(table);
            }
        }





        public int GETPATIENT(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", pid, DbType.Int32);
                    parameters.Add("@name", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);
                    parameters.Add("@phone", dbType: DbType.StringFixedLength, size: 15, direction: ParameterDirection.Output);
                    parameters.Add("@birthDate", dbType: DbType.String, size: 10, direction: ParameterDirection.Output);
                    parameters.Add("@address", dbType: DbType.String, size: 40, direction: ParameterDirection.Output);
                    parameters.Add("@age", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@gender", dbType: DbType.StringFixedLength, size: 1, direction: ParameterDirection.Output);

                    con.Execute("RetrievePatientData", parameters, commandType: CommandType.StoredProcedure);

                    name = parameters.Get<string>("@name") ?? "";
                    phone = parameters.Get<string>("@phone") ?? "";
                    address = parameters.Get<string>("@address") ?? "";
                    birthDate = parameters.Get<string>("@birthDate") ?? "";
                    age = parameters.Get<int>("@age");
                    gender = parameters.Get<string>("@gender") ?? "";

                    return 0;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
        }










        public int GET_DOCTOR_PROFILE(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@dID", dID, DbType.Int32);
                    parameters.Add("@name", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);
                    parameters.Add("@phone", dbType: DbType.String, size: 15, direction: ParameterDirection.Output);
                    parameters.Add("@gender", dbType: DbType.String, size: 2, direction: ParameterDirection.Output);
                    parameters.Add("@charges", dbType: DbType.Double, direction: ParameterDirection.Output);
                    parameters.Add("@RI", dbType: DbType.Double, direction: ParameterDirection.Output);
                    parameters.Add("@PTreated", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@qualification", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
                    parameters.Add("@specialization", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@workE", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@age", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute("GET_DOCTOR_PROFILE", parameters, commandType: CommandType.StoredProcedure);

                    name = parameters.Get<string>("@name") ?? "";
                    phone = parameters.Get<string>("@phone") ?? "";
                    gender = parameters.Get<string>("@gender") ?? "";
                    charges_Per_Visit = Convert.ToSingle(parameters.Get<double>("@charges"));
                    ReputeIndex = Convert.ToSingle(parameters.Get<double>("@RI"));
                    PatientsTreated = parameters.Get<int>("@PTreated");
                    qualification = parameters.Get<string>("@qualification") ?? "";
                    specialization = parameters.Get<string>("@specialization") ?? "";
                    workE = parameters.Get<int>("@workE");
                    age = parameters.Get<int>("@age");

                    return 1;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
        }



        public int GETSATFF(int id, ref string name, ref string phone, ref string address, ref string gender, ref string desig, ref int sal)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                parameters.Add("@name", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);
                parameters.Add("@phone", dbType: DbType.String, size: 15, direction: ParameterDirection.Output);
                parameters.Add("@gender", dbType: DbType.String, size: 2, direction: ParameterDirection.Output);
                parameters.Add("@address", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
                parameters.Add("@desig", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
                parameters.Add("@sal", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute("GET_STAFF", parameters, commandType: CommandType.StoredProcedure);

                name = parameters.Get<string>("@name") ?? "";
                phone = parameters.Get<string>("@phone") ?? "";
                gender = parameters.Get<string>("@gender") ?? "";
                address = parameters.Get<string>("@address") ?? "";
                desig = parameters.Get<string>("@desig") ?? "";
                sal = parameters.Get<int>("@sal");

                return 1;
            }
        }





        //-----------------------------------------------------------------------------------//
        //                                                                                   //
        //                                       PATIENT                                     //
        //                                                                                   //
        //-----------------------------------------------------------------------------------//










        /*-------------------DISPLAYS PATIENT INFORMATION AT PATIENT HOME--------------------------------------- */

        public int patientInfoDisplayer(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", pid, DbType.Int32);
                    parameters.Add("@name", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);
                    parameters.Add("@phone", dbType: DbType.StringFixedLength, size: 15, direction: ParameterDirection.Output);
                    parameters.Add("@birthDate", dbType: DbType.String, size: 10, direction: ParameterDirection.Output);
                    parameters.Add("@address", dbType: DbType.String, size: 40, direction: ParameterDirection.Output);
                    parameters.Add("@age", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@gender", dbType: DbType.StringFixedLength, size: 1, direction: ParameterDirection.Output);

                    con.Execute("RetrievePatientData", parameters, commandType: CommandType.StoredProcedure);

                    name = parameters.Get<string>("@name") ?? "";
                    phone = parameters.Get<string>("@phone") ?? "";
                    address = parameters.Get<string>("@address") ?? "";
                    birthDate = parameters.Get<string>("@birthDate") ?? "";
                    age = parameters.Get<int>("@age");
                    gender = parameters.Get<string>("@gender") ?? "";

                    return 0;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}


		


		/*---------------------------GENERATE BILL HISTORY--------------------------------------*/

		public int getBillHistory(int id, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pId", id, DbType.Int32);
                    parameters.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("RetrieveBillHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return (int)cmd1.Parameters["@count"].Value;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------CURRENT APPOINTMENTS------------------------------------------//

		public int appointmentTodayDisplayer(int pid, ref string dName, ref string timings)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pid", pid, DbType.Int32);
                    parameters.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@timings", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
                    parameters.Add("@dName", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);

                    con.Execute("RetrieveCurrentAppointment", parameters, commandType: CommandType.StoredProcedure);

                    int status = parameters.Get<int>("@count");
                    if (status == 0)
                    {
                        return status;
                    }
                    else
                    {
                        dName = parameters.Get<string>("@dName") ?? "";
                        timings = parameters.Get<string>("@timings") ?? "";
                        return status;
                    }
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------TREATMENT HISTORY------------------------------------------//
		public int getTreatmentHistory(int id, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("RetrieveTreatmentHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return (int)cmd1.Parameters["@count"].Value;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		/*-------------------------TAKE APPOINMENT------------------------------------*/
		public int getdeptInfo(ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("select* from deptInfo", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return 1;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------VIEW DOCTORS------------------------------------------//

		public int getDeptDoctorInfo(string deptName, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("RetrieveDeptDoctorInfo", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@deptName", SqlDbType.VarChar, 30).Value = deptName;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return 1;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}







		//-------------------------------------DOCTOR PROFILE------------------------------------------//


		public int doctorInfoDisplayer(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@dID", dID, DbType.Int32);
                    parameters.Add("@name", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);
                    parameters.Add("@phone", dbType: DbType.String, size: 15, direction: ParameterDirection.Output);
                    parameters.Add("@gender", dbType: DbType.String, size: 2, direction: ParameterDirection.Output);
                    parameters.Add("@charges", dbType: DbType.Double, direction: ParameterDirection.Output);
                    parameters.Add("@RI", dbType: DbType.Double, direction: ParameterDirection.Output);
                    parameters.Add("@PTreated", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@qualification", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
                    parameters.Add("@specialization", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@workE", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@age", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute("RetrieveDoctorData", parameters, commandType: CommandType.StoredProcedure);

                    name = parameters.Get<string>("@name") ?? "";
                    phone = parameters.Get<string>("@phone") ?? "";
                    gender = parameters.Get<string>("@gender") ?? "";
                    charges_Per_Visit = Convert.ToSingle(parameters.Get<double>("@charges"));
                    ReputeIndex = Convert.ToSingle(parameters.Get<double>("@RI"));
                    PatientsTreated = parameters.Get<int>("@PTreated");
                    qualification = parameters.Get<string>("@qualification") ?? "";
                    specialization = parameters.Get<string>("@specialization") ?? "";
                    workE = parameters.Get<int>("@workE");
                    age = parameters.Get<int>("@age");

                    return 0;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}


		//-------------------------------------APPOINTMENT TAKER------------------------------------------//

		public int getFreeSlots(int dID, int pID, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("RetrieveFreeSlots", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
                    cmd1.Parameters.Add("@pID", SqlDbType.Int).Value = pID;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return (int)cmd1.Parameters["@count"].Value;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------APPOINTMENT REQUEST SENT------------------------------------------//

		public int insertAppointment(int dID, int pID, int freeSlot, ref string mes)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                string m = "";
                con.InfoMessage += delegate (object sender, SqlInfoMessageEventArgs e)
                {
                    m += "\n" + e.Message;
                };

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@dID", dID, DbType.Int32);
                    parameters.Add("@pID", pID, DbType.Int32);
                    parameters.Add("@freeSlot", freeSlot, DbType.Int32);

                    con.Execute("insertInAppointmentTable", parameters, commandType: CommandType.StoredProcedure);
                    mes = m;
                    return 0;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------PATIENT NOTIFICATIONS------------------------------------------//

		public int getNotifications(int pid, ref string dName, ref string timings)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pId", pid, DbType.Int32);
                    parameters.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@timings", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
                    parameters.Add("@dName", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);

                    con.Execute("RetrievePatientNotifications", parameters, commandType: CommandType.StoredProcedure);

                    int status = parameters.Get<int>("@count");
                    if (status == 0)
                    {
                        return status;
                    }
                    else
                    {
                        dName = parameters.Get<string>("@dName") ?? "";
                        timings = parameters.Get<string>("@timings") ?? "";
                        return status;
                    }
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------PATIENT FEEDBACK------------------------------------------//
		//-------------------------------------FUNCTION 1------------------------------------------//

		public int isFeedbackPending(int pid, ref string dName, ref string timings, ref int aID)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pId", pid, DbType.Int32);
                    parameters.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@timings", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
                    parameters.Add("@dName", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
                    parameters.Add("@aID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute("RetrievePendingFeedback", parameters, commandType: CommandType.StoredProcedure);

                    int status = parameters.Get<int>("@count");
                    if (status == 0)
                    {
                        return status;
                    }
                    else
                    {
                        dName = parameters.Get<string>("@dName") ?? "";
                        timings = parameters.Get<string>("@timings") ?? "";
                        aID = parameters.Get<int>("@aID");
                        return status;
                    }
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-------------------------------------FUNCTION 2------------------------------------------//

		public int givePendingFeedback(int aID)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@aId", aID, DbType.Int32);
                    con.Execute("storeFeedback", parameters, commandType: CommandType.StoredProcedure);
                    return 0;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		//-----------------------------------------------------------------------------------//
		//                                                                                   //
		//                                       DOCTOR                                      //
		//                                                                                   //
		//-----------------------------------------------------------------------------------//




		/*THIS FUNCITON WILL RETRIEVE THE INFORMATION OF CURRENT LOGGED IN DOCTOR*/
		public int docinfo_DAL(int doctorid, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("Doctor_Information_By_ID1", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters["@id"].Value = doctorid;
                    cmd.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(ds);

                    result = ds.Tables[0];
                    return 1;
                }
                catch (SqlException)
                {
                    return 0;
                }
            }
		}




		/*THIS FUNCTION WILL RETURN PENDING APPOINTMENT FORM THE DATABASE IN THE FORM OF DATASET*/
		public void GetAllpendingappointments_DAL(int doctorid, ref DataTable DT)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("PENDING_APPOINTMENTS2", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@DOCTOR_ID", SqlDbType.Int);
                    cmd.Parameters["@DOCTOR_ID"].Value = doctorid;
                    cmd.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                    DT = ds.Tables[0];
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                }
            }
		}




		/*THIS FUNCTION WILL BE CALLED WHEN DOCTOR APPROVE THE REQUEST OF PATIENT*/
		public int UpdateAppointment_DAL(int Appointmentid)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                int result = 0;
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@APPOINT_ID", Appointmentid, DbType.Int32);
                    result = con.Execute("APPROVE_APPOINTMENT", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                }
                return result;
            }
		}



		/*DELETES THE APPOINTMENT*/
		public int Deleteappointment_DAL(int appointmentid)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@APPOINT_ID", appointmentid, DbType.Int32);
                    con.Execute("delete_APPOINTMENT", parameters, commandType: CommandType.StoredProcedure);
                    return 1;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                    return -1;
                }
            }
		}




		/*THIS FUNTION RETURN CURRENT DAY APPONTMENT*/
		public int search_patient_DAL(int did, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("TODAYS_APPOINTMENTS", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@DOC_ID", SqlDbType.Int).Value = did;
                    cmd.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                }
                catch (SqlException)
                {
                    // handled by caller
                }
                return 1;
            }
		}




		/*UPDATE THE PRESCRIPTION WHEN APPOINTMENT IS GOING ON BY DOCTOR*/
		public int update_prescription_DAL(int did, int appointid, string disease, string progres, string prescrip)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@docId", did, DbType.Int32);
                    parameters.Add("@appointid", appointid, DbType.Int32);
                    parameters.Add("@Disease", disease, DbType.String, size: 30);
                    parameters.Add("@progress", progres, DbType.String, size: 50);
                    parameters.Add("@prescription", prescrip, DbType.String, size: 60);

                    con.Execute("UpdatePrescription", parameters, commandType: CommandType.StoredProcedure);
                    return 1;
                }
                catch (SqlException)
                {
                    return 0;
                }
            }
		}


		/*GENERATES BILL*/

		public int generate_bill_DAL(int docid, ref DataTable result)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("generate_bill", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@dId", SqlDbType.Int);
                    cmd.Parameters["@did"].Value = docid;
                    cmd.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return 1;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
		}




		public void paid_bill_DAL(int did, int appoint)
		{
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@docId", did, DbType.Int32);
                parameters.Add("@appointid", appoint, DbType.Int32);
                con.Execute("finishedPaid", parameters, commandType: CommandType.StoredProcedure);
            }
		}


        public void Unpaid_bill_DAL(int did, int appoint)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@docId", did, DbType.Int32);
                parameters.Add("@appointid", appoint, DbType.Int32);
                con.Execute("finishedUnPaid", parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public int getPHistory(int id, ref DataTable result)
        {
            // Cloud-ready: Using Dapper with RDS Proxy-compatible connection factory
            using (var con = CreateConnection())
            {
                con.Open();
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd1 = new SqlCommand("RetrievePHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@dId", SqlDbType.Int).Value = id;
                    cmd1.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }

                    result = ds.Tables[0];
                    return 1;
                }
                catch (SqlException)
                {
                    return -1;
                }
            }
        }


    }


}
