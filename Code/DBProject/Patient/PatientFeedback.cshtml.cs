using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class PatientFeedbackModel : PageModel
    {
        public string Feedback { get; set; }

        // FDoctor / FTimings were Labels kept by the view state across the postback;
        // they are posted back in hidden fields so the page after "Give Feedback" shows the same.
        [BindProperty]
        public string FDoctor { get; set; }

        [BindProperty]
        public string FTimings { get; set; }

        // Visible state of Message, List and button1 (true when a feedback is pending)
        public bool ShowFeedback { get; set; }

        // Selected value of the DropDownList "List"
        [BindProperty]
        public string List { get; set; }

        public string F { get; set; }

        public void OnGet()
        {
            HttpContext.Session.SetString("aID", "");
            pendingFeedback();
        }

        //-----------------------Function1--------------------------//

        protected void pendingFeedback()
        {
            myDAL objmyDAl = new myDAL();

            int pid = HttpContext.Session.GetRequiredInt32("idoriginal");

            string dName = "";
            string timings = "";

            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                Feedback = "There was some error in retrieving the Pending Feedbacks.";
            }

            else if (status == 0)
            {
                Feedback = "There are no pending feedbacks :)";
            }

            else
            {
                HttpContext.Session.SetInt32("aID", aID);

                FDoctor = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                FTimings = "The Appointment Timings were : " + timings;

                //Make the things visible -- Magic O.O --
                ShowFeedback = true;

                return;
            }
        }

        //-----------------------Function2--------------------------//

        public IActionResult OnPostGiveFeedback()
        {
            // the button can only be clicked when the feedback controls were visible
            ShowFeedback = true;

            myDAL objmyDAl = new myDAL();

            int aID = HttpContext.Session.GetRequiredInt32("aID");


            int rating = Convert.ToInt32(List);


            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                F = "There was some error.";
            }

            else if (status == 0)
            {
                F = "Thank you for your feedback :)";
            }

            return Page();
        }
    }
}
