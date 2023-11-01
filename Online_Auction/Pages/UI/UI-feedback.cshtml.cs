using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRC.Pages.dashboard.feedbacks;
using System.Data.SqlClient;

namespace SRC.Pages.UI
{
    public class UI_feedbackModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {
            feedback_list feedback = new feedback_list();
            feedback.email = Request.Form["email"];
            feedback.Message = Request.Form["msg"];
            feedback.Phone = Request.Form["phone"];
            feedback.address = Request.Form["address"];
            feedback.fname = Request.Form["fname"];
            feedback.lname = Request.Form["lname"];
            feedback.status = 1;

            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string query = "INSERT INTO feedback (fname,lname,email,phone,address,msg,status)" +
                        "Values (@fname,@lname,@email,@phone,@address,@msg,@status)";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@fname", feedback.fname);
                        command.Parameters.AddWithValue("@lname", feedback.lname);
                        command.Parameters.AddWithValue("@email", feedback.email);
                        command.Parameters.AddWithValue("@phone", feedback.Phone);
                        command.Parameters.AddWithValue("@address", feedback.address);
                        command.Parameters.AddWithValue("@msg", feedback.Message);
                        command.Parameters.AddWithValue("@status", feedback.status);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Feeedback Submitted Successfully";
                            return;
                        }
                        else
                        {
                            errorMessage = "Failed to Submit Feedback";
                            return;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {

                    errorMessage = ex.Message;
                    return;
                
            }
        }
    }
}
