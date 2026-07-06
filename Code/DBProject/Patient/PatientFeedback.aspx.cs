using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;




namespace DBProject
{
    public partial class PatientFeedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
                // Cloud-ready: Session set via CloudSessionHelper for Amazon ElastiCache for Redis
                // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
                CloudSessionHelper.Set(Session, "aID", "");
				pendingFeedback(sender, e);
			}
        }



        
        //-----------------------Function1--------------------------//

        protected void pendingFeedback(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            // Cloud-ready: Session accessed via CloudSessionHelper for distributed session state
            int pid = CloudSessionHelper.GetInt(Session, "idoriginal");

            string dName = "";
            string timings = "";

            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                Feedback.Text = "There was some error in retrieving the Pending Feedbacks.";
            }

            else if (status == 0)
            {
                Feedback.Text = "There are no pending feedbacks :)";
            }

            else
            {
                // Cloud-ready: Session set via CloudSessionHelper for distributed session state
                CloudSessionHelper.Set(Session, "aID", aID);

                FDoctor.Text = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                FTimings.Text = "The Appointment Timings were : " + timings;

                //Make the things visible -- Magic O.O --
                Message.Visible = true;
                List.Visible = true;
                button1.Visible = true;

                return;
            }
        }




        //-----------------------Function2--------------------------//

        protected void giveFeedback(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            // Cloud-ready: Session accessed via CloudSessionHelper for distributed session state
            int aID = CloudSessionHelper.GetInt(Session, "aID");

            int rating = Convert.ToInt32(List.SelectedItem.Value);

            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                F.Text = "There was some error.";
            }

            else if (status == 0)
            {
                F.Text = "Thank you for your feedback :)";
            }

            return;
        }



        //-----------------------Add a new function here------------------//



    }
}
