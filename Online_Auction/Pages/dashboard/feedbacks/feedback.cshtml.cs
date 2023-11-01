using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SRC.Pages.dashboard.feedbacks
{
    public class feedbackModel : PageModel
    { 
        public List<feedback_list> feedbacks = new List<feedback_list>();
        public string errorMessage = "";
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
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM feedback ORDER BY id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedback_list feedback_List = new feedback_list();
                                feedback_List.id = reader.GetInt32(0);
                                feedback_List.fname = reader.GetString(1);
                                feedback_List.lname = reader.GetString(2);
                                feedback_List.email = reader.GetString(3);
                                feedback_List.Phone = reader.GetString(4);
                                feedback_List.address = reader.GetString(5);
                                feedback_List.Message = reader.GetString(6);
                                feedback_List.status = reader.GetInt32(7);
                                feedbacks.Add(feedback_List);
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
    }
    public class feedback_list
    {
        public int id { get; set; }
        public string fname { get; set; }

        public string lname { get; set; }

        public string email { get; set; }
        public string Phone { get; set; }

        public string address { get; set; }

        public string Message { get; set; }
        public int status { get; set; }

    }
}
