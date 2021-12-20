using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Donation_Managment_System.Model;
using Newtonsoft.Json;

namespace Donation_Managment_System.Controllers
{
    public class HomeController : Controller
    {
        protected string DBConnection = WebConfigurationManager.AppSettings["Connection"];

        public ActionResult Signup1()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup1(DonorLogin objUser, string user_type)
        {
            //return objUser.Password;
            string query = "";
            bool viewinfo = false;
            string email = objUser.Email;

            if(objUser.Password != objUser.ConfirmPassword)
            {
                View("PasswordError");
            }
            
            if (user_type == "Donor")
            {
                query = "insert into dbo.Donor values(@First_Name,@Last_name,@Email,@Password,@Gender,@Phone_no,@Occupation,@Cnic,@block)";
                //query = "select * from dbo.Donor";
            }
            else
            {
                query = "insert into dbo.Recipient values(@First_Name,@Last_name,@Email,@Password,@Gender,@Phone_no,@Occupation,@Cnic,@block)";
            }

            using (SqlConnection connection = new SqlConnection(DBConnection))
            {
                SqlCommand command = new SqlCommand(query, connection);


                command.Parameters.AddWithValue("@First_Name", objUser.First_name);
                command.Parameters.AddWithValue("@Last_name", objUser.Last_name);
                command.Parameters.AddWithValue("@Email", objUser.Email);
                command.Parameters.AddWithValue("@Password", objUser.Password);
                command.Parameters.AddWithValue("@Gender", objUser.Gender);
                command.Parameters.AddWithValue("@Phone_no", objUser.Phone_no);
                command.Parameters.AddWithValue("@Occupation", objUser.Occupation);
                command.Parameters.AddWithValue("@Cnic", objUser.Cnic);
                command.Parameters.AddWithValue("@block", 0);

                connection.Open();

                try { command.ExecuteNonQuery(); viewinfo = true; }
                catch (Exception e) { Response.Write("Email ID OR CNIC Already Exist");Response.Write(e); }

                connection.Close();

            }

            var user = new Dictionary<string, string>
            {
                { "email", email},
                { "first_name", objUser.First_name},
                { "Last_name", objUser.Last_name},
                { "User_type", user_type}
            };
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var response = await client.PostAsync("https://dfd7-39-51-33-68.ngrok.io", data);
            string result = await response.Content.ReadAsStringAsync();

            if (viewinfo == true)
            {
                System.Web.HttpContext.Current.Session["email"] = objUser.Email;

                if (user_type == "Donor")
                {
                    System.Web.HttpContext.Current.Session["user"] = "donor";
                    return View("DonationFront", "donor_navbar");
                }
                else
                {
                    System.Web.HttpContext.Current.Session["user"] = "recipient";
                    return View("RecipientFront", "recipient_navbar");
                }
                    
            }

            return View("ErrorSignup");
        }
        public ActionResult BlockedErrorLogin()
        {
            return View();
        }
        public ActionResult InvalidBG()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string email = form["email"].ToString();
            string pass = form["password"].ToString();

            //return DBConnection;

            using (SqlConnection connection = new SqlConnection(DBConnection))
            {
                //Check if user is blocked
                string query = "Select * from dbo.Blocked where Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandText = query;
                command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = email;
                connection.Open();
                SqlDataReader reader;
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    return View("BlockedErrorLogin");
                }
                reader.Close();
                command.Dispose();

                //Check if donor login
                query = "Select * from dbo.Donor where Email = @Email and Password = @Password";
                //return query;
                command = new SqlCommand(query, connection);
                // SqlDataAdapter adapter = new SqlDataAdapter();

