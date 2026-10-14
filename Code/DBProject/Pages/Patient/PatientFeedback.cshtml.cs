using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class PatientFeedbackModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientFeedbackModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string FeedbackText { get; set; } = "";
        public string FDoctorText { get; set; } = "";
        public string FTimingsText { get; set; } = "";
        public string MessageText { get; set; } = "";
        public bool MessageVisible { get; set; } = false;
        public bool ListVisible { get; set; } = false;
        public bool ButtonVisible { get; set; } = false;
        public string FText { get; set; } = "";

        public void OnGet()
        {
            _httpContextAccessor.HttpContext.Session.SetString("aID", "");
            pendingFeedback();
        }

        protected void pendingFeedback()
        {
            myDAL objmyDAl = new myDAL();

            int pid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            string dName = "";
            string timings = "";
            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                FeedbackText = "There was some error in retrieving the Pending Feedbacks.";
            }
            else if (status == 0)
            {
                FeedbackText = "There are no pending feedbacks :)";
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("aID", aID);

                FDoctorText = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                FTimingsText = "The Appointment Timings were : " + timings;

                MessageVisible = true;
                ListVisible = true;
                ButtonVisible = true;
            }
        }

        public IActionResult OnPostGiveFeedback(int rating)
        {
            myDAL objmyDAl = new myDAL();

            int aID = (int)_httpContextAccessor.HttpContext.Session.GetInt32("aID");

            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                FText = "There was some error.";
            }
            else if (status == 0)
            {
                FText = "Thank you for your feedback :)";
            }

            return Page();
        }
    }
}
