using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRC.Pages.dashboard.Buses;
using System.Data.SqlClient;

namespace Online_Auction.Pages.UI
{
    public class bidModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public product product = new product();
        public List<product> buseslist = new List<product>();
        public List<biding> bid_log = new List<biding>();
        public biding last_bid = new biding();
        public void OnGet()
        {
            bool sessionExists = HttpContext.Session.TryGetValue("login", out byte[] value);
            if (!sessionExists || HttpContext.Session.GetString("login") == null || HttpContext.Session.GetString("login") != "true")
            {
                Response.Redirect("/Auth-user/login");
            }
            string id = Request.Query["id"];
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM product " +
                        "INNER JOIN users ON users.id=product.saller_id " +
                        "INNER JOIN category ON category.id=product.category WHERE product.id=@id";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {


                                product.id = reader.GetInt32(0);
                                product.name = reader.GetString(1);
                                product.description = reader.GetString(2);
                                product.img = reader.GetString(3);
                                product.bid_status = reader.GetInt32(4);
                                product.start_date = reader.GetString(5);
                                product.end_date = reader.GetString(6);
                                product.start_price = reader.GetString(7);
                                product.inc_price = reader.GetString(8);
                                product.category = reader.GetInt32(9);
                                product.sellar_id = reader.GetInt32(10);
                                product.status = reader.GetInt32(11);
                                product.sell = reader.GetString(13);
                                product.cate = reader.GetString(23);
                            }
                            else
                            {
                                errorMessage = "Account Doesn't Exists";
                                return;
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                errorMessage = ex.Message;

            }
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM product INNER JOIN users ON users.id=product.saller_id " +
                        "INNER JOIN category ON category.id=product.category WHERE product.end_time > CONVERT(DATE, GETDATE()) " +
                        "AND product.status=1 AND product.bid_status=1 AND product.id!=@id ORDER BY product.id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id",id);
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
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT * FROM biding WHERE item_id=@id ORDER BY id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id",id);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                biding bid = new biding();
                                bid.id = reader.GetInt32(0);
                                bid.price = reader.GetString(1);
                                bid.buyer_id=reader.GetInt32(2);
                                bid.bought_at=reader.GetString(3);
                                bid.status = reader.GetInt32(4);
                                bid.item_id=reader.GetInt32(5);
                                bid_log.Add(bid);
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
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "SELECT TOP 1 * FROM biding WHERE item_id=@id ORDER BY id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                last_bid.id = reader.GetInt32(0);
                                last_bid.price = reader.GetString(1);
                                last_bid.buyer_id = reader.GetInt32(2);
                                last_bid.bought_at = reader.GetString(3);
                                last_bid.status = reader.GetInt32(4);
                                last_bid.item_id = reader.GetInt32(5);
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
            biding nbid=new biding();
            nbid.price = Request.Form["price"];
            nbid.item_id = int.Parse(Request.Form["id"]);
            nbid.buyer_id = int.Parse(HttpContext.Session.GetString("id"));
            nbid.status = 1;
            try
            {
                string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string sql = "INSERT INTO biding (price,buyer_id,bought_at,status,item_id) VALUES (@price,@buyer,CONVERT(Date,GETDATE()),@status," +
                        "@item_id)";
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.Parameters.AddWithValue("@price", nbid.price);
                        command.Parameters.AddWithValue("@buyer", nbid.buyer_id);
                        command.Parameters.AddWithValue("@status", nbid.status);
                        command.Parameters.AddWithValue("@item_id", nbid.item_id);


                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Response.Redirect("/UI/bid?id="+nbid.item_id);

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
    public class biding
    {
        public int id { get; set; }
        public string price { get; set; }

        public int buyer_id { get; set; }

        public string bought_at { get; set; }

        public int status { get; set; }

        public int item_id { get; set;}

        public string buyer_name { get; set; }

        public string item_name { get; set; }
    }
}
