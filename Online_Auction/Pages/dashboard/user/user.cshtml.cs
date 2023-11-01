using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace SRC.Pages.dashboard.user
{
    public class userModel : PageModel
    {
        public List<userlist> userlist = new List<userlist>();
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

                    string sql = "SELECT * FROM users WHERE role!=1";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userlist user = new userlist();
                                user.Id = reader.GetInt32(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                user.Email = reader.GetString(3);
                                user.Phone = reader.GetString(4);
                                user.address = reader.GetString(5);
                                user.role = reader.GetInt32(8);
                                user.status = reader.GetInt32(9);
                                userlist.Add(user);
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
}
