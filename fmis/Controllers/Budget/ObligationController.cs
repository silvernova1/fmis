using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using Syncfusion.EJ2.Grids;
using System.Configuration;
using System.Data.SqlClient;

namespace fmis.Controllers.Grid
{
    public partial class ObligationController : Controller
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ObligrationContext"].ConnectionString;
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            return View();
        }
        public JsonResult GetAllUser(int Id)
        {
            List<Obligation> obligation = new List<Obligation>();
            string query = string.Format("Select * From Obligation", Id);
            SqlConnection connection = new SqlConnection(connectionString);
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        obligation.Add(
                            new Obligation
                            {
                                Id = int.Parse(reader["Id"].ToString()),
                                Date = DateTime.Parse(reader["Date"].ToString()),
                                Pr_no = reader.GetValue(0).ToString(),
                                Po_no = reader.GetValue(0).ToString(),
                                Payee = reader.GetValue(0).ToString(),
                                Address = reader.GetValue(0).ToString(),
                                Particulars = reader.GetValue(0).ToString(),
                                Ors_no = int.Parse(reader["Ors_no"].ToString()),
                                Fund_source = reader.GetValue(0).ToString(),
                                Gross = float.Parse(reader["Gross"].ToString()),
                                Created_by = int.Parse(reader["Created_by"].ToString()),
                                Date_recieved = DateTime.Parse(reader["Date_recieved"].ToString()),
                                Time_recieved = DateTime.Parse(reader["Time_recieved"].ToString()),
                                Date_released = DateTime.Parse(reader["Date_released"].ToString()),
                                Time_released = DateTime.Parse(reader["Time_released"].ToString())

                            }
                        );
                    }
                }
                return Json(obligation, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            }
        }
    }
}

