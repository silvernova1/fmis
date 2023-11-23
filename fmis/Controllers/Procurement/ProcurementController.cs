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
