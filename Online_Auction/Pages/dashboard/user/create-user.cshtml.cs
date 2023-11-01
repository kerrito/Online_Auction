using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SRC.Pages.dashboard.user
{
    public class create_userModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
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
        }
        public void OnPost()
        {
            userlist user = new userlist();
            user.Email = Request.Form["email"];
            user.password = Request.Form["pass"];
            user.Phone = Request.Form["phone"];
            user.address = Request.Form["address"];
            user.FirstName = Request.Form["fname"];
            user.LastName = Request.Form["lname"];
            user.role = Convert.ToInt32(Request.Form["role"]);
            user.status = 1;

            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();


                    createpasswordhash(user.password, out byte[] passwordhash, out byte[] passwordsalt);
                    user.Passwordhash = passwordhash;
                    user.Passwordsalt = passwordsalt;
                    string query = "INSERT INTO users (fname,lname,email,phone,address,passwordhash,passwordsalt,role,status)" +
                        "Values (@fname,@lname,@email,@phone,@address,@passwrdhash,@passwordsalt,@role,@status)";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@fname", user.FirstName);
                        command.Parameters.AddWithValue("@lname", user.LastName);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@phone", user.Phone);
                        command.Parameters.AddWithValue("@address", user.address);
                        command.Parameters.AddWithValue("@role", user.role);
                        command.Parameters.AddWithValue("@status", user.status);

                        command.Parameters.AddWithValue("@passwrdhash", user.Passwordhash);
                        command.Parameters.AddWithValue("@passwordsalt", user.Passwordsalt);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Account Created Successfully";
                            return;
                        }
                        else
                        {
                            errorMessage = "Failed to create account";
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
    public class userlist
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

