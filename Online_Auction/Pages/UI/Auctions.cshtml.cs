using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRC.Pages.dashboard.Buses;
using System.Data.SqlClient;

namespace Online_Auction.Pages.UI
{
    public class bidingModel : PageModel
    {
        public List<product> buseslist = new List<product>();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            bool sessionExists = HttpContext.Session.TryGetValue("login", out byte[] value);
            if (!sessionExists || HttpContext.Session.GetString("login") == null || HttpContext.Session.GetString("login") != "true")
            {
                Response.Redirect("/Auth-user/login");
            }
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM product INNER JOIN users ON users.id=product.saller_id " +
                        "INNER JOIN category ON category.id=product.category WHERE product.end_time > CONVERT(DATE, GETDATE()) " +
                        "AND product.status=1 AND product.bid_status=1 ORDER BY product.id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                product buses = new product();
                                buses.id = reader.GetInt32(0);
                                buses.name = reader.GetString(1);
                                buses.description = reader.GetString(2);
                                buses.img = reader.GetString(3);
                                buses.bid_status = reader.GetInt32(4);
                                buses.start_date = reader.GetString(5);
                                buses.end_date = reader.GetString(6);
                                buses.start_price = reader.GetString(7);
                                buses.inc_price = reader.GetString(8);
                                buses.category = reader.GetInt32(9);
                                buses.sellar_id = reader.GetInt32(10);
                                buses.status = reader.GetInt32(11);
                                buses.sell = reader.GetString(13);
                                buses.cate = reader.GetString(23);
                                buseslist.Add(buses);
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
