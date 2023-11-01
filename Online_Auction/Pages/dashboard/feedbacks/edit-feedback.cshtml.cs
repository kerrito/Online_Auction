using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRC.Pages.Auth_user;
using System.Data.SqlClient;

namespace SRC.Pages.dashboard.feedbacks
{
    public class edit_feedbackModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public feedback_list feedback=new feedback_list();
        public void OnGet()
        {
            bool sessionExists = HttpContext.Session.TryGetValue("login", out byte[] value);
            if (!sessionExists || HttpContext.Session.GetString("login") == null || HttpContext.Session.GetString("login") != "true")
            {
                Response.Redirect("/Auth-user/login");
            }
            if (HttpContext.Session.GetInt32("role") != null)
            {
                if (HttpContext.Session.GetInt32("role") == 1)
                {

                }
                else
                {
                    Response.Redirect("/user-dashboard/products/product");
                }
            }
            string id = Request.Query["id"];
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM feedback WHERE id=@id";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {


                                feedback.id = reader.GetInt32(0);
                                feedback.fname = reader.GetString(1);
                                feedback.lname = reader.GetString(2);
                                feedback.email = reader.GetString(3);
                                feedback.Phone = reader.GetString(4);
                                feedback.address = reader.GetString(5);
                                feedback.Message = reader.GetString(6);
                                feedback.status = reader.GetInt32(7);
                            }
                            else
                            {
                                errorMessage = "Account Doesn't Exists";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }
        public void OnPost()
        {
            int id = int.Parse( Request.Form["id"]);
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string query = "UPDATE feedback SET status=2 WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Response.Redirect("/dashboard/feedbacks/feedback");
                        }
                        else
                        {
                            errorMessage = "Failed to Update status";
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
