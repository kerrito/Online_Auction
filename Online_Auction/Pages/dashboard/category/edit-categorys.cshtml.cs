using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Online_Auction.Pages.dashboard.category
{
    public class edit_categorysModel : PageModel
    {
        public category category = new category();
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
            try
            {
                int id =int.Parse( Request.Query["id"]);

                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM category WHERE id=@id";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id",id);

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                category.id = reader.GetInt32(0);
                                category.name = reader.GetString(1);
                                category.status = reader.GetInt32(2);
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
            category ncat = new category();
            ncat.id = int.Parse(Request.Form["id"]);
            ncat.name = Request.Form["name"];
            ncat.status = int.Parse(Request.Form["status"]);
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "UPDATE category SET name=@name,status=@status WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.Parameters.AddWithValue("@id",ncat.id);
                        command.Parameters.AddWithValue("@name", ncat.name);
                        command.Parameters.AddWithValue("@status", ncat.status);


                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Response.Redirect("/dashboard/category/categorys");
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