                command.CommandText = query;
                command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = email;
                command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 50).Value = pass;

                
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    System.Web.HttpContext.Current.Session["email"] = email;
                    System.Web.HttpContext.Current.Session["user"] = "donor";
                    return View("DonationFront", "donor_navbar");
                }
                reader.Close();
                command.Dispose();


                //Check if recipient login
                query = "Select * from dbo.Recipient where Email = @Email and Password = @Password";
                command = new SqlCommand(query, connection);
                // SqlDataAdapter adapter = new SqlDataAdapter();

                command.CommandText = query;
                command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = email;
                command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 50).Value = pass;


                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    System.Web.HttpContext.Current.Session["email"] = email;
                    System.Web.HttpContext.Current.Session["user"] = "recipient";
                    return View("RecipientFront", "recipient_navbar");
                }
                reader.Close();
                command.Dispose();


                //Check if admin login
                query = "Select * from dbo.Admin where Email = @Email and Password = @Password";
                command = new SqlCommand(query, connection);
                // SqlDataAdapter adapter = new SqlDataAdapter();

                command.CommandText = query;
                command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = email;
                command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 50).Value = pass;


                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    System.Web.HttpContext.Current.Session["email"] = email;
                    System.Web.HttpContext.Current.Session["user"] = "admin";
                    return View("AdminFront", "admin_navbar");
                }
                reader.Close();
                connection.Close();


            }

            return View("ErrorLogin");
            //return email;
            //return View();
        }

        public ActionResult ErrorLogin()
        {
            return View();
        }
        /*public ActionResult recipient_navbar()
        {
            return View();
        }*/

        public ActionResult donation()
        {
            //return "hello";

            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "donor")
            {
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    string query = "Select * from dbo.Proff where Approval = 1";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //StringBuilder strResults = new StringBuilder();

                        Proff p = new Proff();
                        List<Proff> objmodel = new List<Proff>();

                        while (reader.Read())
                        {

                            var data = new Proff();

                            data.Recipient_id = reader["recipient_Id"].ToString();
                            data.Blood_Group = reader["Blood_Group"].ToString();
                            data.Quantity = reader["Quantity_ltr"].ToString();
                            data.Medical_report = reader["Medical_Report"].ToString();
                            data.Conclusion_Rating = reader["Conclusion_Rating"].ToString();
                            data.Approval = reader["Approval"].ToString();

                            objmodel.Add(data);
                        }
                        p.proffs = objmodel;

                        ViewBag.status = true;
                        return View("donation", "donor_navbar", p);
                    }
                    else
                    {
                        ViewBag.status = false;
                        //Response.Write("status false");
                        return View("donation", "donor_navbar");
                    }
                }
            }
            return View("Login");
        }


        public ActionResult Logout()
        {
            Session.Contents.RemoveAll();
            return View("Login");
        }

        public ActionResult appointment()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null)
            {
                return View();
            }
            return View("Login");


        }

        public ActionResult ExtraQuantityError()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null)
            {
                return View();
            }
            return View("Login");


        }
        [HttpPost]
        public ActionResult appoint_post(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "donor")
            {
                int reci_id = Int32.Parse(form["reci_id"]);
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    //get donor id
                    string query = "Select * from dbo.Donor where Email = @Email";
                    //return query;
                    SqlCommand command = new SqlCommand(query, connection);
                    // SqlDataAdapter adapter = new SqlDataAdapter();

                    command.CommandText = query;
                    command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = System.Web.HttpContext.Current.Session["email"].ToString();

                    connection.Open();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return View("error");
                    }
                    reader.Read();
                    int donor_id = (int)reader["donor_Id"];
                    reader.Close();
                    command.Dispose();


                    //check wheather a recipient with such id exist
                    query = "Select * from dbo.Proff where recipient_Id = @reci_id and Approval = 1";
                    //return query;
                    command = new SqlCommand(query, connection);
                    // SqlDataAdapter adapter = new SqlDataAdapter();

                    command.CommandText = query;
                    command.Parameters.Add("@reci_id", System.Data.SqlDbType.VarChar, 50).Value = reci_id;


                    reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return View("Error_Appoint");
                    }
                    reader.Close();
                    command.Dispose();


                    //if request is already pending
                    query = "Select * from dbo.MedicalReport where donor_id = @donor_id and Compatibility is null";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.Add("@donor_id", System.Data.SqlDbType.Int).Value = donor_id;
                    //command.Parameters.Add("@comp", System.Data.SqlDbType.VarChar, 50).Value = null;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return View("error");
                    }
                    reader.Close();
                    command.Dispose();


                    //Insert data into Medical Report
                    query = "insert into dbo.MedicalReport (donor_id) values (@donor_id)";
                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@donor_id", donor_id);

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }


                    //get the report ID
                    query = "Select * from dbo.MedicalReport where donor_id = @donor_id and Compatibility is null";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.Add("@donor_id", System.Data.SqlDbType.Int).Value = donor_id;
                    //command.Parameters.Add("@comp", System.Data.SqlDbType.VarChar, 50).Value = null;
                    reader = command.ExecuteReader();
                    reader.Read();
                    int report_id = (int)reader["report_Id"];

                    reader.Close();
                    command.Dispose();




                    //Insert data into History
                    query = "insert into dbo.History (donor_id, recipient_id,quantity,report_id) values (@donor_id,@recipient_id,0,@report_id)";
                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@donor_id", donor_id);
                    command.Parameters.AddWithValue("@recipient_id", reci_id);
                    command.Parameters.AddWithValue("@report_id", report_id);
                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }


                }
                return View("appointment_set");
            }
            return View("Login");
        }

        public ActionResult appointment_set()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "donor")
            {
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    string query = "Select * from dbo.Donor where Email = @Email";
                    //return query;
                    SqlCommand command = new SqlCommand(query, connection);
                    // SqlDataAdapter adapter = new SqlDataAdapter();

                    command.CommandText = query;
                    command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = System.Web.HttpContext.Current.Session["email"].ToString();

                    connection.Open();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return View("error");
                    }
                    reader.Read();
                    int donor_id = (int)reader["donor_Id"];

                    reader.Close();
                    command.Dispose();


                    //if request is already pending
                    query = "Select * from dbo.MedicalReport where donor_id = @donor_id and Compatibility is NULL";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.Add("@donor_id", System.Data.SqlDbType.Int).Value = donor_id;
                    //command.Parameters.Add("@comp", System.Data.SqlDbType.VarChar, 50).Value = null;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return View("error");
                    }
                    reader.Close();
                    command.Dispose();




                    query = "insert into dbo.MedicalReport (donor_id) values (@donor_id)";
                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@donor_id", donor_id);

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }

                    //get the report ID
                    query = "Select * from dbo.MedicalReport where donor_id = @donor_id and Compatibility is null";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.Add("@donor_id", System.Data.SqlDbType.Int).Value = donor_id;
                    reader = command.ExecuteReader();
                    reader.Read();
                    int report_id = (int)reader["report_Id"];

                    reader.Close();
                    command.Dispose();


                    //Insert data into History
                    query = "insert into dbo.History (donor_id,quantity,report_id) values (@donor_id,0,@report_id)";
                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@donor_id", donor_id);
                    command.Parameters.AddWithValue("@report_id", report_id);
                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }

                    connection.Close();
                }
                return View();
            }
            return View("Login");


        }

        public ActionResult error()
        {

            if (System.Web.HttpContext.Current.Session["email"] != null)
            {
                return View();
            }
            return View("Login");

        }

        public ActionResult DonationFront()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "donor")
            {
                return View("DonationFront", "donor_navbar");
            }
            return View("Login");
        }

        public ActionResult RecipientFront()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                return View("RecipientFront", "recipient_navbar");
            }
            return View("Login");
        }

        public ActionResult requestSubmit()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                return View();
            }
            return View("Login");
        }

        public ActionResult AdminFront()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                return View("AdminFront", "admin_navbar");
            }
            return View("Login");
        }


        public ActionResult Error_Appoint()
        {
            return View();
        }

        public ActionResult Reci_BloodForm()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                return View("Reci_BloodForm");
            }
            return View("Login");
        }

        public ActionResult recipient()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                return View("recipient");
            }
            return View("Login");
        }

        public ActionResult RecipientFileUpload(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                return View();
            }
            return View("Login");
        }

            [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                string format = "Mddyyyyhhmmsstt";
                string timeStamp = String.Format("{0}", DateTime.Now.ToString(format));
                string _FileName = Path.GetFileName(file.FileName);
                string f_name = timeStamp + _FileName;
                string _path = Path.Combine(Server.MapPath("~/ProffImages"), _FileName);
                file.SaveAs(_path);

                string newPath = Path.Combine(Server.MapPath("~/ProffImages"), f_name);
                System.IO.File.Move(_path, newPath);

                string email = System.Web.HttpContext.Current.Session["email"].ToString();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    string query = "Select * from dbo.Recipient where Email = @email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", email);
                    DonorLogin p = new DonorLogin();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();

                    reader.Read();
                    int reci_id = Int32.Parse(reader["recipient_Id"].ToString());

                    reader.Close();
                    command.Dispose();

                    query = "update dbo.Proff set Medical_Report = @mr where recipient_Id = @rid";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    //byte[] file = form["Blood_Med"];
                    

                    command.Parameters.AddWithValue("@rid", reci_id);
                    command.Parameters.AddWithValue("@mr", f_name);

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }
                }
            }
            return View("requestSubmit");
        }

        [HttpPost]
        public ActionResult reci_BloodData(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                string email = System.Web.HttpContext.Current.Session["email"].ToString();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    //get recipient id
                    string query = "Select * from dbo.Recipient where Email = @email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", email);
                    DonorLogin p = new DonorLogin();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();

                    reader.Read();
                    int reci_id = Int32.Parse(reader["recipient_Id"].ToString());
                                            
                    reader.Close();
                    command.Dispose();
                    

                    //Check if request is already pending
                    query = "Select * from dbo.Proff where recipient_Id = @rid";
                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@rid", reci_id);

                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return View("error");
                    }
                    reader.Close();
                    command.Dispose();

                    query = "insert into dbo.Proff (recipient_Id,Blood_Group,Quantity_ltr,Conclusion_Rating,Approval) values (@rid,@bg,@q,@cr,0)";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    //byte[] file = form["Blood_Med"];
                    //string timeStamp = DateTime.Now.ToString();
                    //string report_name = timeStamp + form["Blood_Med"].ToString();

                    command.Parameters.AddWithValue("@rid", reci_id);
                    command.Parameters.AddWithValue("@bg", form["Blood_Grp"]);
                    command.Parameters.AddWithValue("@q", form["Blood_Quantity"]);
                    //command.Parameters.AddWithValue("@MR", report_name);

                    int priority_value = Int32.Parse(System.Web.HttpContext.Current.Session["priority"].ToString());

                    string bg = form["Blood_Grp"];
                    if (bg == "O-" || bg == "AB+" || bg == "AB-" || bg == "O+")
                    {
                        priority_value += 2;
                    }
                    if(Int32.Parse(form["Blood_Quantity"]) > 3)
                        priority_value += 3;

                    command.Parameters.AddWithValue("@cr", priority_value);

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }




                }
                return View("RecipientFileUpload");
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult BloodPriorityValue(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "recipient")
            {
                int p_val = 0;
                string bp = form["bp1"].ToString();

                if (bp == "Average") p_val += 1;
                else if (bp == "Severe") p_val += 2;
                else if (bp == "Critical") p_val += 3;

                bp = form["bp2"].ToString();

                if (bp == "Can wait for some time") p_val += 1;
                else if (bp == "Urgent") p_val += 2;
                else if (bp == "Extremely Urgent") p_val += 3;

                bp = form["bp3"].ToString();

                if (bp == "No") p_val += 1;
                else if (bp == "Yes") p_val += 2;

                bp = form["bp4"].ToString();

                if (bp == "Others") p_val += 1;
                else if (bp == "Organ malfunction") p_val += 2;
                else if (bp == "Accident") p_val += 3;

                bp = form["bp5"].ToString();

                if (bp == "Rich") p_val += 1;
                else if (bp == "Middle") p_val += 2;
                else if (bp == "Poor") p_val += 3;

                System.Web.HttpContext.Current.Session["priority"] = p_val;

                return recipient();
            }
            return View("Login");
        }

        public ActionResult Admin_Blockuser()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                Blocked b = new Blocked();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    string query = "Select * from dbo.Blocked";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    

                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        List<Blocked> objmodel1 = new List<Blocked>();

                        while (reader.Read())
                        {

                            var data = new Blocked();

                            data.Id = reader["Blocked_id"].ToString();
                            data.Email = reader["Email"].ToString();
                            data.Cnic = reader["Cnic"].ToString();

                            objmodel1.Add(data);
                        }
                        b.blocked = objmodel1;

                        ViewBag.status1 = true;

                    }

                    reader.Close();
                    command.Dispose();
                }
                return View("Admin_Blockuser", "admin_navbar", b);
             
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Admin_BlockManage(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    string query = "Delete from dbo.Blocked where Email = @email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", form["email"].ToString());

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }

                    command.Dispose();

                    bool donor = false;
                    query = "Select * from dbo.Donor where Email = @email";
                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", form["email"].ToString());
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        donor = true;
                    }

                    reader.Close();
                    command.Dispose();

                    if(donor)
                    {
                        query = "update dbo.Donor set Blocked = 0 where Email = @email";
                    }
                    else
                    {
                        query = "update dbo.Recipient set Blocked = 0 where Email = @email";
                    }
                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", form["email"].ToString());

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }
                }
                return Admin_Blockuser();
            }
            return View("Login");
        }
        public ActionResult Admin_appointment()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                MedicalReport m = new MedicalReport();
                History h = new History();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    //string query = "Select * from dbo.MedicalReport m inner join dbo.History h on m.report_Id = h.report_id where Compatibility is null";

                    string query = "Select * from dbo.MedicalReport where Compatibility is null";
                    string query2 = "Select * from dbo.History";

                    SqlCommand command1 = new SqlCommand(query, connection);
                    command1.CommandText = query;
                    SqlCommand command2 = new SqlCommand(query2, connection);
                    command2.CommandText = query2;

                    connection.Open();
                    SqlDataReader reader;
                    reader = command1.ExecuteReader();
                    List<MedicalReport> objmodel1 = new List<MedicalReport>();
                    if (reader.HasRows)
                    {
                        

                        while (reader.Read())
                        {

                            var data = new MedicalReport();

                            data.donor_id = Int32.Parse(reader["donor_id"].ToString());

                            data.report_id = Int32.Parse(reader["report_Id"].ToString());
                            data.Compatibility = reader["Compatibility"].ToString();
                            data.Blood_Group = reader["Blood_Group"].ToString();

                            objmodel1.Add(data);
                        }
                        m.MR = objmodel1;
                    }
                    reader.Close();
                    command1.Dispose();

                    reader = command2.ExecuteReader();
                    List<History> objmodel2 = new List<History>();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            var data = new History();

                            data.donor_id = Int32.Parse(reader["donor_id"].ToString());

                            data.report_id = Int32.Parse(reader["report_Id"].ToString());
                            if (DBNull.Value.Equals(reader["recipient_id"]))
                            {
                                data.recipient_id = -1;
                            }
                            else
                            {
                                data.recipient_id = Int32.Parse(reader["recipient_id"].ToString());
                            }
                            data.quantity = Int32.Parse(reader["quantity"].ToString());

                            objmodel2.Add(data);
                        }
                        h.history = objmodel2;
                    }


                    ViewData["appointment"] = from mr in objmodel1
                                      join hty in objmodel2 on mr.report_id equals hty.report_id into table1
                                      from hty in table1.DefaultIfEmpty()
                                      select new ViewModel
                                      {
                                          medicalReport = mr,
                                          history = hty
                                      };
                }


                 return View("Admin_appointment","admin_navbar",ViewData["appointment"]);
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Admin_Aprove_App(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                string BG_check = form["blood_group"].ToString();
                if (Int32.Parse(form["compatibility"]) > 2)
                {
                    if(form["recipient_id"].ToString() != "") //Donation to specific person
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query = "Select * from dbo.Proff where recipient_Id = @reciID and Approval = 1";

                            SqlCommand command = new SqlCommand(query, connection);
                            command.CommandText = query;

                            command.Parameters.AddWithValue("@reciID", Int32.Parse(form["recipient_id"]));
                            
                            SqlDataReader reader;
                            reader = command.ExecuteReader();
                            var data = new Proff();
                            if (reader.HasRows)
                            {
                                reader.Read();
                                
                                data.Recipient_id = reader["recipient_Id"].ToString();
                                data.Blood_Group = reader["Blood_Group"].ToString();
                                data.Quantity = reader["Quantity_ltr"].ToString();
                                data.Medical_report = reader["Medical_Report"].ToString();
                                data.Conclusion_Rating = reader["Conclusion_Rating"].ToString();
                                data.Approval = reader["Approval"].ToString();
                            }
                            reader.Close();
                            command.Dispose();
                            if (data.Blood_Group == "AB+" || BG_check == "O-" || data.Blood_Group == BG_check)
                            {
                                if (Int32.Parse(form["quantity"]) > Int32.Parse(data.Quantity))
                                {
                                    return View("ExtraQuantityError");
                                }
                                else
                                {
                                
                                    query = "update dbo.History set quantity = @q where donor_id = @did and recipient_id = @rid and report_id = @rpid";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;
                                    command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                    command.Parameters.AddWithValue("@rid", Int32.Parse(form["recipient_id"]));
                                    command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                    command.Parameters.AddWithValue("@q", Int32.Parse(form["quantity"]));

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();
                                    if (Int32.Parse(form["quantity"]) == Int32.Parse(data.Quantity))
                                    {
                                        query = "delete from dbo.Proff where recipient_Id = @rid";
                                        command = new SqlCommand(query, connection);
                                        Response.Write(query);
                                    }
                                    else
                                    {
                                        query = "update dbo.Proff set Quantity_ltr = @q where recipient_Id = @rid";
                                        command = new SqlCommand(query, connection);
                                        int quan = Int32.Parse(data.Quantity) - Int32.Parse(form["quantity"]);
                                        command.Parameters.AddWithValue("@q", quan);
                                        Response.Write(quan);
                                    }
                                    

                                    command.Parameters.AddWithValue("@rid", Int32.Parse(form["recipient_id"]));
                                    command.CommandText = query;
                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                }
                            }
                            else
                            {
                                return View("InvalidBG");
                            }


                            connection.Close();
                        }
                        //return "1";
                    }
                    else //Donation system way
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query;
                            SqlCommand command;
                            int total = Int32.Parse(form["quantity"]);
                            while (total != 0)
                            {
                                if (BG_check == "O-")
                                {
                                    query = "Select * from dbo.Proff where  Approval = 1 order by Conclusion_Rating desc";
                                    command = new SqlCommand(query, connection);
                                }
                                else
                                {
                                    query = "Select * from dbo.Proff where (Approval = 1 and Blood_Group = @bg1) or (Approval = 1 and Blood_Group = @bg2) order by Conclusion_Rating desc";
                                    command = new SqlCommand(query, connection);
                                    command.Parameters.AddWithValue("@bg1", "AB+");
                                    command.Parameters.AddWithValue("@bg2", BG_check);
                                }


                                command.CommandText = query;

                                
                                SqlDataReader reader;
                                reader = command.ExecuteReader();
                                Proff data = new Proff();
                                if (reader.HasRows)
                                {
                                    reader.Read();

                                    data.Recipient_id = reader["recipient_Id"].ToString();
                                    data.Blood_Group = reader["Blood_Group"].ToString();
                                    data.Quantity = reader["Quantity_ltr"].ToString();
                                    data.Medical_report = reader["Medical_Report"].ToString();
                                    data.Conclusion_Rating = reader["Conclusion_Rating"].ToString();
                                    data.Approval = reader["Approval"].ToString();

                                    reader.Close();

                                    if (total > Int32.Parse(data.Quantity))
                                    {
                                        query = "update dbo.History set quantity = @q,recipient_id = @rid where donor_id = @did and quantity = 0 and report_id = @rpid";
                                        command = new SqlCommand(query, connection);

                                        command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));
                                        command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                        command.Parameters.AddWithValue("@q", Int32.Parse(data.Quantity));
                                        command.CommandText = query;
                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }

                                        command.Dispose();

                                        total = total - Int32.Parse(data.Quantity);

                                        query = "delete from dbo.Proff where recipient_Id = @rid";
                                        command = new SqlCommand(query, connection);
                                        command.CommandText = query;
                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));

                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }


                                        reader.Close();
                                        /*if (BG_check == "O-")
                                        {
                                            query = "Select * from dbo.Proff where Approval = 1 order by Conclusion_Rating desc";
                                            command = new SqlCommand(query, connection);
                                        }
                                        else
                                        {
                                            query = "Select * from dbo.Proff where (Approval = 1 and Blood_Group = @bg1) or (Approval = 1 and Blood_Group = @bg2) order by Conclusion_Rating desc";
                                            command = new SqlCommand(query, connection);
                                            command.Parameters.AddWithValue("@bg1", "AB+");
                                            command.Parameters.AddWithValue("@bg2", BG_check);
                                        }

                                        command.CommandText = query;

                                        reader = command.ExecuteReader();
                                        if (reader.HasRows)
                                        {*/
                                            query = "insert into dbo.History (donor_id,report_id,quantity) values (@did,@rpid,0)";
                                            command.Dispose();
                                            reader.Close();
                                            command = new SqlCommand(query, connection);
                                            command.CommandText = query;

                                            command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                            command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                            
                                            try { command.ExecuteNonQuery(); }
                                            catch (Exception e) { Response.Write(e); }

                                        /*}
                                        reader.Close();
                                        command.Dispose();*/


                                    }
                                    else if (total == Int32.Parse(data.Quantity))
                                    {
                                        //Enter finalized data in History
                                        query = "update dbo.History set quantity = @q,recipient_id = @rid where donor_id = @did and quantity = 0 and report_id = @rpid";
                                        command = new SqlCommand(query, connection);

                                        command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));
                                        command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                        command.Parameters.AddWithValue("@q", Int32.Parse(data.Quantity));
                                        command.CommandText = query;
                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }

                                        command.Dispose();

                                        //Delete entry from proff (Donation is done)
                                        query = "delete from dbo.Proff where recipient_Id = @rid";
                                        command = new SqlCommand(query, connection);

                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));
                                        command.CommandText = query;
                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }

                                        command.Dispose();
                                        total = 0;

                                    }
                                    else
                                    {
                                        //Enter finalized data in History
                                        query = "update dbo.History set quantity = @q,recipient_id = @rid where donor_id = @did and quantity = 0 and report_id = @rpid";
                                        command = new SqlCommand(query, connection);

                                        command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));
                                        command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                        command.Parameters.AddWithValue("@q", total);
                                        command.CommandText = query;
                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }

                                        command.Dispose();

                                        //Update entry from proff (Donation is done)
                                        query = "update dbo.Proff set Quantity_ltr = @q where recipient_Id = @rid";
                                        command = new SqlCommand(query, connection);

                                        command.Parameters.AddWithValue("@rid", Int32.Parse(data.Recipient_id));
                                        int new_quantity = Int32.Parse(data.Quantity) - total;
                                        command.Parameters.AddWithValue("@q", new_quantity);
                                        command.CommandText = query;
                                        try { command.ExecuteNonQuery(); }
                                        catch (Exception e) { Response.Write(e); }

                                        total = 0;
                                    }

                                }
                                else
                                {
                                    query = "insert into dbo.Inventory (donor_id,quantity,report_id) values (@did,@q,@rpid)";

                                    reader.Close();
                                    command.Dispose();
                                    command = new SqlCommand(query, connection);
                                    command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                                    command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                                    command.Parameters.AddWithValue("@q", total);
                                    command.CommandText = query;
                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }
                                    total = 0;
                                }
                                reader.Close();
                                command.Dispose();
                                
                            }
                            connection.Close();
                        }
                    }
                    

                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DBConnection))
                    {
                        connection.Open();
                        string query = "delete from dbo.History where donor_id = @did and quantity = 0 and report_id = @rpid";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@did", Int32.Parse(form["donor_id"]));
                        command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));
                        command.CommandText = query;
                        try { command.ExecuteNonQuery(); }
                        catch (Exception e) { Response.Write(e); }

                        command.Dispose();
                        connection.Close();
                    }
                }
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    string query = "update dbo.MedicalReport set Blood_Group = @bg, Compatibility = @c where report_id = @rpid";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@bg", BG_check);
                    command.Parameters.AddWithValue("@c", form["compatibility"].ToString());
                    command.Parameters.AddWithValue("@rpid", Int32.Parse(form["report_id"]));

                    try { command.ExecuteNonQuery(); }
                    catch (Exception e) { Response.Write(e); }

                    command.Dispose();
                    connection.Close();
                }
                //return form["blood_group"].ToString();
                return Admin_appointment();
            }
            //return "123";
            return View("Login");
        }

        public ActionResult Admin_request()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                Proff p = new Proff();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    string query = "Select * from dbo.Proff where Approval = 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    connection.Open();

                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //StringBuilder strResults = new StringBuilder();

                        List<Proff> objmodel = new List<Proff>();

                        while (reader.Read())
                        {

                            var data = new Proff();

                            data.Recipient_id = reader["recipient_Id"].ToString();
                            data.Blood_Group = reader["Blood_Group"].ToString();
                            data.Quantity = reader["Quantity_ltr"].ToString();
                            data.Medical_report = reader["Medical_Report"].ToString();
                            data.Conclusion_Rating = reader["Conclusion_Rating"].ToString();
                            data.Approval = reader["Approval"].ToString();

                            objmodel.Add(data);
                            ViewBag.status = true;
                        }
                        p.proffs = objmodel;

                    }

                    reader.Close();
                    command.Dispose();
                }
                    return View("Admin_request","admin_navbar",p);
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Admin_ManageRequest(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                string query = "";
                int reci_id = Int32.Parse(form["recipient_id"].ToString());
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    if (form["approval"].ToString() == "approve")
                    {
                        //request is Approved...set Approval = 1
                        query = "update dbo.Proff set Approval = 1 where recipient_Id = @rid";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@rid", reci_id);

                        try { command.ExecuteNonQuery(); }
                        catch (Exception e) { Response.Write(e); }

                        command.Dispose();


                        //Getting the row data of the specific recipient from Proff table
                        query = "Select * from dbo.Proff where Approval = 1 and recipient_Id = @rid";

                        command = new SqlCommand(query, connection);
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@rid", reci_id);

                        SqlDataReader reader;
                        reader = command.ExecuteReader();

                        reader.Read();
                        var data = new Proff();

                        data.Recipient_id = reader["recipient_Id"].ToString();
                        data.Blood_Group = reader["Blood_Group"].ToString();
                        data.Quantity = reader["Quantity_ltr"].ToString();
                        data.Medical_report = reader["Medical_Report"].ToString();
                        data.Conclusion_Rating = reader["Conclusion_Rating"].ToString();
                        data.Approval = reader["Approval"].ToString();

                        reader.Close();
                        command.Dispose();

                        int total = Int32.Parse(data.Quantity);

                        string query2 = "";
                        while(total != 0)
                        {
                            if(data.Blood_Group != "AB+")
                            {
                                query2 = "select * from dbo.Inventory";
                                query = "select * from dbo.MedicalReport where (Blood_Group = @bg or Blood_Group = @bg2)";
                                
                            }
                            else
                            {
                                query2 = "select * from dbo.Inventory";
                                query = "select * from dbo.MedicalReport where Compatibility is not null";
                            }

                            SqlCommand command1 = new SqlCommand(query, connection);
                            command1.CommandText = query;
                            SqlCommand command2 = new SqlCommand(query2, connection);
                            command2.CommandText = query2;

                            if(data.Blood_Group != "AB+")
                            {
                                command1.Parameters.AddWithValue("@bg", "O-");
                                command1.Parameters.AddWithValue("@bg2", data.Blood_Group);
                            }

                            reader.Close();

                            MedicalReport MR_data = new MedicalReport();
                            reader = command1.ExecuteReader();
                            List<MedicalReport> objmodel1 = new List<MedicalReport>();
                            if (reader.HasRows)
                            {
                                reader.Read();
                                
                                MR_data.donor_id = Int32.Parse(reader["donor_id"].ToString());

                                MR_data.report_id = Int32.Parse(reader["report_Id"].ToString());
                                MR_data.Compatibility = reader["Compatibility"].ToString();
                                MR_data.Blood_Group = reader["Blood_Group"].ToString();

                                objmodel1.Add(MR_data);
                                
                            }
                            reader.Close();
                            command1.Dispose();

                            Inventory inv_data = new Inventory();
                            reader = command2.ExecuteReader();
                            List<Inventory> objmodel2 = new List<Inventory>();
                            if (reader.HasRows)
                            {
                                reader.Read();

                                //var inv_data = new Inventory();

                                inv_data.donor_id = Int32.Parse(reader["donor_id"].ToString());
                                inv_data.blood_id = Int32.Parse(reader["blood_id"].ToString());
                                inv_data.report_id = Int32.Parse(reader["report_Id"].ToString());
                                inv_data.quantity = Int32.Parse(reader["quantity"].ToString());

                                objmodel2.Add(inv_data);
                                
                                //inven.Inv = objmodel2;
                            }
                            reader.Close();
                            command.Dispose();
                            command1.Dispose();
                            command2.Dispose();

                            var inventory = from inv in objmodel2
                                              join mr in objmodel1 on inv.report_id equals mr.report_id into table1
                                              from mr in table1.DefaultIfEmpty()
                                              select new ViewModel
                                              {
                                                  inventory = inv,
                                                  medicalReport = mr
                                              };

                            if (inventory != null && inventory.Any())
                            {
                                int quantity = 0;
                                int blood_id = 0, report_id = 0, donor_id = 0;
                                foreach (var item in inventory)
                                {
                                    quantity = item.inventory.quantity;
                                    blood_id = item.inventory.blood_id;
                                    report_id = item.inventory.report_id;
                                    donor_id = item.inventory.donor_id;
                                    break;
                                }

                                if (quantity > total)
                                {
                                    int n_quantity = quantity - total;

                                    //update new quantity in left in inventory
                                    query = "update dbo.Inventory set quantity = @q where blood_id = @bid and report_id = @rid";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@bid", blood_id);
                                    command.Parameters.AddWithValue("@rid", report_id);
                                    command.Parameters.AddWithValue("@q", n_quantity);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    //update history i.e. Donation has been made
                                    query = "update dbo.History set quantity = @q, recipient_id = @rid where donor_id = @did and report_id = @rpid and quantity = 0";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@did", donor_id);
                                    command.Parameters.AddWithValue("@rpid", report_id);
                                    command.Parameters.AddWithValue("@rid", reci_id);
                                    command.Parameters.AddWithValue("@q", total);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    // Delete Proff Entry
                                    query = "delete from dbo.Proff where recipient_Id = @rid and Approval = 1";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@rid", reci_id);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    //new item for the donation made in future by the donor
                                    query = "insert into dbo.History (donor_id,report_id,quantity) values(@did,@rpid,@q)";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@did", donor_id);
                                    command.Parameters.AddWithValue("@rpid", report_id);
                                    command.Parameters.AddWithValue("@q", 0);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();
                                    total = 0;
                                }
                                else if (quantity == total)
                                {
                                    //Delete item from inventory
                                    query = "delete from dbo.Inventory where blood_id = @bid and report_id = @rid";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@bid", blood_id);
                                    command.Parameters.AddWithValue("@rid", report_id);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    //update history i.e. Donation has been made
                                    query = "update dbo.History set quantity = @q, recipient_id = @rid where donor_id = @did and report_id = @rpid and quantity = 0";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@did", donor_id);
                                    command.Parameters.AddWithValue("@rpid", report_id);
                                    command.Parameters.AddWithValue("@rid", reci_id);
                                    command.Parameters.AddWithValue("@q", total);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    // Delete Proff Entry
                                    query = "delete from dbo.Proff where recipient_Id = @rid and Approval = 1";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@rid", reci_id);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    total = 0;
                                }
                                else
                                {
                                    total = total - quantity;

                                    //Update data in recipient request table i.e. dbo.Proff
                                    query = "update dbo.Proff set Quantity_ltr = @q where recipient_Id = @rid and Approval = 1";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@rid", reci_id);
                                    command.Parameters.AddWithValue("@q", total);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    // Delete Inventry Item
                                    query = "delete from dbo.Inventory where blood_id = @bid and report_id = @rpid";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@bid", blood_id);
                                    command.Parameters.AddWithValue("@rpid", report_id);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                    //update history i.e. Donation has been made
                                    query = "update dbo.History set quantity = @q, recipient_id = @rid where donor_id = @did and report_id = @rpid and quantity = 0";
                                    command = new SqlCommand(query, connection);
                                    command.CommandText = query;

                                    command.Parameters.AddWithValue("@did", donor_id);
                                    command.Parameters.AddWithValue("@rpid", report_id);
                                    command.Parameters.AddWithValue("@rid", reci_id);
                                    command.Parameters.AddWithValue("@q", quantity);

                                    try { command.ExecuteNonQuery(); }
                                    catch (Exception e) { Response.Write(e); }

                                    command.Dispose();

                                }
                            }
                            else
                            {
                                total = 0;
                            }

                        }
                    }
                    else if (form["approval"].ToString() == "reject") //request is reject hence delete entry
                    {
                        query = "delete from dbo.Proff where recipient_Id = @rid";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@rid", reci_id);

                        try { command.ExecuteNonQuery(); }
                        catch (Exception e) { Response.Write(e); }

                        command.Dispose();
                    }
                }
                        return Admin_request();
            }
            return View("Login");
        }

        public ActionResult Admin_ManageUser()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                DonorLogin p = new DonorLogin();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    string query = "Select * from dbo.donor where Blocked = 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    connection.Open();
                    
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //StringBuilder strResults = new StringBuilder();

                        
                        List<DonorLogin> objmodel1 = new List<DonorLogin>();

                        while (reader.Read())
                        {

                            var data = new DonorLogin();

                            data.id = reader["donor_Id"].ToString();
                            
                            data.Email = reader["Email"].ToString();
                            data.Gender = reader["Gender"].ToString();
                            data.Phone_no = reader["Phone_no"].ToString();
                            data.Occupation = reader["Occupation"].ToString();
                            data.Cnic = reader["Cnic"].ToString();
                            data.First_name = reader["First_Name"].ToString();
                            data.Last_name = reader["Last_Name"].ToString();

                            objmodel1.Add(data);
                        }
                        p.donor = objmodel1;

                        ViewBag.status1 = true;
                        
                    }

                    reader.Close();
                    command.Dispose();

                    query = "Select * from dbo.recipient where Blocked = 0";

                    command = new SqlCommand(query, connection);
                    command.CommandText = query;

                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //StringBuilder strResults = new StringBuilder();

                        
                        List<DonorLogin> objmodel2 = new List<DonorLogin>();

                        while (reader.Read())
                        {

                            var data = new DonorLogin();

                            data.id = reader["recipient_Id"].ToString();
                            data.First_name = reader["First_Name"].ToString();
                            data.Last_name = reader["Last_Name"].ToString();
                            data.Email = reader["Email"].ToString();
                            data.Gender = reader["Gender"].ToString();
                            data.Phone_no = reader["Phone_no"].ToString();
                            data.Occupation = reader["Occupation"].ToString();
                            data.Cnic = reader["Cnic"].ToString();

                            objmodel2.Add(data);
                        }
                        p.recipient = objmodel2;

                        ViewBag.status2 = true;

                    }

                    
                    /*else
                    {
                        ViewBag.status = false;
                        Response.Write("status false");
                        return View("donation", "donor_navbar");
                    }*/
                }
                return View("Admin_ManageUser", "admin_navbar", p);
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Admin_ManageNow(FormCollection form)
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                int id = Int32.Parse(form["id"]);
                string email = form["email"].ToString();
                string cnic = form["cnic"].ToString();

                if(form["user_kind"].ToString() == "donor")
                {
                    if(form["handle"].ToString() == "block")
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query = "insert into dbo.Blocked (Email,Cnic) values (@email,@cnic)";
                            SqlCommand command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@cnic", cnic);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }

                            command.Dispose();


                            query = "update dbo.Donor set Blocked = 1 where donor_Id = @id";
                            command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@id", id);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }

                            command.Dispose();

                        }
                        //return "block";
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query = "delete from dbo.Donor where donor_Id = @donor_id";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.CommandText = query;

                            command.Parameters.AddWithValue("@donor_id", id);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }
                            //return "remove";
                        }
                    }
                }
                else
                {
                    if (form["handle"].ToString() == "block")
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query = "insert into dbo.Blocked (Email,Cnic) values (@email,@cnic)";
                            SqlCommand command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@cnic", cnic);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }

                            command.Dispose();


                            query = "update dbo.Recipient set Blocked = 1 where recipient_Id = @id";
                            command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@id", id);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }

                            command.Dispose();

                        }
                        //return "block";
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(DBConnection))
                        {
                            connection.Open();
                            string query = "delete from dbo.Recipient where recipient_Id = @reci_id";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.CommandText = query;

                            command.Parameters.AddWithValue("@reci_id", id);

                            try { command.ExecuteNonQuery(); }
                            catch (Exception e) { Response.Write(e); }
                            //return "remove";
                        }
                    }
                    //return "recipent";
                }
                return Admin_ManageUser();
                //return View("Admin_ManageUser", "admin_navbar");
            }
            //return "manage";
            return View("Login");
        }

        public ActionResult Admin_ViewHistory()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                History h = new History();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    connection.Open();
                    string query = "Select * from dbo.History where quantity <> 0 and recipient_id is not null";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandText = query;
                    SqlDataReader reader = command.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        //StringBuilder strResults = new StringBuilder();

                        
                        List<History> objmodel = new List<History>();

                        while (reader.Read())
                        {

                            var data = new History();

                            data.donor_id = Int32.Parse(reader["donor_id"].ToString());
                            data.recipient_id = Int32.Parse(reader["recipient_id"].ToString());
                            data.report_id = Int32.Parse(reader["report_id"].ToString());
                            data.quantity = Int32.Parse(reader["quantity"].ToString());

                            objmodel.Add(data);

                        }
                        h.history = objmodel;

                        ViewBag.status = true;

                    }
                    else
                    {
                        ViewBag.status = false;
                    }
                }

                return View("Admin_ViewHistory","admin_navbar",h);
            }
            return View("Login");

        }

        public ActionResult Admin_Inventory()
        {
            if (System.Web.HttpContext.Current.Session["email"] != null && System.Web.HttpContext.Current.Session["user"].ToString() == "admin")
            {
                MedicalReport m = new MedicalReport();
                Inventory h = new Inventory();
                using (SqlConnection connection = new SqlConnection(DBConnection))
                {
                    //string query = "Select * from dbo.MedicalReport m inner join dbo.History h on m.report_Id = h.report_id where Compatibility is null";

                    string query = "Select * from dbo.MedicalReport where Compatibility is not null";
                    string query2 = "Select * from dbo.Inventory where quantity <> 0";

                    SqlCommand command1 = new SqlCommand(query, connection);
                    command1.CommandText = query;
                    SqlCommand command2 = new SqlCommand(query2, connection);
                    command2.CommandText = query2;

                    connection.Open();
                    SqlDataReader reader;
                    reader = command1.ExecuteReader();
                    List<MedicalReport> objmodel1 = new List<MedicalReport>();
                    if (reader.HasRows)
                    {
                        

                        while (reader.Read())
                        {

                            var data = new MedicalReport();

                            data.donor_id = Int32.Parse(reader["donor_id"].ToString());

                            data.report_id = Int32.Parse(reader["report_Id"].ToString());
                            data.Compatibility = reader["Compatibility"].ToString();
                            data.Blood_Group = reader["Blood_Group"].ToString();

                            objmodel1.Add(data);
                        }
                        m.MR = objmodel1;
                    }
                    reader.Close();
                    command1.Dispose();

                    reader = command2.ExecuteReader();
                    List<Inventory> objmodel2 = new List<Inventory>();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            var data = new Inventory();

                            data.donor_id = Int32.Parse(reader["donor_id"].ToString());

                            data.report_id = Int32.Parse(reader["report_id"].ToString());

                            data.blood_id = Int32.Parse(reader["blood_id"].ToString());

                            data.quantity = Int32.Parse(reader["quantity"].ToString());

                            objmodel2.Add(data);
                        }
                        h.Inv = objmodel2;
                    }


                    ViewData["inv"] = from inv in objmodel2
                                      join mr in objmodel1 on inv.report_id equals mr.report_id into table1
                                      from mr in table1.DefaultIfEmpty()
                                      select new ViewModel
                                      {
                                          inventory = inv,
                                          medicalReport = mr
                                      };
                }


                 return View("Admin_Inventory","admin_navbar",ViewData["inv"]);
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Admin_ViewProff(FormCollection form)
        {
            ViewBag.report = form["proof"];
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}