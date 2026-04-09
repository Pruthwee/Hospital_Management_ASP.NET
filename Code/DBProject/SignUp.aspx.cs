using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject.Helpers;
using System.Data;


namespace DBProject
{
    /// <summary>
    /// SignUp and Login page
    /// CLOUD READINESS IMPROVEMENTS:
    /// - Uses SessionHelper for abstraction layer
    /// - Ready for migration to JWT-based authentication
    /// - Compatible with stateless cloud deployment patterns
    /// 
    /// MIGRATION NOTES FOR GKE:
    /// - Current: Uses server-side session state (requires sticky sessions or distributed cache)
    /// - Recommended: Migrate to JWT tokens stored in HTTP-only cookies
    /// - Alternative: Configure Memorystore Redis for distributed session state
    /// </summary>
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // CLOUD READY: Initialize session using helper
            // This can be migrated to JWT token validation in future
            if (!IsPostBack)
            {
                SessionHelper.SetUserId(0); // Clear any existing session
            }
        }

        //-----------------------Function1: Login Validation--------------------------//
        /// <summary>
        /// Validates user login credentials
        /// CLOUD READY: Uses DAL with connection pooling
        /// TODO: Add rate limiting for cloud security
        /// TODO: Implement JWT token generation instead of session
        /// </summary>
        protected void loginV(object sender, EventArgs e)
        {
            string email = loginEmail.Text;
            string password = loginPassword.Text;

            myDAL objmyDAl = new myDAL();

            int status = 0;
            int type = 0;
            int id = 0;

            // CLOUD READY: DAL uses connection pooling and proper resource disposal
            status = objmyDAl.validateLogin(email, password, ref type, ref id);

            if (status == 0)
            {
                // CLOUD READY: Using SessionHelper for abstraction
                // Future: Replace with JWT token generation
                SessionHelper.SetUserId(id);
                SessionHelper.SetUserType(type);

                // Route based on user type
                if (type == 1) // Patient
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/Patient/PatientHome.aspx");
                    return;
                }
                else if (type == 2) // Doctor
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/Doctor/DoctorHome.aspx");
                    return;
                }
                else if (type == 3) // Admin
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/Admin/AdminHome.aspx");
                    return;
                }
            }
            else if (status == 1)
            {
                // TODO: Log failed login attempts for cloud monitoring
                Response.Write("<script>alert('Email not found. Try Again !');</script>");
            }
            else if (status == 2)
            {
                // TODO: Log failed login attempts for cloud monitoring
                Response.Write("<script>alert('Incorrect Password. Try Again !');</script>");
            }
            else if (status == -1)
            {
                // TODO: Log errors to cloud logging service (Stackdriver/Cloud Logging)
                Response.Write("<script>alert('There was some error. Try Again !');</script>");
            }
        }




        //-----------------------Function2: Signup Validation--------------------------//
        /// <summary>
        /// Validates and creates new patient user
        /// CLOUD READY: Uses DAL with connection pooling
        /// TODO: Add input validation and sanitization
        /// TODO: Implement JWT token generation instead of session
        /// </summary>
        protected void signupV(object sender, EventArgs e)
        {
            string Name = sName.Text;
            string BirthDate = sBirthDate.Text;
            string Email = sEmail.Text;
            string Password = sPassword.Text;
            string PhoneNo = Phone.Text;
            string Addr = Address.Text;

            string gender = Request.Form["Gender"].ToString();
           


            myDAL objmyDAl = new myDAL();

            int id = 0;

            // CLOUD READY: DAL uses connection pooling and proper resource disposal
            int status = objmyDAl.validateUser(Name, BirthDate, Email, Password, PhoneNo, gender, Addr, ref id);


            //status == 0 failure
            if (status == 0)
            {
                Response.Write("<script>alert('Email already exists. Please choose a different one.');</script>");
            }
            else if (status == 1)
            {
                // CLOUD READY: Using SessionHelper for abstraction
                // Future: Replace with JWT token generation
                SessionHelper.SetUserId(id);
                SessionHelper.SetUserType(1); // Patient type

                // TODO: Log successful registration to cloud monitoring
                //Response.Write("<script>alert('Registration Successful !');</script>");

                Response.BufferOutput = true;
                Response.Redirect("~/Patient/PatientHome.aspx");
            }
            else if (status == -1)
            {
                // TODO: Log errors to cloud logging service (Stackdriver/Cloud Logging)
                Response.Write("<script>alert('There was some error. Try again !');</script>");
            }
           
        }

            //Enter new function here//
    }

}
