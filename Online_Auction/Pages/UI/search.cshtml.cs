using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Online_Auction.Pages.dashboard.category;
using SRC.Pages.dashboard.Buses;
using System.Data.SqlClient;

namespace Online_Auction.Pages.UI
{
    public class searchModel : PageModel
    {
        public List<product> buseslist = new List<product>();
        public List<category> category_list = new List<category>();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
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
            try
            {
                String keyval = Request.Query["id"];
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM product INNER JOIN users ON users.id=product.saller_id " +
                        "INNER JOIN category ON category.id=product.category WHERE product.end_time >= CONVERT(DATE, GETDATE()) " +
                        "AND product.status=1 AND product.bid_status=1 AND ( product.name LIKE @id OR product.category LIKE @id " +
                        "OR product.[desc] LIKE @id ) ORDER BY product.id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", keyval);
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
        public void OnPost()
        {
            string val = Request.Form["id"];
            Response.Redirect("/UI/search?id="+val);
        }
    }
}
