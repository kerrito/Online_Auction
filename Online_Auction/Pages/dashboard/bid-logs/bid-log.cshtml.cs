using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Online_Auction.Pages.UI;
using System.Data.SqlClient;

namespace Online_Auction.Pages.dashboard.bid_logs
{
    public class bid_logModel : PageModel
    {

        public List<biding> bid_log = new List<biding>();
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

                    string sql = "SELECT * FROM biding INNER JOIN users ON users.id=biding.buyer_id " +
                        "INNER JOIN product ON product.id=biding.item_id ORDER BY biding.id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                biding bid = new biding();
                                bid.id = reader.GetInt32(0);
                                bid.price = reader.GetString(1);
                                bid.buyer_id = reader.GetInt32(2);
                                bid.bought_at = reader.GetString(3);
                                bid.status = reader.GetInt32(4);
                                bid.item_id = reader.GetInt32(5);
                                bid.buyer_name= reader.GetString(7);
                                bid.item_name = reader.GetString(17);
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
        }
    }
}
