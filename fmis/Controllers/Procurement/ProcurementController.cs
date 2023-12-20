using fmis.Models.UserModels;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using fmis.Data;
using fmis.Data.MySql;
using fmis.Filters;
using fmis.Models.Accounting;
using fmis.Models.ppmp;

using fmis.Services;

using iTextSharp.text.pdf;
using iTextSharp.text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Item = fmis.Models.ppmp.Item;
using Font = iTextSharp.text.Font;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using fmis.Models.Procurement;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using iTextSharp.tool.xml;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace fmis.Controllers.Procurement
{

    public class ProcurementController : Controller
    {

        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProcurementController(MyDbContext context, IUserService userService, PpmpContext ppmpContext, DtsContext dts, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
            _ppmpContext = ppmpContext;
            _dts = dts;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult SaveChecklist(PuChecklist puChecklist)
        {
            if (ModelState.IsValid)
            {
                if (puChecklist.Prno != null)
                {
                    puChecklist.PrChecklist = puChecklist.PrChecklist.Where(x => x.IsChecked).ToList();


                    _context.PuChecklist.Add(puChecklist);
                    _context.SaveChanges();


                    return Json(new { success = true, message = "Submitted to End User" });
                }
            }

            return Json(new { success = false, message = "Error submitting the form" });
        }


        //INDEX
        #region
        public IActionResult Index()
        {

            return View();
        }
        #endregion


        //CHECKLIST1
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist1()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist1");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST2
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist2()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist2");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST3
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist3()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist3");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST4
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist4()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist4");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST5
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist5()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist5");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST6
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist6()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist6");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST7
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist7()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist7");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST8
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist8()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist8");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST9
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist9()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist9");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST10
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist10()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist10");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST11
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist11()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist11");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST12
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist12()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist12");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST13
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist13()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist13");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST14
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist14()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist14");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST15
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist15()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist15");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST16
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist16()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist16");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST17
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist17()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist17");

            PrDropDownList();
            return View();
        }
        #endregion

        //CHECKLIST18
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist18()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist18");

            PrDropDownList();
            return View();
        }
        #endregion


        //LOGBOOK BAC RES NO
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult BacResolutionNo()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Bac", "BacResolutionNo");

            var bacResNo = _context.BacResNo.ToList();

            return View(bacResNo);
        }

        public IActionResult SaveBacResNo(BacResNo model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(model.BacResNumber))
                {
                    _context.Add(model);
                    _context.SaveChanges();
                }
                return Json(new { success = true} );
            }
            return Json(new { success = false });
        }

        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult BacResolutionNoEdit()
        {
            return PartialView("BacResolutionNoEdit");
        }

        [HttpGet]
        public IActionResult GetItem(int id)
        {
            var item = _context.BacResNo.Find(id);
            return Json(item);
        }

        [HttpPost]
        public IActionResult UpdateBacResNo([FromBody] BacResNo newData)
        {
            if (ModelState.IsValid)
            {
                if(!String.IsNullOrWhiteSpace(newData.BacResNumber))
                {
                    _context.Update(newData);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        #endregion



        //RMOP SIGNATORY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult RmopSignatory()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Signatory", "RmopSignatory");

            var rmop = _context.Rmop.ToList();
            return View(rmop);
        }
        public IActionResult SaveRmop(Rmop model)
        {
            if (ModelState.IsValid)
            {
                if(!String.IsNullOrWhiteSpace(model.FullName))
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetRmop(int id)
        {
            var item = _context.Rmop.Find(id);
            return Json(item);
        }
        [HttpPost]
        public IActionResult UpdateRmop([FromBody] Rmop newData)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(newData.FullName))
                {
                    _context.Update(newData);
                    _context.SaveChanges();
                }
                return Json(new { success = true } );
            }
            return Json(new { success = false });
        }
        #endregion

        //RMOP SIGNATORY EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult RmopSignatoryEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("RmopSignatoryEdit");
        }
        #endregion


        //RMOP AGENCY TO AGENCY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AgencyToAgency()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "AgencyToAgency");
            BacResNoDownList();
            PrDropDownList();



            return View();
        }

        public IActionResult SavePrAta(RmopAta model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        #endregion

        //RMOP DIRECT CONTRACTING
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult DirectContracting()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "DirectContracting");
            BacResNoDownList();
            PrDropDownList();

            return View();
        }

        public IActionResult SaveRmopDc(RmopDc model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "RMOP DIRECT CONTRACTING (DRUGS AND MEDS) saved successfully." });
                }
            }

            return Json(new { success = false, message = "Error submitting the form" });
        }

        #endregion


        //RMOP DIRECT EMERGENCY CASES
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult EmergencyCases()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "EmergencyCases");
            BacResNoDownList();
            PrDropDownList();

            return View();
        }

        public IActionResult SaveRmopEc(RmopEc model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion


        //RMOP LEASE OF VENUE
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult LeaseOfVenue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "LeaseOfVenue");
            BacResNoDownList();
            PrDropDownList();

            return View();
        }

        public IActionResult SaveRmopLov(RmopLov model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        //RMOP PUBLIC BIDDING
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PublicBidding()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PublicBidding");
            BacResNoDownList();
            PrDropDownList();


            return View();
        }
        public IActionResult SaveRmopPb(RmopPb model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion


        //RMOP PS-DBM
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PsDbm()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PsDbm");
            BacResNoDownList();
            PrDropDownList();

            return View();
        }
        public IActionResult SaveRmopPsDbm(RmopPsDbm model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion


        //RMOP SCIENTIFIC SCHOLARLY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult ScientificScholarly()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "ScientificScholarly");
            BacResNoDownList();
            PrDropDownList();


            return View();
        }

        public IActionResult SaveRmopSs(RmopSs model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        //RMOP SMALL VALUE
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult SmallValue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "SmallValue");
            BacResNoDownList();
            PrDropDownList();


            return View();
        }

        public IActionResult SaveRmopSvp(RmopSvp model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion



        //LOGBOOK CANVASS
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Canvass()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");

            var canvass = _context.Canvass.ToList();

            return View(canvass);
        }

        [HttpPost]
        public IActionResult SaveCanvass(Canvass model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.RpqNo))
                {
                    _context.Canvass.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public IActionResult GetCanvass(int id)
        {
            var item = _context.Canvass.Find(id);
            return Json(item);
        }

        [HttpPost]
        public IActionResult UpdateCanvass([FromBody] Canvass newData)
        {
            if (ModelState.IsValid)
            {
                if (newData.RpqNo != null || newData.RpqNo == "")
                {
                    _context.Canvass.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        //LOGBOOK CANVASS EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult CanvassEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("CanvassEdit");
        }
        #endregion


        //LOGBOOK ABSTRACT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Abstract()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Abstract");
            PrWithDate();
            CanvassWithDate();

            return View(_context.Abstract.ToList());
        }

        public IActionResult SaveAbstract(Abstract model)
        {
            if(ModelState.IsValid)
            {
                if(!String.IsNullOrEmpty(model.AbstractNo))
                {
                    _context.Abstract.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true } );
                }
            }

            return Json(new { success = false });
        }

        public IActionResult GetAbstract(int id)
        {
            var item = _context.Abstract.Find(id);
            if (item != null)
            {
                string formattedAbstractDate = item.AbstractDate.ToString("MMM d, yyyy");

                var result = new
                {
                    AbstractNo = item.AbstractNo,
                    AbstractDate = formattedAbstractDate,
                    PrNoWithDate = item.PrNoWithDate,
                    CanvassNoWithDate = item.CanvassNoWithDate,
                    RecommendedAward = item.RecommendedAward,
                    Rmop = item.Rmop
                };

                return Json(result);
            }

            return NotFound();
        }

        public IActionResult UpdateAbstract([FromBody] Abstract newData)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(newData.AbstractNo))
                {
                    _context.Abstract.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        //LOGBOOK ABSTRACT EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AbstractEdit()
        {
            
            return PartialView("AbstractEdit");
        }
        #endregion


        //LOGBOOK PURCHASE ORDER
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrder()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "PurchaseOrder");
            PrDropDownList();

            return View(_context.Po.ToList());
        }

        public JsonResult GetDate(int prNo)
        {
            var date = _context.Pr.FirstOrDefault(x => x.Id == prNo)?.PrnoDate;

            if (date != null)
            {
                string formattedDate = date.Value.ToString("MMM d, yyyy");

                return Json(formattedDate);
            }

            return Json("Date not found");
        }

        [HttpPost]
        public IActionResult SavePo(Po model)
        {
            if(ModelState.IsValid)
            {
                if(!String.IsNullOrEmpty(model.PoNo))
                {
                    _context.Po.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public IActionResult GetPo(int id)
        {
            var item = _context.Po.Find(id);
            if (item != null)
            {
                string formattedPrDate = item.PrDate.ToString("MMM d, yyyy");
                string formattedPoDate = item.PrDate.ToString("MMM d, yyyy");

                var result = new
                {
                    PoNo = item.PoNo,
                    PoDate = formattedPoDate,
                    PrNo = item.PrNo,
                    PrDate = formattedPrDate,
                    Description = item.Description,
                    Supplier = item.Supplier,
                    Amount = item.Amount,
                    Rmop = item.Rmop,
                    Remarks = item.Remarks
                };

                return Json(result);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdatePo([FromBody] Po newData)
        {
            if (ModelState.IsValid)
            {
                if(!String.IsNullOrEmpty(newData.PoNo))
                {
                    _context.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        //LOGBOOK PURCHASE ORDER EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrderEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("PurchaseOrderEdit");
        }
        #endregion

        //LOGBOOK TWF
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Twg()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Twg");
            PrDropDownList();

            return View(_context.Twg.ToList());
        }

        [HttpPost]
        public IActionResult SaveTwg(Twg model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.TwgNo))
                {
                    _context.Twg.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public IActionResult GetTwg(int id)
        {
            var item = _context.Twg.Find(id);
            if (item != null)
            {
                string formattedTwgDate = item.TwgDate.ToString("MMM d, yyyy");
                string formattedPrDate = item.PrDate.ToString("MMM d, yyyy");

                var result = new
                {
                    TwgNo = item.TwgNo,
                    TwgDate = formattedTwgDate,
                    Recommendation = item.Recommendation,
                    Prno = item.Prno,
                    PrDate = formattedPrDate,
                    ReceivedBy = item.ReceivedBy
                };

                return Json(result);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdateTwg([FromBody] Twg newData)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(newData.TwgNo))
                {
                    _context.Twg.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        //LOGBOOK PURCHASE ORDER EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult TwgEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("TwgEdit");
        }
        #endregion


        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Indexing()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Index", "Indexing");
            return View();
        }
        #endregion

        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Supplier()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Supplier");
            return View();
        }
        #endregion




        private void CanvassWithDate()
        {
            ViewBag.CanvassWithDate = new SelectList((from c in _context.Canvass.Where(x => x.RpqNo != null).ToList()
                                                 select new
                                                 {
                                                     CanvassDate = c.RpqNo + " - " + c.RpqDate.ToString("M-d-yyyy")
                                                 }),
                                     "CanvassDate",
                                     "CanvassDate",
                                     null);;

        }

        private void PrWithDate()
        {
            ViewBag.PrWithDate = new SelectList((from pr in _context.Pr.Where(x => x.Prno != null).ToList()
                                           select new
                                           {
                                               PrWithDate = pr.Prno + " - " + pr.PrnoDate.ToString("M-d-yyyy")
                                           }),
                                     "PrWithDate",
                                     "PrWithDate",
                                     null);

        }

        private void PrDropDownList()
        {
            ViewBag.PrId = new SelectList((from pr in _context.Pr.Where(x=>x.Prno != null).ToList()
                                              select new
                                              {
                                                  Id = pr.Id,
                                                  Prno = pr.Prno
                                              }),
                                     "Id",
                                     "Prno",
                                     null);

        }

        private void BacResNoDownList()
        {
            ViewBag.BacResNo = new SelectList((from bcn in _context.BacResNo.ToList()
                                           select new
                                           {
                                               Id = bcn.Id,
                                               BacResNo = bcn.BacResNumber
                                           }),
                                     "Id",
                                     "BacResNo",
                                     null);

        }

        //PRINT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Print()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");

            return View();
        }
        #endregion


        //PRINT PU
        #region
        public IActionResult PrintPu(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                // Method to add centered paragraph with bold text
                void AddCenteredParagraph(PdfPCell cell, string text, bool isBold = false)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f) : new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    Paragraph centeredParagraph = new Paragraph(text, font);
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    cell.AddElement(centeredParagraph);
                }

                // Method to add cross layout
                void AddCrossLayout()
                {
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                    table.LockedWidth = true;

                    // First row
                    PdfPCell topLeftCell = new PdfPCell();
                    topLeftCell.Border = Rectangle.NO_BORDER;
                    topLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    topLeftCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topLeftCell);
                    table.AddCell(topLeftCell);

                    PdfPCell topRightCell = new PdfPCell();
                    topRightCell.Border = Rectangle.NO_BORDER;
                    topRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    topRightCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topRightCell);
                    table.AddCell(topRightCell);

                    // Second row
                    PdfPCell bottomLeftCell = new PdfPCell();
                    bottomLeftCell.Border = Rectangle.NO_BORDER;
                    bottomLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bottomLeftCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomLeftCell);
                    table.AddCell(bottomLeftCell);

                    PdfPCell bottomRightCell = new PdfPCell();
                    bottomRightCell.Border = Rectangle.NO_BORDER;
                    bottomRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bottomRightCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomRightCell);
                    table.AddCell(bottomRightCell);

                    // Add the table to the document
                    doc.Add(table);
                }

                // Method to add sample paragraphs
                void AddSampleParagraphs(PdfPCell cell)
                {

                    cell.Border = Rectangle.BOX;

                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "With Notice of Award and Contract with");
                    AddCenteredParagraph(cell, "REYNA's THE HAVEN AND GARDENS FOOD");
                    AddCenteredParagraph(cell, "CATERING SERVICES, INC.");
                    AddCenteredParagraph(cell, "Located at New Calceta St., Cogon District,");
                    AddCenteredParagraph(cell, "Tagbiliran City 6300 Bohol");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "LUCHE F. QUIRANTE", isBold: true);
                    AddCenteredParagraph(cell, "Head, BAC Secretariat");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "Noted:");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. JONATHAN NEIL V. ERASMO", isBold: true);
                    AddCenteredParagraph(cell, "Chairperson, BAC for Goods, \n Equipment & Consulting Services");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. SOPHIA M. MANCAO", isBold: true);
                    AddCenteredParagraph(cell, "Vice-Chairperson, BAC for Goods, \n Equipment & Consulting Services");
                    AddCenteredParagraph(cell, "\n");

                }

                // Add the cross layout
                AddCrossLayout();

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT RECOMMENDATION GOODS
        #region
        public IActionResult PrintRecommendationGoods(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                // Method to add centered paragraph with bold text
                void AddCenteredParagraph(PdfPCell cell, string text, bool isBold = false, bool alignLeft = false, float leftMargin = 0)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9) : new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                    Paragraph centeredParagraph = new Paragraph(text, font);
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;



                    if (alignLeft)
                    {
                        centeredParagraph.Alignment = Element.ALIGN_LEFT;
                    }
                    else
                    {
                        centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    }

                    centeredParagraph.IndentationLeft = leftMargin;
                    cell.AddElement(centeredParagraph);
                }

                // Method to add cross layout
                void AddCrossLayout()
                {
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                    table.LockedWidth = true;

                    // First row
                    PdfPCell topLeftCell = new PdfPCell();
                    topLeftCell.Border = Rectangle.NO_BORDER;
                    topLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    topLeftCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topLeftCell);
                    table.AddCell(topLeftCell);

                    PdfPCell topRightCell = new PdfPCell();
                    topRightCell.Border = Rectangle.NO_BORDER;
                    topRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    topRightCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topRightCell);
                    table.AddCell(topRightCell);

                    // Second row
                    PdfPCell bottomLeftCell = new PdfPCell();
                    bottomLeftCell.Border = Rectangle.NO_BORDER;
                    bottomLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bottomLeftCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomLeftCell);
                    table.AddCell(bottomLeftCell);

                    PdfPCell bottomRightCell = new PdfPCell();
                    bottomRightCell.Border = Rectangle.NO_BORDER;
                    bottomRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bottomRightCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomRightCell);
                    table.AddCell(bottomRightCell);

                    // Add the table to the document
                    doc.Add(table);
                }

                // Method to add sample paragraphs
                void AddSampleParagraphs(PdfPCell cell)
                {

                    cell.Border = Rectangle.BOX;



                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DOH CENTRAL VISAYAS", isBold: true);
                    AddCenteredParagraph(cell, "CENTER for HEALTH DEVELOPMENT", isBold: true);
                    AddCenteredParagraph(cell, "BIDS and AWARDS COMMITEE", isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
              
                    AddCenteredParagraph(cell, "PR NO: ___________________" + "                      " + "DATE:___________", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "MEMO TO: BAC SECRETARIAT", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "FINDINGS:", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "RECOMMENDATION:", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "The Bids & Awards Committee:", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "ATTY. MARISSA C. GOROSIN" + "              " + "MR. ROLDAN A. CUBILLO", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "          Provisional Member" + "                                       " + "Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "MR. RAMIL R. ABREA" + "                       " + "ATTY. JO DAVID Z. BORCES", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "              Member" + "                           " + "                        Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. SOPHIA M. MANCAO" + "         " + "DR. JONATHAN NEIL V. ERASMO", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "           Vice-Chairperson" + "                  " + "                    Chairperson", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");

                }

                // Add the cross layout
                AddCrossLayout();

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT RECOMMENDATION MEDS
        #region
        public IActionResult PrintRecommendationMeds(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                // Method to add centered paragraph with bold text
                void AddCenteredParagraph(PdfPCell cell, string text, bool isBold = false, bool alignLeft = false, float leftMargin = 0)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9) : new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                    Paragraph centeredParagraph = new Paragraph(text, font);
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    


                    if (alignLeft)
                    {
                        centeredParagraph.Alignment = Element.ALIGN_LEFT;
                    }
                    else
                    {
                        centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    }

                    centeredParagraph.IndentationLeft = leftMargin;
                    cell.AddElement(centeredParagraph);
                }

                // Method to add cross layout
                void AddCrossLayout()
                {
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                    table.LockedWidth = true;

                    // First row
                    PdfPCell topLeftCell = new PdfPCell();
                    topLeftCell.Border = Rectangle.NO_BORDER;
                    topLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    topLeftCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topLeftCell);
                    table.AddCell(topLeftCell);

                    PdfPCell topRightCell = new PdfPCell();
                    topRightCell.Border = Rectangle.NO_BORDER;
                    topRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    topRightCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topRightCell);
                    table.AddCell(topRightCell);

                    // Second row
                    PdfPCell bottomLeftCell = new PdfPCell();
                    bottomLeftCell.Border = Rectangle.NO_BORDER;
                    bottomLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bottomLeftCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomLeftCell);
                    table.AddCell(bottomLeftCell);

                    PdfPCell bottomRightCell = new PdfPCell();
                    bottomRightCell.Border = Rectangle.NO_BORDER;
                    bottomRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bottomRightCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomRightCell);
                    table.AddCell(bottomRightCell);

                    // Add the table to the document
                    doc.Add(table);
                }

                // Method to add sample paragraphs
                void AddSampleParagraphs(PdfPCell cell)
                {

                    cell.Border = Rectangle.BOX;

                

                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DOH CENTRAL VISAYAS", isBold: true);
                    AddCenteredParagraph(cell, "CENTER for HEALTH DEVELOPMENT", isBold: true);
                    AddCenteredParagraph(cell, "BIDS and AWARDS COMMITEE", isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "PR NO: ___________________" + "                      " + "DATE:___________", alignLeft: true, leftMargin: 10f, isBold:true) ;
                    AddCenteredParagraph(cell, "MEMO TO: BAC SECRETARIAT", alignLeft: true, leftMargin: 10f, isBold: true);
                    AddCenteredParagraph(cell, "FINDINGS:", alignLeft: true, leftMargin: 10f, isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "RECOMMENDATION:", isBold: true, alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "The Bids & Awards Committee:", isBold: true, alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. SHARON ZENITH LAUREL" + "                   " + "DR. NELNER OMUS", isBold: true, alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "          Provisional Member" + "                               " + "Provisional Member", alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "MR. ROLDAN A. CUBILLO" + "                        " + "MR. RAMIL R. ABREA", isBold: true, alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "              Member" + "                           " + "                            Member", alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "ATTY. JO DAVID BORCES" + "                   " + "DR. SOPHIA M. MANCAO", isBold: true, alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "             Member" + "                     " + "                         Vice-Chairperson", alignLeft: true, leftMargin: 10f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. JONATHAN NEIL V. ERASMO", isBold: true);
                    AddCenteredParagraph(cell, "Chairperson");
                    AddCenteredParagraph(cell, "\n");

                }

                // Add the cross layout
                AddCrossLayout();

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT TWG GOODS
        #region
        public IActionResult PrintTWGGoods(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                // Method to add centered paragraph with bold text
                void AddCenteredParagraph(PdfPCell cell, string text, bool isBold = false, bool alignLeft = false, float leftMargin = 0, float rightMargin = 0)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9) : new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                    Paragraph centeredParagraph = new Paragraph(text, font);
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;


                    if (alignLeft)
                    {
                        centeredParagraph.Alignment = Element.ALIGN_LEFT;
                    }
                    else
                    {
                        centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    }

                    centeredParagraph.IndentationLeft = leftMargin;
                    cell.AddElement(centeredParagraph);
                }

                // Method to add cross layout
                void AddCrossLayout()
                {
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                    table.LockedWidth = true;

                    // First row
                    PdfPCell topLeftCell = new PdfPCell();
                    topLeftCell.Border = Rectangle.NO_BORDER;
                    topLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    topLeftCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topLeftCell);
                    table.AddCell(topLeftCell);

                    PdfPCell topRightCell = new PdfPCell();
                    topRightCell.Border = Rectangle.NO_BORDER;
                    topRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    topRightCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topRightCell);
                    table.AddCell(topRightCell);

                    // Second row
                    PdfPCell bottomLeftCell = new PdfPCell();
                    bottomLeftCell.Border = Rectangle.NO_BORDER;
                    bottomLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bottomLeftCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomLeftCell);
                    table.AddCell(bottomLeftCell);

                    PdfPCell bottomRightCell = new PdfPCell();
                    bottomRightCell.Border = Rectangle.NO_BORDER;
                    bottomRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bottomRightCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomRightCell);
                    table.AddCell(bottomRightCell);

                    // Add the table to the document
                    doc.Add(table);
                }

                // Method to add sample paragraphs
                void AddSampleParagraphs(PdfPCell cell)
                {

                    cell.Border = Rectangle.BOX;

                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DOH CENTRAL VISAYAS", isBold: true);
                    AddCenteredParagraph(cell, "CENTER for HEALTH DEVELOPMENT", isBold: true);
                    AddCenteredParagraph(cell, "BIDS and AWARDS COMMITEE", isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "PR NO: ___________________" + "                      " + "DATE:___________", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "MEMO TO: TWG for GOODS", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "FINDINGS:" + "                      "+ "Supplier submitted counter offers", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "RECOMMENDATION" + "     Evaluate quotations as to comppliance", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "                                          " + "with specifications as & requirements of RA");
                    AddCenteredParagraph(cell, "     " + "9184", alignLeft: true, leftMargin: 95f);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "The Bids & Awards Committee:", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "ATTY. MARISSA C. GOROSIN" + "            " + "MR. ROLDAN A. CUBILLO", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "          Provisional Member" + "                                       " + "Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "MR. RAMIL R. ABREA" + "                     " + "ATTY. JO DAVID Z. BORCES", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "              Member" + "                           " + "                        Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. SOPHIA M. MANCAO" + "         " + "DR. JONATHAN NEIL V. ERASMO", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "         Vice-Chairperson" + "                  " + "                    Chairperson", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                }

                // Add the cross layout
                AddCrossLayout();

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT TWG GOODS
        #region
        public IActionResult PrintTWGMeds(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                // Method to add centered paragraph with bold text
                void AddCenteredParagraph(PdfPCell cell, string text, bool isBold = false, bool alignLeft = false, float leftMargin = 0, float rightMargin = 0)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9) : new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                    Paragraph centeredParagraph = new Paragraph(text, font);
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;



                    if (alignLeft)
                    {
                        centeredParagraph.Alignment = Element.ALIGN_LEFT;
                    }
                    else
                    {
                        centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    }

                    centeredParagraph.IndentationLeft = leftMargin;
                    cell.AddElement(centeredParagraph);
                }

                // Method to add cross layout
                void AddCrossLayout()
                {
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                    table.LockedWidth = true;

                    // First row
                    PdfPCell topLeftCell = new PdfPCell();
                    topLeftCell.Border = Rectangle.NO_BORDER;
                    topLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    topLeftCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topLeftCell);
                    table.AddCell(topLeftCell);

                    PdfPCell topRightCell = new PdfPCell();
                    topRightCell.Border = Rectangle.NO_BORDER;
                    topRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    topRightCell.VerticalAlignment = Element.ALIGN_TOP;
                    AddSampleParagraphs(topRightCell);
                    table.AddCell(topRightCell);

                    // Second row
                    PdfPCell bottomLeftCell = new PdfPCell();
                    bottomLeftCell.Border = Rectangle.NO_BORDER;
                    bottomLeftCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bottomLeftCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomLeftCell);
                    table.AddCell(bottomLeftCell);

                    PdfPCell bottomRightCell = new PdfPCell();
                    bottomRightCell.Border = Rectangle.NO_BORDER;
                    bottomRightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bottomRightCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    AddSampleParagraphs(bottomRightCell);
                    table.AddCell(bottomRightCell);

                    // Add the table to the document
                    doc.Add(table);
                }

                // Method to add sample paragraphs
                void AddSampleParagraphs(PdfPCell cell)
                {

                    cell.Border = Rectangle.BOX;

                    AddCenteredParagraph(cell, "DOH CENTRAL VISAYAS", isBold: true);
                    AddCenteredParagraph(cell, "CENTER for HEALTH DEVELOPMENT", isBold: true);
                    AddCenteredParagraph(cell, "BIDS and AWARDS COMMITEE", isBold: true);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "PR NO: ___________________" + "                      " + "DATE:___________", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "MEMO TO: TWG For DRUGS/MEDICINE, MEDICAL", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "                    & DENTAL SUPPLIES/MATERIALS,", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "                    REAGENTS AND ACTIVE INGREDIENTS,", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "FINDINGS:" + "                      " + "Supplier submitted counter offers", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "RECOMMENDATION" + "     Evaluate quotations as to comppliance", alignLeft: true, leftMargin: 7, isBold: true);
                    AddCenteredParagraph(cell, "                                          " + "with specifications as & requirements of RA");
                    AddCenteredParagraph(cell, "     " + "9184", alignLeft: true, leftMargin: 95f);
                    AddCenteredParagraph(cell, "The Bids & Awards Committee:", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. SHARON ZENITH LAUREL" + "                   " + "DR. NELNER OMUS", isBold: true, alignLeft: true, leftMargin : 7);
                    AddCenteredParagraph(cell, "          Provisional Member" + "                          " + "     Provisional Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "MR. ROLDAN A. CUBILLO" + "                        " + "MR. RAMIL R. ABREA", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "              Member" + "                           " + "                        Member", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "ATTY. JO DAVID BORCES" + "                   " + "DR. SOPHIA M. MANCAO", isBold: true, alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "               Member" + "                  " + "                          Vice-Chairperson", alignLeft: true, leftMargin: 7);
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "\n");
                    AddCenteredParagraph(cell, "DR. JONATHAN NEIL V. ERASMO", isBold: true);
                    AddCenteredParagraph(cell, "Chairperson");
                    AddCenteredParagraph(cell, "\n");


                }

                // Add the cross layout
                AddCrossLayout();

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        // PRINT AGENCY TO AGENCY
        #region
        public IActionResult PrintAgencyToAgency(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS,", "NOW THEREFORE,", and "AGENCY TO AGENCY")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE,|AGENCY TO AGENCY)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE," || chunks[i].Trim() == "AGENCY TO AGENCY") ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }




                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 13);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                             “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT:", true, 11);
                AddCellWithContent(nT, "                              NEGOTIATED PROCUREMENT (AGENCY TO AGENCY) UNDER SEC. 53.5 OF", true, 11);
                AddCellWithContent(nT, "                                THE REVISED IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, pursuant to  Sec.  48.2, in  relation  to  Sec. 10 of  the  IRR,  as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as the general method of  procurement  and  that  alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, Sec. 53, in relation to Sec. 53.5, of the IRR  provides  that  Negotiated  Procurement", false, 11);
                AddCellWithContent(nT, "              is a method of procurement of Goods, Infrastructure Projects and Consulting services, whereby  the Procuring", false, 11);
                AddCellWithContent(nT, "              Entity directly negotiates a contract with a technically, legally, and financially capable supplier, contractor, or", false, 11);
                AddCellWithContent(nT, "              consultant in instances such as in AGENCY TO AGENCY  wherein  Procurement  of  Goods,  Infrastructure", false, 11);
                AddCellWithContent(nT, "              Projects and Consulting Services is contracted with another agency of the GoP, such as  the  PS-DBM,  which", false, 11);
                AddCellWithContent(nT, "              is tasked with a centralized procurement of Common-Use Supplies for the GoP in accordance with  Letters  of", false, 11);
                AddCellWithContent(nT, "              Instruction No. 755 and E.O. 359, s. 1989;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  corollary  thereto,  Purchase  Request  No." + "_____________" + " for  the  Procurement  of ", false, 11);
                AddCellWithContent(nT, "              ___________________________________________________________________________________  in the ", false, 11);
                AddCellWithContent(nT, "              amount of PHP" + "_________" + "was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, after evaluation of the purchase request vis-à-vis the  implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing the  same,  the  BAC  has  come  to  a  resolution  that", false, 11);
                AddCellWithContent(nT, "              resort to the Alternative Mode of Procurement: Negotiated Procurement (Agency to Agency)  under  Sec. 53.5", false, 11);
                AddCellWithContent(nT, "              of the IRR would be most advantageous to the  government  and  is  essentially  the  most  applicable  recourse", false, 11);
                AddCellWithContent(nT, "              given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, the BAC shall contract the foregoing procurement with the" + " ___________;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 NOW THEREFORE,  above   premises  considered,   resolved   as   it   is   hereby    resolved,   to", false, 11);
                AddCellWithContent(nT, "              recommend the use of the Alternative  Mode  of  Procurement:  Negotiated  Procurement  (Agency to Agency)", false, 11);
                AddCellWithContent(nT, "              under Sec. 53.5  of  the  Revised  Implementing  Rules  and  Regulations  of  the  Republic  Act No.  9184  for", false, 11);
                AddCellWithContent(nT, "              Purchase Request No." + " __________________" + "in the amount of PHP" + " ________________" + "for the Procurement of", false, 11);
                AddCellWithContent(nT, "              _____________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             DR. ANESSA PATINDOL" + "                        " + "DR. VAN PHILLIP BATON" + "                    " + "MR. RAMIL R. ABREA", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                   " + "Provisional Member" + "                                          " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             MR. ROLDAN A. CUBILLO" + "              " + "ATTY. JO DAVID Z. BORCES" + "                 " + "DR. SOPHIA M. MANCAO", true, 11);
                AddCellWithContent(nT, "                      Provisional Member" + "                                     " + "Provisional Member" + "                                  " + "Vice-Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                           DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                                                                                               Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT DIRECT CONTRACTING
        #region
        public IActionResult PrintDirectContracting(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE,)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE,") ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }


                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "                                                             Osmeña Boulevard,Sambag II, Cebu City, 6000 Philippines", false, 10);
                AddCellWithContent(nT, "                                                     Regional Director’s Office Tel. No. (032) 253-6355 Fax No. (032) 254-0109", false, 10);
                AddCellWithContent(nT, "                                             Official Website http://www.ro7.doh.gov.ph E-mail Address: dohro7@gmail.com", false, 10);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "                             “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT:", true, 11);
                AddCellWithContent(nT, "                              DIRECT CONTRACTING UNDER SEC. 50 OF THE REVISED IMPLEMENTING", true, 11);
                AddCellWithContent(nT, "                                                         RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  pursuant  to   Sec.  48.2,  in  relation  to  Sec. 10 of  the  IRR,  as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as  the  general  method  of  procurement  and  that  alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, Sec. 50 of the IRR provides that Direct Contracting or  single source procurement is  a", false, 11);
                AddCellWithContent(nT, "              method of procurement of Goods that does not require elaborate  Bidding  Documents  and  that  the  supplier  is", false, 11);
                AddCellWithContent(nT, "              simply asked to submit a price quotation or a pro-forma invoice together with the conditions of the sale;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  WHEREAS, the same provision  in the  immediately  preceding  clause  enumerates  the  conditions", false, 11);
                AddCellWithContent(nT, "              material prior to a resort with Direct Contracting, thus: (1) Procurement  of  Goods  of  proprietary  nature  which", false, 11);
                AddCellWithContent(nT, "              can be obtained only from the proprietary source, i.e., when patents, trade secrets, and copyrights prohibit  others", false, 11);
                AddCellWithContent(nT, "              from manufacturing the same item; (2) When the procurement of critical components from a specific  supplier  is", false, 11);
                AddCellWithContent(nT, "              a condition precedent to hold a contractor to guarantee its project performance, in accordance with the provisions", false, 11);
                AddCellWithContent(nT, "              of its contract; or (3) those sold by an exclusive dealer or manufacturer which does not  have  sub-dealers  selling", false, 11);
                AddCellWithContent(nT, "              at lower prices and for which no suitable substitute can be obtained at a more advantageous terms to the GoP;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   WHEREAS,  corollary  thereto,  Purchase  Request  No. _____________  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "              ____________________________________________________________________________  in the amount of ", false, 11);
                AddCellWithContent(nT, "              PHP _____________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   WHEREAS, after evaluation of the purchase request  vis-à-vis  the  implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing the same, the BAC has come to a  resolution  that  resort", false, 11);
                AddCellWithContent(nT, "              to  the  Alternative  Mode  of  Procurement   Direct  Contracting  under  Sec.  50  of  the  IRR   would   be   most", false, 11);
                AddCellWithContent(nT, "              advantageous to the government and is essentially the most applicable recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   NOW THEREFORE, above premises considered, resolved as it is hereby resolved, to  recommend", false, 11);
                AddCellWithContent(nT, "             the use of the Alternative Mode of Procurement Direct Contracting under Sec. 50  of  the  Revised  Implementing", false, 11);
                AddCellWithContent(nT, "             Rules and Regulations of the Republic Act No. 9184 for Purchase Request No. ______________ , in  the  amount", false, 11);
                AddCellWithContent(nT, "             of PHP _________________  for the Procurement of  ________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          DR. SHARON AZENITH LAUREL" + "                        " + "DR. NELNER OMUS" + "                    " + "MR. RAMIL R. ABREA", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                             " + "Provisional Member" + "                                   " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             MR. ROLDAN A. CUBILLO" + "              " + "ATTY. JO DAVID Z. BORCES" + "                 " + "DR. SOPHIA M. MANCAO", true, 11);
                AddCellWithContent(nT, "                 Member (DOH Union Rep)" + "                                     " + "Member" + "                                                  " + "Vice-Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                           DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                                                                                               Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT EMERGENCY CASES
        #region
        public IActionResult PrintEmergencyCases(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE, |EMERGENCY CASES)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE," || chunks[i].Trim() == "EMERGENCY CASES" ) ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }



                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "                                                             Osmeña Boulevard,Sambag II, Cebu City, 6000 Philippines", false, 10);
                AddCellWithContent(nT, "                                                     Regional Director’s Office Tel. No. (032) 253-6355 Fax No. (032) 254-0109", false, 10);
                AddCellWithContent(nT, "                                             Official Website http://www.ro7.doh.gov.ph E-mail Address: dohro7@gmail.com", false, 10);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "                             “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT:", true, 11);
                AddCellWithContent(nT, "                              NEGOTIATED PROCUREMENT (EMERGENCY CASES) UNDER SEC. 53.2 OF", true, 11);
                AddCellWithContent(nT, "                               THE REVISED IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  pursuant  to  Sec.  48.2,  in  relation  to  Sec. 10 of  the  IRR,  as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as  the  general  method  of  procurement  and  that alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, Sec. 53, in relation to Sec. 53.2, of the IRR provides  that  Negotiated  Procurement  is", false, 11);
                AddCellWithContent(nT, "              method of procurement of Goods, Infrastructure Projects and Consulting  services, whereby the Procuring Entity", false, 11);
                AddCellWithContent(nT, "              directly  negotiates  a  contract  with  a  technically,  legally,  and  financially  capable   supplier,   contractor,  or", false, 11);
                AddCellWithContent(nT, "              consultant in instances such as in EMERGENCY CASES as when there is imminent danger  to life or property", false, 11);
                AddCellWithContent(nT, "              during a state of calamity, or when time is of the essence arising from natural or man-made  calamities  or  other", false, 11);
                AddCellWithContent(nT, "              causes where immediate action is necessary to prevent damage to or loss of life or property,  or  to  restore  vital", false, 11);
                AddCellWithContent(nT, "              public services, infrastructure facilities and other public utilities;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  WHEREAS, corollary thereto, Purchase Request No. _________________  for the Procurement of", false, 11);
                AddCellWithContent(nT, "              _______________________________________________________________________  in the amount of  PHP", false, 11);
                AddCellWithContent(nT, "              _____________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  WHEREAS,  after evaluation of the purchase  request  vis-à-vis the  implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing the same, the BAC has come to a  resolution  that  resort", false, 11);
                AddCellWithContent(nT, "              to the Alternative Mode of Procurement: Negotiated Procurement (Emergency Cases) under Sec. 53.2 of the IRR", false, 11);
                AddCellWithContent(nT, "              would be most advantageous to the government and is essentially the most applicable recourse given the availing", false, 11);
                AddCellWithContent(nT, "              circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   NOW THEREFORE, above premises considered, resolved as it is hereby resolved, to recommend", false, 11);
                AddCellWithContent(nT, "             the use of the Alternative Mode of Procurement: Negotiated Procurement (Emergency Cases) under Sec. 53.2  of", false, 11);
                AddCellWithContent(nT, "             the Revised Implementing Rules  and  Regulations  of  the  Republic  Act  No.  9184  for  Purchase  Request  No.", false, 11);
                AddCellWithContent(nT, "             _________________ ,  in  the  amount  of  PHP   ______________________________  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "             _____________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          DR. SHARON AZENITH LAUREL" + "                        " + "DR. NELNER OMUS" + "                    " + "MR. RAMIL R. ABREA", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                             " + "Provisional Member" + "                                   " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             MR. ROLDAN A. CUBILLO" + "              " + "ATTY. JO DAVID Z. BORCES" + "                 " + "DR. SOPHIA M. MANCAO", true, 11);
                AddCellWithContent(nT, "                 Member (DOH Union Rep)" + "                                     " + "Member" + "                                                 " + "Vice-Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                           DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                                                                                               Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT LEASE OF VENUE
        #region
        public IActionResult PrintLeaseOfVenue(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE, |LEASE OF |REAL PROPERTY | AND VENUE)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE," || chunks[i].Trim() == "LEASE OF" || chunks[i].Trim() == "REAL PROPERTY" || chunks[i].Trim() == "AND VENUE" ) ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }

                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                         “RECOMMENDING THE USE OF ALTERNATIVE MODE OF", true, 11);
                AddCellWithContent(nT, "                                   PROCUREMENT: NEGOTIATED PROCUREMENT (LEASE OF REAL", true, 11);
                AddCellWithContent(nT, "                                       PROPERTY AND VENUE) UNDER SEC. 53.10 OF THE REVISED", true, 11);
                AddCellWithContent(nT, "                                         IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               ____________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, pursuant to  Sec. 48.2, in relation to  Sec. 10 of the IRR, as a general rule, the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity  shall  adopt  competitive  bidding  as  the  general  method  of  procurement  and  that ", false, 11);
                AddCellWithContent(nT, "              alternative methods of procurement shall be resorted to only in highly exceptional cases provided for in", false, 11);
                AddCellWithContent(nT, "              the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  Sec. 53,  in  relation  to  Sec.  53.10,  of  the  IRR   provides  that  Negotiated", false, 11);
                AddCellWithContent(nT, "              Procurement is a method of procurement  of  Goods,  Infrastructure  Projects  and  Consulting  services,", false, 11);
                AddCellWithContent(nT, "              whereby the Procuring Entity directly negotiates a contract with a technically,  legally,  and  financially", false, 11);
                AddCellWithContent(nT, "              capable supplier, contractor, or consultant  in  instances  such  as  in  LEASE OF REAL PROPERTY", false, 11);
                AddCellWithContent(nT, "              AND VENUE;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, corollary thereto, Purchase Request No. ____________ for the Procurement of", false, 11);
                AddCellWithContent(nT, "              __________________________________________________________________  in the amount of PHP", false, 11);
                AddCellWithContent(nT, "              __________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  after evaluation of the purchase request vis-à-vis the  implementing  rules  and", false, 11);
                AddCellWithContent(nT, "              upon  confirmation of  the  existence  of  the  conditions  allowing  the  same,  the  BAC  has  come  to  a", false, 11);
                AddCellWithContent(nT, "              resolution that resort to the Alternative Mode of Procurement: Negotiated  Procurement  (Lease  of  Real", false, 11);
                AddCellWithContent(nT, "              Property and Venue) under Sec. 53.10 of the IRR would be most advantageous to the government and is", false, 11);
                AddCellWithContent(nT, "              essentially the most applicable recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 NOW THEREFORE,  above  premises  considered,  resolved  as  it  is hereby  resolved, to", false, 11);
                AddCellWithContent(nT, "              recommend the use of the Alternative Mode of Procurement: Negotiated  Procurement  (Lease  of  Real", false, 11);
                AddCellWithContent(nT, "              Property and Venue) under Sec.  53.10  of  the  Revised  Implementing  Rules  and  Regulations  of  the", false, 11);
                AddCellWithContent(nT, "              Republic  Act  No.  9184  for  Purchase  Request  No. ___________________ , in  the  amount of PHP", false, 11);
                AddCellWithContent(nT, "              ____________ for the Procurement of _______________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "         ATTY. MARISSA C. GOROSIN" + "                        " + "MR. RAMIL R. ABREA" + "                    " + "MR. ROLDAN A. CUBILLO", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                      " + "    Provisional Member" + "                                            " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          ATTY. JO DAVID Z. BORCES" + "              " + "DR. SOPHIA M. MANCAO" + "            " + "DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                              Member" + "                                                " + "Vice-Chairperson" + "                                      " + "Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT PUBLIC BIDDING
        #region
        public IActionResult PrintPublicBidding(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);
                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE,)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE,") ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }

                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                         “RECOMMENDING THE USE OF COMPETITIVE BIDDING (PUBLIC", true, 11);
                AddCellWithContent(nT, "                                       BIDDING) UNDER SEC. 10 OF THE REVISED IMPLEMENTING RULES", true, 11);
                AddCellWithContent(nT, "                                                                     AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  pursuant to Sec. 10 of the IRR, all procurement shall be done through", false, 11);
                AddCellWithContent(nT, "                        competitive bidding, except as provided in Rule XVI;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  corollary  thereto,  Purchase  Request  No. _____________________",  false, 11);
                AddCellWithContent(nT, "                        for  the  Procurement  of  ____________________________________   in  the amount  of  PHP", false, 11);
                AddCellWithContent(nT, "                        __________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS, after evaluation of  the  purchase  request  vis-à-vis  the  implementing ", false, 11);
                AddCellWithContent(nT, "                        rules and finding no substantial ground to depart from the general rule, the BAC has  come  to  a ", false, 11);
                AddCellWithContent(nT, "                        resolution that Competitive Bidding, as the Mode of Procurement, be adopted  for  the  purchase ", false, 11);
                AddCellWithContent(nT, "                        request referred to in the immediately preceding clause;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           NOW THEREFORE, above premises considered, resolved as it is hereby resolved,", false, 11);
                AddCellWithContent(nT, "                        to recommend the use of Competitive Bidding  (Public Bidding)  under  Sec. 10 of  the  Revised", false, 11);
                AddCellWithContent(nT, "                        Implementing Rules and Regulations of the Republic Act  No. 9184  for  Purchase  Request  No.", false, 11);
                AddCellWithContent(nT, "                        _________________ , in  the  amount  of  PHP ___________________  for  the  Procurment  of  ", false, 11);
                AddCellWithContent(nT, "                        _____________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                             _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "         ATTY. MARISSA C. GOROSIN" + "                        " + "MR. RAMIL R. ABREA" + "                    " + "MR. ROLDAN A. CUBILLO", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                      " + "Provisional Member" + "                                            " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                            ATTY. JO DAVID Z. BORCES" + "              " + "" + "                                   " + "DR. SOPHIA M. MANCAO", true, 11);
                AddCellWithContent(nT, "                                                Member" + "                   " + "                                                         " + "Vice-Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);

                AddCellWithContent(nT, "                                                                       DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                                                                                            Chairperson", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion


        //PRINT PS DBM
        #region
        public IActionResult PrintPsDbm(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, bool isItalic = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : (isItalic ? Font.ITALIC : Font.NORMAL));
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);
                    Font italicFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.ITALIC);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;

                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE, |when  there  is  an)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            Font chunkFont;
                            if (chunks[i].Trim() == "when  there  is  an")
                            {
                                chunkFont = italicFont; // Set font to italic
                            }
                            else if (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE,")
                            {
                                chunkFont = boldFont;
                            }
                            else
                            {
                                chunkFont = isItalic ? italicFont : regularFont;
                            }

                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }


                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f, bool isItalic = false)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, isItalic, fontSize);
                    table.AddCell(cell);
                }


                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT", true, 11);
                AddCellWithContent(nT, "                                (PS-DBM), IF NOT AVAILBLE SHOPPING UNDER SEC. 52.1 OF THE REVISED", true, 11);
                AddCellWithContent(nT, "                                            IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  pursuant to  Sec. 48.2,  in  relation to Sec. 10  of  the  IRR, as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as the general method of procurement  and  that  alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                WHEREAS, Sec. 52.1 of the IRR defines  Shopping  as  the  method  of  procurement  of  Goods", false, 11);
                AddCellWithContent(nT, "              whereby the Procuring Entity simply requests for the submission of price quotations for readily available off-", false, 11);
                AddCellWithContent(nT, "              the-shelf goods or ordinary/regular equipment to be procured directly from suppliers of known qualifications", false, 11);
                AddCellWithContent(nT, "              and shall only be employed when  any  of  the  following  cases  are  attendant,  thus:  (1)  when  there  is  an", false, 11);
                AddCellWithContent(nT, "              unforeseen contingency  requiring  immediate  purchase,  provided,  that  the  amount  shall  not  exceed  the", false, 11, 0f, true);
                AddCellWithContent(nT, "              thresholds prescribed by the rules and (2) procurement of ordinary or regular office supplies and equipment", false, 11, 0f, true);
                AddCellWithContent(nT, "              not available in the Procurement Service involving an amount not exceeding the thresholds as prescribed;", false, 11, 0f, true);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, corollary thereto, Purchase  Request  No. ____________  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "              _________________________________________________________________________ in the amount of ", false, 11);
                AddCellWithContent(nT, "              PHP __________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, despite diligent  efforts, the  BAC  found  the  items  covered  in  the  above-stated", false, 11);
                AddCellWithContent(nT, "              Purchase Request were not available in the Procurement Service – Department of  Budget  and Management", false, 11);
                AddCellWithContent(nT, "              (PS-DBM);", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, after evaluating the options provided under the rules and  confirming  the existence", false, 11);
                AddCellWithContent(nT, "              of the conditions allowing the same, the BAC has come to a resolution that resort to the Alternative Mode of", false, 11);
                AddCellWithContent(nT, "              Procurement: Shopping under Sec. 52.1 of the IRR would be most advantageous to  the  government  and  is", false, 11);
                AddCellWithContent(nT, "              essentially the most applicable recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 NOW THEREFORE,   above   premises   considered,   resolved  as  it   is   hereby  resolved, to", false, 11);
                AddCellWithContent(nT, "              recommend the use of the  Alternative  Mode  of  Procurement: Shopping  under  Sec.  52.1  of  the  Revised", false, 11);
                AddCellWithContent(nT, "              Implementing   Rules   and   Regulations   of  the   Republic   Act   No.   9184   for   Purchase   Request   No.", false, 11);
                AddCellWithContent(nT, "              ____________________ , in    the    amount    of  PHP ____________________  for    the    Procurment    of  ", false, 11);
                AddCellWithContent(nT, "              _____________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "         ATTY. MARISSA C. GOROSIN" + "                        " + "MR. RAMIL R. ABREA" + "                    " + "MR. ROLDAN A. CUBILLO", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                          " + "Provisional Member" + "                                             " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          ATTY. JO DAVID Z. BORCES" + "              " + "DR. SOPHIA M. MANCAO" + "            " + "DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                              Member" + "                                                " + "Vice-Chairperson" + "                                      " + "Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT SCIENTIFIC SCHOLARLY
        #region
        public IActionResult PrintScientificScholarly(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified 

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE, |SCIENTIFIC,  SCHOLARLY, OR ARTISTIC WORK, EXCLUSIVE|TECHNOLOGY AND MEDIA SERVICES)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE," || chunks[i].Trim() == "SCIENTIFIC,  SCHOLARLY, OR ARTISTIC WORK, EXCLUSIVE" || chunks[i].Trim() == "TECHNOLOGY AND MEDIA SERVICES") ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }



                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                              “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT:", true, 11);
                AddCellWithContent(nT, "                                 NEGOTIATED PROCUREMENT (SCIENTIFIC, SCHOLARLY, OR ARTISTIC", true, 11);
                AddCellWithContent(nT, "                             WORK EXCLUSIVE TECHNOLOGY & MEDIA SERVICES) UNDER SEC. 53.6 OF", true, 11);
                AddCellWithContent(nT, "                                  THE REVISED IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  pursuant  to  Sec. 48.2,  in  relation  to Sec. 10  of  the  IRR,  as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as the general  method  of  procurement  and  that  alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, Sec. 53, in relation to Sec. 53.6, of the  IRR  provides  that  Negotiated  Procurement", false, 11);
                AddCellWithContent(nT, "              is a method of procurement of Goods, Infrastructure Projects and Consulting services, whereby  the  Procuring", false, 11);
                AddCellWithContent(nT, "              Entity directly negotiates a contract with a technically, legally, and financially capable  supplier, contractor, or", false, 11);
                AddCellWithContent(nT, "              consultant  in  instances such as in  SCIENTIFIC,  SCHOLARLY, OR ARTISTIC WORK, EXCLUSIVE", false, 11);
                AddCellWithContent(nT, "              TECHNOLOGY AND MEDIA SERVICES where it  can  be  contracted to a particular supplier, contractor", false, 11);
                AddCellWithContent(nT, "              or consultant and as determined by the HoPE for any of the requirements enumerated in items 1 and 2 thereof;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  corollary  thereto,  Purchase  Request  No. ____________  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "              ___________________________________________________________________________________ in  the ", false, 11);
                AddCellWithContent(nT, "              amount of PHP __________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  after evaluation of the purchase request vis-à-vis the implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing  the  same,  the  BAC  has  come  to  a  resolution that", false, 11);
                AddCellWithContent(nT, "              resort to the Alternative  Mode  of  Procurement:  Negotiated  Procurement  (Scientific,  Scholarly,  or  Artistic ", false, 11);
                AddCellWithContent(nT, "              Work, Exclusive Technology and Media Services) under Sec. 53.6 of the IRR would be most advantageous  to", false, 11);
                AddCellWithContent(nT, "              the government and is essentially the most applicable recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 NOW THEREFORE,   above   premises   considered,   resolved   as   it   is   hereby   resolved, to", false, 11);
                AddCellWithContent(nT, "              recommend the use of the Alternative Mode of Procurement:  Negotiated  Procurement  (Scientific,  Scholarly,", false, 11);
                AddCellWithContent(nT, "              or Artistic Work, Exclusive Technology and Media Services) under  Sec.  53.6  of  the  Revised  Implementing", false, 11);
                AddCellWithContent(nT, "              Rules and Regulations of the Republic Act No. 9184 for Purchase Request No. ___________________,  in  the", false, 11);
                AddCellWithContent(nT, "              amount of PHP  ____________________________ for the Procurment of  _____________________________ ", false, 11);
                AddCellWithContent(nT, "              _____________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "         ATTY. MARISSA C. GOROSIN" + "                        " + "MR. RAMIL R. ABREA" + "                    " + "MR. ROLDAN A. CUBILLO", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                        " + "Provisional Member" + "                                            " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          ATTY. JO DAVID Z. BORCES" + "              " + "DR. SOPHIA M. MANCAO" + "            " + "DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                              Member" + "                                                " + "Vice-Chairperson" + "                                      " + "Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        //PRINT SMALL VALUE
        #region
        public IActionResult PrintSmallValue(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);


                float totalWidth = doc.PageSize.Width - 50; // Adjust for 50px right margin
                PdfPTable logoTable = new PdfPTable(1);
                logoTable.SetTotalWidth(new float[] { totalWidth });

                // Create a cell for the logo
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = Rectangle.NO_BORDER;

                // Add the cell to the table
                logoTable.AddCell(logoCell);

                // Position the table with a margin top of 50 pixels
                logoTable.WriteSelectedRows(0, -1, 33, doc.PageSize.Height - 20, writer.DirectContent);

                void AddContentToCell(PdfPCell cell, string content, bool isBold = false, int fontSize = 10, float yOffset = 0f)
                {
                    Font regularFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, isBold ? Font.BOLD : Font.NORMAL);
                    Font boldFont = new Font(Font.FontFamily.TIMES_ROMAN, fontSize, Font.BOLD);

                    Paragraph paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_JUSTIFIED;  // Set text alignment to justified

                    // Split the content into chunks based on the target strings ("WHEREAS," and "NOW THEREFORE,")
                    string[] chunks = Regex.Split(content, "(WHEREAS,|NOW THEREFORE, |SMALL VALUE PROCUREMENT)");

                    for (int i = 0; i < chunks.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chunks[i]))
                        {
                            // Add the text with appropriate formatting
                            Font chunkFont = (chunks[i].Trim() == "WHEREAS," || chunks[i].Trim() == "NOW THEREFORE," || chunks[i].Trim() == "SMALL VALUE PROCUREMENT") ? boldFont : regularFont;
                            Chunk chunk = new Chunk(chunks[i], chunkFont);
                            paragraph.Add(chunk);
                        }
                    }

                    paragraph.Alignment = Element.ALIGN_LEFT;
                    paragraph.SetLeading(0, 1);
                    paragraph.SpacingAfter = yOffset;

                    cell.AddElement(paragraph);
                }

                // Method to add a cell with content (without borders)
                void AddCellWithContent(PdfPTable table, string content, bool isBold = false, int fontSize = 10, float cellHeight = 0f)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER; // Remove the border
                    cell.FixedHeight = cellHeight; // Set the height of the cell
                    AddContentToCell(cell, content, isBold, fontSize);
                    table.AddCell(cell);
                }

                PdfPTable nT = new PdfPTable(1);
                nT.TotalWidth = PageSize.A4.Width - 20f; // Adjusted for margins
                nT.LockedWidth = true;

                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                                        Republic Of The Philippines", false, 10);
                AddCellWithContent(nT, "                                                                                              Department Of Health", false, 10);
                AddCellWithContent(nT, "                                 CENTRAL VISAYAS CENTER for HEALTH DEVELOPMENT", true, 12);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO." + "        " + "-AMP s. 2023", true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  “RECOMMENDING THE USE OF ALTERNATIVE MODE OF PROCUREMENT:", true, 11);
                AddCellWithContent(nT, "                                   NEGOTIATED PROCUREMENT (SCIENTIFIC, SCHOLARLY, OR ARTISTIC", true, 11);
                AddCellWithContent(nT, "                            53.9 OF THE REVISED IMPLEMENTING RULES AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS,  pursuant  to  Sec. 48.2,  in  relation  to Sec. 10  of  the  IRR,  as  a  general  rule,  the", false, 11);
                AddCellWithContent(nT, "              Procuring Entity shall adopt competitive bidding as the general  method  of  procurement  and  that  alternative", false, 11);
                AddCellWithContent(nT, "              methods of procurement shall be resorted to only in highly exceptional cases provided for in the rules;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, Sec. 53, in relation to Sec. 53.9, of the  IRR  provides  that  Negotiated  Procurement", false, 11);
                AddCellWithContent(nT, "              is a method of procurement of Goods, Infrastructure Projects and Consulting services, whereby  the  Procuring", false, 11);
                AddCellWithContent(nT, "              Entity directly negotiates a contract with a technically, legally, and financially capable supplier, contractor,  or", false, 11);
                AddCellWithContent(nT, "              consultant in instances such as in SMALL VALUE PROCUREMENT where  the  amount involved does not", false, 11);
                AddCellWithContent(nT, "              exceed the threshold, provided that, in case of Goods, the procurement does not fall under shopping in Sec. 52", false, 11);
                AddCellWithContent(nT, "              of the IRR;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  WHEREAS,  corollary  thereto,  Purchase  Request  No. _______________  for  the Procurement ", false, 11);
                AddCellWithContent(nT, "              of ______________________________________________________________________________________ ", false, 11);
                AddCellWithContent(nT, "              in the amount of PHP __________ was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  WHEREAS, after evaluation of the purchase request vis-à-vis the  implementing  rules  and upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the  conditions allowing the same,  the  BAC  has  come  to  a  resolution  that", false, 11);
                AddCellWithContent(nT, "              resort to the  Alternative Mode  of  Procurement:  Negotiated  Procurement  (Small Value Procurement)  under ", false, 11);
                AddCellWithContent(nT, "              Sec. 53.9 of the IRR would be most advantageous to the government  and   is  essentially  the  most  applicable", false, 11);
                AddCellWithContent(nT, "              recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                  NOW THEREFORE,   above   premises   considered,   resolved   as   it   is   hereby   resolved, to", false, 11);
                AddCellWithContent(nT, "              recommend  the  use  of   the   Alternative   Mode   of  Procurement:   Negotiated   Procurement   (Small Value", false, 11);
                AddCellWithContent(nT, "              Procurement) under Sec. 53.9 of the Revised Implementing Rules and  Regulations  of  the  Republic  Act  No.", false, 11);
                AddCellWithContent(nT, "              R9184  for  Purchase  Request  No. __________, in  the  amount  of  PHP  for  the  _____________________ ", false, 11);
                AddCellWithContent(nT, "              Procurement of  ___________________________________________________________________________  ", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 _____________________" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "         ATTY. MARISSA C. GOROSIN" + "                        " + "MR. RAMIL R. ABREA" + "                    " + "MR. ROLDAN A. CUBILLO", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                        " + "Provisional Member" + "                                            " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "          ATTY. JO DAVID Z. BORCES" + "              " + "DR. SOPHIA M. MANCAO" + "            " + "DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                              Member" + "                                                " + "Vice-Chairperson" + "                                      " + "Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                    Approved:", false, 11);
                AddCellWithContent(nT, "                                                                       JAIME S. BERNADAS, MD, MGM, CESO III", true, 11);
                AddCellWithContent(nT, "                                                                                                 Director IV", false, 11);

                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region LOGIN
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            bool isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "pu_admin":
                        return RedirectToAction("Checklist1", "Procurement");
                    default:
                        return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);
                if (user is not null)
                {
                    user.Year = model.Year.ToString();
                    await LoginAsync(user, model.RememberMe);


                    if (user.Username == "hr_admin")
                    {
                        return RedirectToAction("Checklist1", "Procurement");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Username or Password is Incorrect");
                }

            }
            return View(model);
        }
        #endregion

        #region LOGOUT
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return await Logout();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Scheme4");
            return RedirectToAction("Login", "App");
        }
        #endregion

        #region NOT FOUND
        public new IActionResult NotFound()
        {
            return View();
        }
        #endregion

        #region HELPERS
        private async Task LoginAsync(FmisUser user, bool rememberMe)
        {
            var properties = new AuthenticationProperties
            {
                AllowRefresh = false,
                IsPersistent = rememberMe
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Username.Equals("hr_admin") ? "pu_admin" : null),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname),
            };

            var identity1 = new ClaimsIdentity(claims, "Scheme4");
            var principal1 = new ClaimsPrincipal(identity1);

            await HttpContext.SignInAsync("Scheme4", principal1);
        }
        #endregion


    }
}
