using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SRC.Pages.Auth_user
{
    public class forget_passwordModel : PageModel
    {
        public string errorMessage = "";
        public void OnGet()
        {
            bool sessionExists = HttpContext.Session.TryGetValue("login", out byte[] value);
            if (!sessionExists || HttpContext.Session.GetString("login") == null || HttpContext.Session.GetString("login") == "true")
            {
                if (HttpContext.Session.GetInt32("role") != null)
                {
                    if (HttpContext.Session.GetInt32("role") == 1)
                    {

                        Response.Redirect("/dashboard/user/user");
                    }
                    else
                    {
                        Response.Redirect("/user-dashboard/products/product");
                    }
                }
            }
        }
        public void OnPost()
        {
            user user = new user();
            user.Email = Request.Form["email"];
            user.password = Request.Form["pass"];

            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM users WHERE email=@email";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@email", user.Email);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                createpasswordhash(user.password, out byte[] passwordhash, out byte[] passwordsalt);
                                user.Passwordhash= passwordhash;
                                user.Passwordsalt= passwordsalt;
                                user.Id = reader.GetInt32(0);
                                user.role=reader.GetInt32(8);

                                reader.Close();

                                string query= "UPDATE users SET passwordhash=@passwrdhash,passwordsalt=@passwordsalt WHERE id=@id";
                                using(SqlCommand command=new SqlCommand(query, con))
                                {

                                    
                                    command.Parameters.AddWithValue("@passwrdhash", user.Passwordhash);
                                    command.Parameters.AddWithValue("@passwordsalt", user.Passwordsalt);
                                    command.Parameters.AddWithValue("@id", user.Id);
                                    
                                    int rowsAffected = command.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        HttpContext.Session.SetString("login", "true");
                                        HttpContext.Session.SetString("email", "" + user.Email);
                                        HttpContext.Session.SetString("id", reader.GetInt32(0).ToString());
                                        HttpContext.Session.SetString("Name", reader.GetString(1));
                                        HttpContext.Session.SetInt32("role", reader.GetInt32(8));
                                        if (HttpContext.Session.GetInt32("role") != null)
                                        {
                                            if (HttpContext.Session.GetInt32("role") == 1)
                                            {

                                                Response.Redirect("/dashboard/user/user");
                                            }
                                            else
                                            {
                                                Response.Redirect("/user-dashboard/products/product");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errorMessage = "Failed To Update password";
                                    }
                                    
                                }
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
        private void createpasswordhash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
