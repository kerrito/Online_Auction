using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Online_Auction.Pages.dashboard.category
{
    public class categorysModel : PageModel
    {
        public List<category> category_list = new List<category>();
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
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM category ORDER BY id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                category category = new category();
                                category.id = reader.GetInt32(0);
                                category.name = reader.GetString(1);
                                category.status = reader.GetInt32(2);
                                category_list.Add(category);
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
            category ncat=new category();
            ncat.name = Request.Form["name"];
            ncat.status = 1;
            successMessage = ncat.name;
            try {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "INSERT INTO category (name,status) VALUES (@name,@status)";
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.Parameters.AddWithValue("@name", ncat.name);
                        command.Parameters.AddWithValue("@status", ncat.status);


                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Category Inserted Successfully";

                            string query = "SELECT * FROM category ORDER BY id DESC";
                            using (SqlCommand comm = new SqlCommand(query, con))
                            {
                                using (SqlDataReader reader = comm.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        category category = new category();
                                        category.id = reader.GetInt32(0);
                                        category.name = reader.GetString(1);
                                        category.status = reader.GetInt32(2);
                                        category_list.Add(category);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message; 
                return;
            }
            
        }
    }

    public class category
    {
        public int id { get; set; }

        public string name { get; set; }

        public int status { get; set; }
    }
}
