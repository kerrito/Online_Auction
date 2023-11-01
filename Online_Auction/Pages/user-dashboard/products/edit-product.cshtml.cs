using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Online_Auction.Pages.dashboard.category;
using SRC.Pages.dashboard.Buses;
using System.Data.SqlClient;

namespace Online_Auction.Pages.user_dashboard.products
{
    public class edit_productModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        private static readonly string[] AllowedExtensions = { ".png", ".jpg" };
        public product product = new product();
        public List<category> category_list = new List<category>();
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

                    string sql = "SELECT * FROM category WHERE status=1 AND id!=@id ORDER BY id DESC";
                    using (SqlCommand comm = new SqlCommand(sql, con))
                    {
                        comm.Parameters.AddWithValue("@id", product.category);
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

        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            product nproduct = new product();
            nproduct.id = int.Parse(Request.Form["id"]);
            nproduct.name = Request.Form["name"];
            nproduct.description = Request.Form["desc"];
            nproduct.end_date = Request.Form["end_date"];
            nproduct.start_price = Request.Form["start_amount"];
            nproduct.inc_price = Request.Form["inc_amount"];
            nproduct.sellar_id = int.Parse(HttpContext.Session.GetString("id"));
            nproduct.category = int.Parse(Request.Form["category"]);
            nproduct.bid_status = 1;
            nproduct.status = int.Parse(Request.Form["status"]);
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileExtension = Path.GetExtension(imageFile.FileName);

                if (AllowedExtensions.Contains(fileExtension.ToLower()))
                {
                    var uniqueFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
                    var uploadPath = Path.Combine("wwwroot", "uploads", "img", uniqueFileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                        nproduct.img = uniqueFileName;

                        try
                        {
                            string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                            using (SqlConnection con = new SqlConnection(constr))
                            {
                                con.Open();

                                string sql = "UPDATE product SET name=@name,[desc]=@desc,img=@img,bid_status=@bid_status," +
                                    "end_time=@end_date,start_price=@start_price,increment=@inc,category=@category," +
                                    "saller_id=@sellar_id,status=@status WHERE id=@id";
                                using (SqlCommand comm = new SqlCommand(sql, con))
                                {
                                    comm.Parameters.AddWithValue("@id", nproduct.id);
                                    comm.Parameters.AddWithValue("@name", nproduct.name);
                                    comm.Parameters.AddWithValue("@desc", nproduct.description);
                                    comm.Parameters.AddWithValue("@img", nproduct.img);
                                    comm.Parameters.AddWithValue("@bid_status", nproduct.bid_status);
                                    comm.Parameters.AddWithValue("@end_date", nproduct.end_date);
                                    comm.Parameters.AddWithValue("@start_price", nproduct.start_price);
                                    comm.Parameters.AddWithValue("@inc", nproduct.inc_price);
                                    comm.Parameters.AddWithValue("@category", nproduct.category);
                                    comm.Parameters.AddWithValue("@sellar_id", nproduct.sellar_id);
                                    comm.Parameters.AddWithValue("@status", nproduct.status);
                                    int rowsAffected = comm.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        Response.Redirect("/dashboard/products/product");

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                            return Page();
                        }
                    }


                }
                else
                {
                    errorMessage = "Invalid file format. Allowed formats: .png, .jpg";
                }
            }
            else
            {
                try
                {
                    string constr = "Data Source=DESKTOP-BJP90HP\\SA;Initial Catalog=online_auction;User ID=sa;Password=123";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();

                        string sql = "UPDATE product SET name=@name,[desc]=@desc,bid_status=@bid_status," +
                            "end_time=@end_date,start_price=@start_price,increment=@inc,category=@category," +
                            "saller_id=@sellar_id,status=@status WHERE id=@id";
                        using (SqlCommand comm = new SqlCommand(sql, con))
                        {
                            comm.Parameters.AddWithValue("@id", nproduct.id);
                            comm.Parameters.AddWithValue("@name", nproduct.name);
                            comm.Parameters.AddWithValue("@desc", nproduct.description);
                            comm.Parameters.AddWithValue("@bid_status", nproduct.bid_status);
                            comm.Parameters.AddWithValue("@end_date", nproduct.end_date);
                            comm.Parameters.AddWithValue("@start_price", nproduct.start_price);
                            comm.Parameters.AddWithValue("@inc", nproduct.inc_price);
                            comm.Parameters.AddWithValue("@category", nproduct.category);
                            comm.Parameters.AddWithValue("@sellar_id", nproduct.sellar_id);
                            comm.Parameters.AddWithValue("@status", nproduct.status);
                            int rowsAffected = comm.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Response.Redirect("/dashboard/products/product");

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return Page();
                }
            }

            return Page();
        }
    }
}
