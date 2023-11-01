using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SRC.Pages.dashboard.user
{
    public class edit_userModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public userlist user = new userlist();
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

                    string sql = "SELECT * FROM users WHERE id=@id";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                
                                user.Id = reader.GetInt32(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                user.Email = reader.GetString(3);
                                user.Phone = reader.GetString(4);
                                user.address = reader.GetString(5);
                                user.role = reader.GetInt32(8);
                                user.status = reader.GetInt32(9);
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
            userlist Nuser = new userlist();
            Nuser.Email = Request.Form["email"];
            Nuser.Phone = Request.Form["phone"];
            Nuser.address = Request.Form["address"];
            Nuser.FirstName = Request.Form["fname"];
            Nuser.LastName = Request.Form["lname"];
            Nuser.Id = Convert.ToInt32(Request.Form["id"]);
            Nuser.role = Convert.ToInt32(Request.Form["role"]);
            Nuser.status = Convert.ToInt32(Request.Form["status"]);

            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string query = "UPDATE users SET fname=@fname,lname=@lname,email=@email,phone=@phone" +
                        ",address=@address,role=@role,status=@status WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@id", Nuser.Id);
                        command.Parameters.AddWithValue("@fname", Nuser.FirstName);
                        command.Parameters.AddWithValue("@lname", Nuser.LastName);
                        command.Parameters.AddWithValue("@email", Nuser.Email);
                        command.Parameters.AddWithValue("@phone", Nuser.Phone);
                        command.Parameters.AddWithValue("@address", Nuser.address);
                        command.Parameters.AddWithValue("@role", Nuser.role);
                        command.Parameters.AddWithValue("@status", Nuser.status);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Account Edit Successfully";
                            return;
                        }
                        else
                        {
                            errorMessage = "Failed to Edit account";
                            return;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    // Handle duplicate email error
                    errorMessage = "Email address already exists.";
                }
                else
                {

                    errorMessage = ex.Message;
                    return;
                }
            }
        }
    }
}
