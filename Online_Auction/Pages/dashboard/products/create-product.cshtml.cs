using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Online_Auction.Pages.dashboard.category;

namespace SRC.Pages.dashboard.Buses
{
    public class create_busModel : PageModel
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


        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            product nproduct = new product();
            nproduct.name = Request.Form["name"];
            nproduct.description = Request.Form["desc"];
            nproduct.end_date = Request.Form["end_date"];
            nproduct.start_price= Request.Form["start_amount"];
            nproduct.inc_price = Request.Form["inc_amount"];
            nproduct.sellar_id = int.Parse(Request.Form["sellar_id"]);
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
                                    comm.Parameters.AddWithValue("@desc",nproduct.description);
                                    comm.Parameters.AddWithValue("@img",nproduct.img);
                                    comm.Parameters.AddWithValue("@bid_status",nproduct.bid_status);
                                    comm.Parameters.AddWithValue("@end_date",nproduct.end_date);
                                    comm.Parameters.AddWithValue("@start_price",nproduct.start_price);
                                    comm.Parameters.AddWithValue("@inc",nproduct.inc_price);
                                    comm.Parameters.AddWithValue("@category",nproduct.category);
                                    comm.Parameters.AddWithValue("@sellar_id",nproduct.sellar_id);
                                    comm.Parameters.AddWithValue("@status",nproduct.status);
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
    public class product
    {
        public int id { get; set; }
        public string name { get; set; }

        public string description { get; set; }

        public string img { get; set; }

        public string start_date { get; set; }

        public string end_date { get; set; }

        public string start_price { get; set; }

        public string inc_price { get; set; }

        public int category { get; set; }

        public int bid_status { get; set; }

        public int status { get; set; }
        public int sellar_id { get; set; }

        //for show

        public string cate { get; set; }

        public string sell { get; set; }
    }
}
