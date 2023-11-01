using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Online_Auction.Pages.dashboard.category;
using SRC.Pages.dashboard.Buses;
using System.Data.SqlClient;

namespace Online_Auction.Pages.user_dashboard.products
{
    public class cretae_productModel : PageModel
    {
        public List<userlist> userlist = new List<userlist>();
        public List<category> category_list = new List<category>();
        public string errorMessage = "";
        public string successMessage = "";
        private static readonly string[] AllowedExtensions = { ".png", ".jpg" };
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


        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            product nproduct = new product();
            nproduct.name = Request.Form["name"];
            nproduct.description = Request.Form["desc"];
            nproduct.end_date = Request.Form["end_date"];
            nproduct.start_price = Request.Form["start_amount"];
            nproduct.inc_price = Request.Form["inc_amount"];
            nproduct.sellar_id = int.Parse(HttpContext.Session.GetString("id"));
            nproduct.category = int.Parse(Request.Form["category"]);
            nproduct.bid_status = 1;
            nproduct.status = 1;

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

                                string sql = "INSERT INTO product (name,[desc],img,bid_status," +
                                    "start_date,end_time,start_price,increment,category,saller_id,status) VALUES " +
                                    "(@name,@desc,@img,@bid_status,CONVERT(DATE, GETDATE()),@end_date,@start_price,@inc,@category," +
                                    "@sellar_id,@status)";
                                using (SqlCommand comm = new SqlCommand(sql, con))
                                {
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
                                        successMessage = "Product has been inserted Successfully";
                                        return Page();

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
                errorMessage = "Please select a image file.";
            }

            return Page();
        }
    }
}
