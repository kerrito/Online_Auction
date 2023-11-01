using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace SRC.Pages.Auth_user
{
    public class loginModel : PageModel
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

                    string sql = "SELECT * FROM users WHERE email=@email AND status=1";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@email", user.Email);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (crackpasswordhash(user.password, (byte[])reader["passwordhash"], (byte[])reader["passwordsalt"]))
                                {
                                    HttpContext.Session.SetString("login", "true");
                                    HttpContext.Session.SetString("email", "" + user.Email);
                                    HttpContext.Session.SetString("id", reader.GetInt32(0).ToString());
                                    HttpContext.Session.SetString("Name", reader.GetString(1));
                                    HttpContext.Session.SetInt32("role", reader.GetInt32(8));
                                    if (HttpContext.Session.GetInt32("role") == 1)
                                    {

                                        Response.Redirect("/dashboard/user/user");
                                    }
                                    else
                                    {
                                        Response.Redirect("/user-dashboard/products/product");
                                    }
                                }
                                else
                                {
                                    errorMessage = "Invalid Email/Password";
                                    return;
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
        
        private bool crackpasswordhash(string password, byte[] passwordhash, byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512(passwordsalt))
            {
                var computehash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computehash.SequenceEqual(passwordhash);
            }
        }
    }
    public class user
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string password { get; set; }
        public byte[] Passwordhash { get; set; }
        public byte[] Passwordsalt { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string address { get; set; }

        public int role { get; set; }

        public int status { get; set; }
    }
}
