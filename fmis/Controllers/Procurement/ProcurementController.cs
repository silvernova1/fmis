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
using fmis.Models.pr;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.SignalR;
using fmis.Hubs;

namespace fmis.Controllers.Procurement
{

    public class ProcurementController : Controller
    {

        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly fmisContext _dtsContext;
        private readonly IHubContext<PrStatus> _hubContext;

        public ProcurementController(MyDbContext context, IUserService userService, PpmpContext ppmpContext, DtsContext dts, IHttpContextAccessor httpContextAccessor, fmisContext dtsContext, IHubContext<PrStatus> hubContext)
        {
            _context = context;
            _userService = userService;
            _ppmpContext = ppmpContext;
            _dts = dts;
            _httpContextAccessor = httpContextAccessor;
            _dtsContext = dtsContext;
            _hubContext = hubContext;

        }

        #region PR TRACKING
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseRequest()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "PurchaseRequest", "");

            var pr = _context.Pr.ToList();

            return View(pr);
        }

        [HttpPost]
        public async Task<IActionResult> AddRouteNumber(int id, string routeNumber)
        {
            var pr = await _context.Pr.FirstOrDefaultAsync(x => x.Id == id);

            pr.RouteNumber = routeNumber;

            _context.Pr.Update(pr);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> PrRecieve(int id)
        {
            var pr = await _context.Pr.FirstOrDefaultAsync(x => x.Id == id);

            pr.IsReceiveOnPU = !pr.IsReceiveOnPU;

            _context.Pr.Update(pr);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        #endregion

        #region PR STATUS
        public IActionResult PrStatus(int id)
        {
            var active = _context.RmopAta.FirstOrDefault(x => x.Id == id);

            return Ok();
        }
        #endregion


        [HttpGet]
        public IActionResult GetChecklist(int id)
        {
            var prChecklist = _context.PuChecklist
            .Where(x => x.Prno == id)
            .ToList();


            return Json(prChecklist);
        }

        #region SAVE CHECKLIST
        public IActionResult SaveChecklist(PuChecklist puChecklist)
        {
            if (ModelState.IsValid)
            {
                if (puChecklist.Prno != 0)
                {
                    puChecklist.PrChecklist = puChecklist.PrChecklist.Where(x => x.IsChecked).ToList();
                    puChecklist.PrTrackingChecklist = DateTime.Now;

					_context.PuChecklist.Add(puChecklist);
                    _context.SaveChanges();

                    var pr = _context.Pr.FirstOrDefault(x => x.Id == Convert.ToInt32(puChecklist.Prno));

                    pr.Rmop = "Checklist 1";
                    _context.Pr.Update(pr);
                    _context.SaveChanges();


                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region INDEX
        public IActionResult Index()
        {

            return View();
        }
        #endregion

        #region CHECKLIST1
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist1()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist1");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST2
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist2()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist2");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST3
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist3()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist3");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST4
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist4()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist4");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST5
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist5()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist5");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST6
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist6()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist6");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST7
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist7()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist7");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST8
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist8()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist8");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST9
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist9()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist9");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST10
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist10()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist10");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST11
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist11()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist11");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST12
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist12()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist12");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST13
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist13()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist13");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST14
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist14()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist14");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST15
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist15()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist15");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST16
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist16()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist16");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST17
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist17()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist17");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST18
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist18()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist18");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST19
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist19()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist19");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST20
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist20()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist20");

            PrDropDownList();
            return View();
        }
        #endregion

        #region CHECKLIST21
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist21()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist21");

            PrDropDownList();
            return View();
        }
        #endregion

        #region LOGBOOK BAC RES NO
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
                return Json(new { success = true });
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
                if (!String.IsNullOrWhiteSpace(newData.BacResNumber))
                {
                    _context.Update(newData);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        #endregion

        #region RMOP SIGNATORY
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
                if (!String.IsNullOrWhiteSpace(model.FullName))
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
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        #endregion

        #region RMOP SIGNATORY EDIT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult RmopSignatoryEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("RmopSignatoryEdit");
        }
        #endregion

        #region RMOP AGENCY TO AGENCY
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AgencyToAgency()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "AgencyToAgency");
            BacResNoDownList();
            PrDdl();

            return View(_context.RmopAta.ToList());
        }

        public IActionResult SavePrAta(RmopAta model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;
                    model.PrTrackingDate = DateTime.Now;

                    _context.Add(model);
                    _context.SaveChanges();

                    string htmlContent = $@"
                    <div role='tabpanel' class='bs-stepper-pane fade active dstepper-block' id='rmopDate'>
                        <div class='form-group'>
                            <label for='inputMailForm'>{model.PrTrackingDate.ToString("MMM d, yyyy")}</label>
                            <br />
                            <br />
                            <b>Remarks</b>
                            <div class='invalid-feedback'>Sample Remarks</div>
                        </div>
                    </div>";

                    _hubContext.Clients.All.SendAsync("PrUpdateRmop", model.PrNoOne, htmlContent);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        public IActionResult AddRemarks(int id, string remarks)
        {
            var rmopAta = _context.RmopAta.FirstOrDefault(x=>x.Id == id);

            if(rmopAta != null)
            {
                rmopAta.Remarks = remarks;

                _context.RmopAta.Update(rmopAta);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });

        }

        #endregion

        #region RMOP DIRECT CONTRACTING
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult DirectContracting()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "DirectContracting");
            BacResNoDownList();
            PrDdl();

            return View(_context.RmopDc.ToList());
        }

        public IActionResult SaveRmopDc(RmopDc model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        #region RMOP DIRECT EMERGENCY CASES
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult EmergencyCases()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "EmergencyCases");
            BacResNoDownList();
            PrDdl();

            return View(_context.RmopEc.ToList());
        }

        public IActionResult SaveRmopEc(RmopEc model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;
                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        #region RMOP LEASE OF VENUE
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult LeaseOfVenue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "LeaseOfVenue");
            BacResNoDownList();
            PrDdl();

            return View(_context.RmopLov.ToList());
        }

        public IActionResult SaveRmopLov(RmopLov model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;

                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region RMOP PUBLIC BIDDING GOODS AND EQUIPMENT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PublicBidding()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PublicBidding");
            BacResNoDownList();
            PrDdl();


            return View(_context.RmopPb.ToList());
        }


        //RMOP PUBLIC BIDDING INFRA
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PublicBiddingInfra()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PbInfra");
            BacResNoDownList();
            PrDdl();


            return View(_context.RmopPb.ToList());
        }
        #endregion


        public IActionResult SaveRmopPb(RmopPb model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;

                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region RMOP PS-DBM
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PsDbm()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PsDbm");
            BacResNoDownList();
            PrDdl();

            return View(_context.RmopPsDbm.ToList());
        }
        public IActionResult SaveRmopPsDbm(RmopPsDbm model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;

                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region RMOP SCIENTIFIC SCHOLARLY
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult ScientificScholarly()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "ScientificScholarly");
            BacResNoDownList();
            PrDdl();


            return View(_context.RmopSs.ToList());
        }

        public IActionResult SaveRmopSs(RmopSs model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;

                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region RMOP SMALL VALUE
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult SmallValue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "SmallValue");
            BacResNoDownList();
            PrDdl();


            return View(_context.RmopSvp.ToList());
        }

        public IActionResult SaveRmopSvp(RmopSvp model)
        {
            if (ModelState.IsValid)
            {
                if (model.BacNo != null)
                {
                    var userId = _context.Pr.FirstOrDefault(x => x.Prno == model.PrNoOne).UserId;
                    model.UserId = userId;

                    _context.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region CANVASS
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Canvass()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");

            var canvass = _context.Canvass.ToList();
            PrDdl();

            ViewBag.addReCanvass = _context.AddReCanvass.ToList();

            return View(canvass);
        }

        [HttpPost]
        public IActionResult SaveCanvass(Canvass model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.RpqNo))
                {
                    model.PrTrackingDate = DateTime.Now;
                    _context.Canvass.Add(model);
                    _context.SaveChanges();

                    string htmlContent = $@"
                    <div role='tabpanel' class='bs-stepper-pane fade active dstepper-block'>
                        <div class='form-group' id='rmopDiv'>
                            <label for='inputMailForm'>{model?.SubmissionDate.ToString("MMM d, yyyy")}</label>
                            <br />
                            <br />
                            <b>Remarks</b>
                            <div class='invalid-feedback'>{model?.Remarks}</div>
                        </div>
                    </div>";

                    _hubContext.Clients.All.SendAsync("PrUpdateCanvass", model.PrNo, htmlContent);
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public IActionResult GetCanvass(int id)
        {
            var item = _context.Canvass.Find(id);
            if (item != null)
            {
                string formattedRpqDate = item.RpqDate.ToString("MMM d, yyyy");
                string formattedPrDate = item.PrDate.ToString("MMM d, yyyy");
                string formattedSubDate = item.SubmissionDate.ToString("MMM d, yyyy");

                var result = new
                {
                    RpqNo = item.RpqNo,
                    RpqDate = formattedRpqDate,
                    PrNo = item.PrNo,
                    PrDate = formattedPrDate,
                    SubDate = formattedSubDate,
                    ItemDesc = item.ItemDesc,
                    Remarks = item.Remarks,
                    Rmop = item.Rmop
                };

                return Json(result);
            }

            return NotFound();
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

        [HttpGet]
        [Route("Procurement/Additional")]
        public IActionResult AdditionalOrReCanvass(int canvassId)
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");

            ViewBag.Canvass = canvassId;
            PrDdl();

            var additional = _context.AddReCanvass.FirstOrDefault(x=>x.CanvassId == canvassId && x.Step == "Additional");

            return View(additional);
        }
        [HttpPost]
        public IActionResult SaveAr(AddReCanvass model)
        {
            if(ModelState.IsValid)
            {
                var canvassStep = "Additional";
                model.Step = canvassStep;



                _context.AddReCanvass.Add(model);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [Route("Procurement/Second")]
        public IActionResult SecondAdditionalOrReCanvass(int canvassId)
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");
            ViewBag.Canvass = canvassId;
            PrDdl();

            var second = _context.AddReCanvass.FirstOrDefault(x => x.CanvassId == canvassId && x.Step == "Second");

            return View(second);
        }

        [HttpPost]
        public IActionResult SaveSar(AddReCanvass model)
        {
            if (ModelState.IsValid)
            {
                var canvassStep = "Second";
                model.Step = canvassStep;

                _context.AddReCanvass.Add(model);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [Route("Procurement/Third")]
        public IActionResult ThirdAdditionalOrReCanvass(int canvassId)
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");
            ViewBag.Canvass = canvassId;
            PrDdl();

            var third = _context.AddReCanvass.FirstOrDefault(x => x.CanvassId == canvassId && x.Step == "Third");

            return View(third);
        }

        [HttpPost]
        public IActionResult SaveTar(AddReCanvass model)
        {
            if (ModelState.IsValid)
            {
                var canvassStep = "Third";
                model.Step = canvassStep;

                _context.AddReCanvass.Add(model);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult AddReCanvass(AddReCanvass formData)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(formData.RpqNo))
                {
                    _context.AddReCanvass.Add(formData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }



        #region LOGBOOK CANVASS EDIT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult CanvassEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("CanvassEdit");
        }
        #endregion

        #region RECANVASS ADDITIONAL
        [HttpGet]
        public IActionResult ReCanvassAdditional()
        {
            // You can pass any necessary model or data to the partial view
            return PartialView("ReCanvassAdditional");
        }
        #endregion

        #region LOGBOOK ABSTRACT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Abstract()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Abstract");
            PrWithDate();
            CanvassWithDate();
            CanvassDropDown();

            return View(_context.Abstract.ToList());
        }

        public IActionResult SaveAbstract(Abstract model)
        {
            if(ModelState.IsValid)
            {
				var PrNo = model.PrNoWithDate?.Split('/')[0]?.Trim();
                model.PrNo = PrNo;
				if (!String.IsNullOrEmpty(model.AbstractNo))
                {
                    model.PrTrackingDate = DateTime.Now;
                    _context.Abstract.Add(model);
                    _context.SaveChanges();

					string htmlContent = $@"
                    <div role='tabpanel' class='bs-stepper-pane fade active dstepper-block'>
                        <div class='form-group'>
                            <label for='inputMailForm'>{model?.AbstractDate.ToString("MMM d, yyyy")}</label>
                            <br />
                            <br />
                            <b>Remarks</b>
                            <div class='invalid-feedback'>{model?.Remarks}</div>
                        </div>
                    </div>";

					_hubContext.Clients.All.SendAsync("PrUpdateAbstract", model.PrNo, htmlContent);

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
                    Rmop = item.Rmop,
                    Remarks = item.Remarks
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

        #region LOGBOOK ABSTRACT EDIT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AbstractEdit()
        {
            
            return PartialView("AbstractEdit");
        }
        #endregion

        #region LOGBOOK PURCHASE ORDER
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrder()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "PurchaseOrder");
            PrDdl();
            AbstractDropDown();


			string year = DateTime.Now.Year.ToString();

            int latestPoNo = _context.Po.Where(x => x.PoNo.StartsWith(year)).AsEnumerable().Select(x => int.Parse(x.PoNo.Substring(year.Length + 1))).DefaultIfEmpty(0).Max();
            latestPoNo++;

			string formattedPoNo = $"{year}-{latestPoNo:D4}";

            ViewBag.FormattedPoNo = formattedPoNo;

            return View(_context.Po.ToList());
        }

        public JsonResult GetDate(string prNo)
        {
            var date = _context.Pr.FirstOrDefault(x => x.Prno == prNo)?.PrnoDate;

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
                    model.PrTrackingDate = DateTime.Now;

                    _context.Po.Add(model);
                    _context.SaveChanges();

					string htmlContent = $@"
                    <div role='tabpanel' class='bs-stepper-pane fade active dstepper-block'>
                        <div class='form-group' id='rmopDiv'>
                            <label for='inputMailForm'>{model?.PoDate.ToString("MMM d, yyyy")}</label>
                            <br />
                            <br />
                            <b>Remarks</b>
                            <div class='invalid-feedback'>{model?.Remarks}</div>
                        </div>
                    </div>";

					_hubContext.Clients.All.SendAsync("PrUpdatePo", model.PrNo, htmlContent);
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
                    AbstractNo = item.AbstractNo,
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

        #region LOGBOOK PURCHASE ORDER EDIT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrderEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("PurchaseOrderEdit");
        }
        #endregion

        #region LOGBOOK TWG
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Twg()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Twg");
            PrDdl();

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

					string htmlContent = $@"
                    <div role='tabpanel' class='bs-stepper-pane fade active dstepper-block'>
                        <div class='form-group'>
                            <label for='inputMailForm'>{model?.TwgDate.ToString("MMM d, yyyy")}</label>
                            <br />
                            <br />
                            <b>Remarks</b>
                            <div class='invalid-feedback'>{model?.Remarks}</div>
                        </div>
                    </div>";

					_hubContext.Clients.All.SendAsync("PrUpdateTwg", model.Prno, htmlContent);

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
                    PrNo = item.Prno,
                    PrDate = formattedPrDate,
                    ReceivedBy = item.ReceivedBy,
                    Remarks = item.Remarks
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

        #region LOGBOOK TWG EDIT
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult TwgEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("TwgEdit");
        }
        #endregion

        #region SUPPLIER
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Supplier()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Supplier");
            var supplier = _context.Supplier.ToList();

            return View(supplier);
        }
        [HttpPost]
        public IActionResult SaveSupplier(Supplier model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.SupplierName))
                {
                    _context.Supplier.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        public IActionResult GetSupplier(int id)
        {
            var supplier = _context.Supplier.Find(id);
            return Ok(supplier);
        }
        [HttpPost]
        public IActionResult UpdateSupplier([FromBody] Supplier newData)
        {
            if (ModelState.IsValid)
            {
                if (newData.SupplierName != null || newData.SupplierName == "")
                {
                    _context.Supplier.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region INDEXING
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Indexing()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Index", "Indexing");
            var index = _context.PuIndexing.ToList();

            return View(index);
        }
        [HttpPost]
        public IActionResult SaveIndex(PuIndexing model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.PoNo))
                {
                    _context.PuIndexing.Add(model);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        public IActionResult GetIndex(int id)
        {
            var item = _context.PuIndexing.Find(id);
            if (item != null)
            {
                string formattedBudgetDate = item.BudgetReleased.ToString("MMM d, yyyy");
                string formattedSupplyDate = item.SupplyReleased.ToString("MMM d, yyyy");

                var result = new
                {
                    PoNo = item.PoNo,
                    PrNo = item.PrNo,
                    ItemDesc = item.ItemDesc,
                    Gp = item.Gp,
                    Rmop = item.Rmop,
                    BudgetReleased = formattedBudgetDate,
                    SupplyReleased = formattedSupplyDate,
                    Remarks = item.Remarks
                };

                return Json(result);
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult UpdateIndex([FromBody] PuIndexing newData)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(newData.PoNo))
                {
                    _context.Update(newData);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        #endregion

        #region STEPPER
        public IActionResult Stepper()
        {
            ViewBag.Step1Status = "completed";
            ViewBag.Step2Status = "completed";
            return View();
        }
        #endregion

        #region PING IP ADDRESS
        [HttpPost]
        public async Task<IActionResult> PingIpAddress(string ipAddress)
        {
            var result = await PingIpAsync(ipAddress);

            return Json(result);
        }
        #endregion

        #region PING IP ASYNC
        private async Task<string> PingIpAsync(string ipAddress)
        {
            using (var ping = new Ping())
            {
                try
                {
                    var reply = await ping.SendPingAsync(ipAddress);
                    return $"Ping successful. Roundtrip time: {reply.RoundtripTime} ms";
                }
                catch (PingException ex)
                {
                    return $"Ping failed. Exception: {ex.Message}";
                }
            }
        }
        #endregion

        #region DROPDOWN LIST
        private void CanvassWithDate()
        {
            ViewBag.CanvassWithDate = new SelectList((from c in _context.Canvass.Where(x => x.RpqNo != null).ToList()
                                                      select new
                                                      {
                                                          CanvassDate = c.RpqNo + " / " + c.RpqDate.ToString("M-d-yyyy")
                                                      }),
                                     "CanvassDate",
                                     "CanvassDate",
                                     null); ;

        }

        private void PrWithDate()
        {
            ViewBag.PrWithDate = new SelectList((from pr in _context.Pr.Where(x => x.Prno != null).ToList()
                                                 select new
                                                 {
                                                     PrWithDate = pr.Prno + " / " + pr.PrnoDate.ToString("M-d-yyyy")
                                                 }),
                                     "PrWithDate",
                                     "PrWithDate",
                                     null);

        }

        private void PrDdl()
        {
            ViewBag.Pr = new SelectList((from pr in _context.Pr.Where(x => x.Prno != null).ToList()
                                           select new
                                           {
                                               Id = pr.Id,
                                               Prno = pr.Prno
                                           }),
                                     "Prno",
                                     "Prno",
                                     null);

        }

        private void PrDropDownList()
        {
            var query = from pr in _context.Pr
                        where pr.Prno != null
                        select new
                        {
                            Id = pr.Id,
                            Prno = pr.Prno
                        };

            var items = query.ToList();
            if (items.Any())
            {
                items.Insert(0, new { Id = 0, Prno = "Select an option" });

                ViewBag.PrId = new SelectList(items, "Id", "Prno");
            }
            else
            {
                ViewBag.PrId = new SelectList(new List<object>(), "Id", "Prno");
            }
        }

        private void BacResNoDownList()
        {
            ViewBag.BacResNo = new SelectList((from bcn in _context.BacResNo.ToList()
                                               select new
                                               {
                                                   Id = bcn.Id,
                                                   BacResNo = bcn.BacResNumber
                                               }),
                                     "BacResNo",
                                     "BacResNo",
                                     null);

        }

        private void CanvassDropDown()
        {
            ViewBag.Canvass = new SelectList((from c in _context.Canvass.ToList()
                                               select new
                                               {
                                                   Id = c.Id,
                                                   RpqNo = c.RpqNo
                                               }),
                                     "RpqNo",
                                     "RpqNo",
                                     null);

        }

        private void AbstractDropDown()
        {
            ViewBag.Abstract = new SelectList((from a in _context.Abstract.ToList()
                                              select new
                                              {
                                                  Id = a.Id,
                                                  AbstractNo = a.AbstractNo
                                              }),
                                     "AbstractNo",
                                     "AbstractNo",
                                     null);

        }
        #endregion

        #region PRINT NOA
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Print()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            SupplierDdl();

            return View();
        }
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Gas()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            PrDropDownList();

            return View();
        }
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Medicine()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            PrDropDownList();

            return View();
        }
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult TwgGoods()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            PrDropDownList();

            return View();
        }
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult TwgMedicine()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            PrDropDownList();

            return View();
        }
        #endregion

        #region SUPPLIER DROPDOWN LIST
        private void SupplierDdl()
        {
            ViewBag.Supplier = new SelectList((from supplier in _context.Supplier.ToList()
                                               select new
                                               {
                                                   Id = supplier.Id,
                                                   SupName = supplier.SupplierName
                                               }),
                                     "Id",
                                     "SupName",
                                     null);

        }
        #endregion

        #region PRINT SUPPLIER
        public IActionResult PrintPu(int supplierValue)
        {
            var supName = _context.Supplier.FirstOrDefault(x => x.Id == supplierValue).SupplierName;
            var supAddress = _context.Supplier.FirstOrDefault(x => x.Id == supplierValue).SupplierAddress;
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
                    AddCenteredParagraph(cell, supName);
                    //AddCenteredParagraph(cell, supAddress);
                    AddCenteredParagraph(cell, "Located at " + supAddress + ",");
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
                //return File(stream.ToArray(), "application/pdf");

                byte[] pdfBytes = stream.ToArray();
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                return Json(new { pdf = base64Pdf });
            }
        }
        #endregion

        #region PRINT RECOMMENDATION GOODS
        public IActionResult PrintRecommendationGoods(int gasValue, DateTime dateValue)
        {
            var prNo = _context.Pr.FirstOrDefault(x => x.Id == gasValue).Prno;
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
              
                    AddCenteredParagraph(cell, "PR NO:" + prNo + "                      " + "DATE:" + dateValue.ToString("MMM d, yyyy"), alignLeft: true, leftMargin: 7, isBold: true);
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
                byte[] pdfBytes = stream.ToArray();
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                return Json(new { pdf = base64Pdf });
            }
        }
        #endregion

        #region PRINT RECOMMENDATION MEDS
        public IActionResult PrintRecommendationMeds(int gasValue, DateTime dateValue)
        {
            var prNo = _context.Pr.FirstOrDefault(x => x.Id == gasValue).Prno;
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
                    AddCenteredParagraph(cell, "PR NO: " + prNo + "                      " + "DATE: " + dateValue.ToString("MMM d, yyyy"), alignLeft: true, leftMargin: 10f, isBold:true) ;
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
                byte[] pdfBytes = stream.ToArray();
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                return Json(new { pdf = base64Pdf });
            }
        }
        #endregion

        #region PRINT TWG GOODS
        public IActionResult PrintTWGGoods(int gasValue, DateTime dateValue)
        {
            var prNo = _context.Pr.FirstOrDefault(x => x.Id == gasValue).Prno;
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
                    AddCenteredParagraph(cell, "PR NO: " + prNo + "                      " + "DATE: " + dateValue.ToString("MMM d, yyyy"), alignLeft: true, leftMargin: 7, isBold: true);
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
                byte[] pdfBytes = stream.ToArray();
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                return Json(new { pdf = base64Pdf });
                //return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT TWG GOODS
        public IActionResult PrintTWGMeds(int gasValue, DateTime dateValue)
        {
            var prNo = _context.Pr.FirstOrDefault(x => x.Id == gasValue).Prno;
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
                    AddCenteredParagraph(cell, "PR NO: " + prNo + "                      " + "DATE: " + dateValue.ToString("MMM d, yyyy"), alignLeft: true, leftMargin: 7, isBold: true);
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
                byte[] pdfBytes = stream.ToArray();
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                return Json(new { pdf = base64Pdf });
                //return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT AGENCY TO AGENCY
        public IActionResult PrintAgencyToAgency(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {


                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var ata = _context.RmopAta.FirstOrDefault(x=>x.Id == id);

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "doh_logo_updated.png");
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(70f, 70f);

                int currentYear = DateTime.Now.Year;

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + ata.BacNo + "-AMP s. " + currentYear , true, 11);
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
                AddCellWithContent(nT, "                                 WHEREAS,  corollary  thereto,  Purchase  Request  No." + ata.PrNoOne + " for  the  Procurement  of ", false, 11);
                AddCellWithContent(nT, "              " + ata.PrDescriptionOne + "  in the ", false, 11);
                AddCellWithContent(nT, "              amount of PHP" + ata.PrAmountOne.ToString("##,#00.00") + " was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, after evaluation of the purchase request vis-à-vis the  implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing the  same,  the  BAC  has  come  to  a  resolution  that", false, 11);
                AddCellWithContent(nT, "              resort to the Alternative Mode of Procurement: Negotiated Procurement (Agency to Agency)  under  Sec. 53.5", false, 11);
                AddCellWithContent(nT, "              of the IRR would be most advantageous to the  government  and  is  essentially  the  most  applicable  recourse", false, 11);
                AddCellWithContent(nT, "              given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 WHEREAS, the BAC shall contract the foregoing procurement with the " + ata.AgencyBefore + "(Agency)" + ata.AgencyAfter + ";", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 NOW THEREFORE,  above   premises  considered,   resolved   as   it   is   hereby    resolved,   to", false, 11);
                AddCellWithContent(nT, "              recommend the use of the Alternative  Mode  of  Procurement:  Negotiated  Procurement  (Agency to Agency)", false, 11);
                AddCellWithContent(nT, "              under Sec. 53.5  of  the  Revised  Implementing  Rules  and  Regulations  of  the  Republic  Act No.  9184  for", false, 11);
                AddCellWithContent(nT, "              Purchase Request No. " + ata.PrNoOne + "in the amount of PHP" + ata.PrAmountOne.ToString("##,#00.00") + " for the Procurement of", false, 11);
                AddCellWithContent(nT, "              " + ata.PrDescriptionOne , false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 " + ata.PrDate + currentYear + ", Cebu City, Philippines.", false, 11);
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

        #region SIGNATORIES
        public IActionResult BacSignature(int id, int poid)
        {
            var rmopAta = _context.RmopAta.FirstOrDefault(x => x.Id == id);

            var abstractNo = _context.Abstract.FirstOrDefault(x=>x.Id == id);

            var poNo = _context.Po.FirstOrDefault(x => x.Id == poid);


            if(rmopAta != null)
            {
                rmopAta.IsForBac = true;

                _context.RmopAta.Update(rmopAta);
                _context.SaveChanges();

                return Json(new { success = true, message = "RMOP forwarded to the BAC for signature." });
            }
            else if (poNo != null)
            {
                poNo.IsForBudget = true;

                _context.Po.Update(poNo);
                _context.SaveChanges();

                return Json(new { success = true, message = "PO forwarded to the Budget." });
            }
            else
            {
                return Json(new { success = false, message = "ERROR saving to database." });
            }
        }

        public IActionResult RdSignature(int id)
        {
            var rmopAta = _context.RmopAta.FirstOrDefault(x => x.Id == id);

            if (rmopAta == null)
            {
                return NotFound();
            }

            rmopAta.IsForRd = true;

            _context.RmopAta.Update(rmopAta);
            _context.SaveChanges();

            return Json(new { success = true, message = "RMOP forwarded to the RD for signature." });
        }
        #endregion

        #region PRINT DIRECT CONTRACTING
        public IActionResult PrintDirectContracting(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var dc = _context.RmopDc.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + dc.BacNo + "-AMP s. " + currentYear, true, 11);
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
                AddCellWithContent(nT, "                                   WHEREAS,  corollary  thereto,  Purchase  Request  No. " + dc.PrNoOne +   " for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "              " + dc.PrDescriptionOne + "  in the amount of ", false, 11);
                AddCellWithContent(nT, "              PHP " + dc.PrAmountOne.ToString("##,#00.00") + "  was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   WHEREAS, after evaluation of the purchase request  vis-à-vis  the  implementing  rules  and  upon", false, 11);
                AddCellWithContent(nT, "              confirmation of the existence of the conditions allowing the same, the BAC has come to a  resolution  that  resort", false, 11);
                AddCellWithContent(nT, "              to  the  Alternative  Mode  of  Procurement   Direct  Contracting  under  Sec.  50  of  the  IRR   would   be   most", false, 11);
                AddCellWithContent(nT, "              advantageous to the government and is essentially the most applicable recourse given the availing circumstances;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                   NOW THEREFORE, above premises considered, resolved as it is hereby resolved, to  recommend", false, 11);
                AddCellWithContent(nT, "             the use of the Alternative Mode of Procurement Direct Contracting under Sec. 50  of  the  Revised  Implementing", false, 11);
                AddCellWithContent(nT, "             Rules and Regulations of the Republic Act No. 9184 for Purchase Request No. " + dc.PrNoOne + ", in  the  amount", false, 11);
                AddCellWithContent(nT, "             of PHP " + dc.PrAmountOne.ToString("##,#00.00") + "  for the Procurement of " + dc.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 " + dc.PrDate + ", Cebu City, Philippines.", false, 11);
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

        #region PRINT EMERGENCY CASES
        public IActionResult PrintEmergencyCases(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopEc = _context.RmopEc.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + rmopEc.BacNo + "-AMP s. " + currentYear, true, 11);
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
                AddCellWithContent(nT, "                                  WHEREAS, corollary thereto, Purchase Request No. " + rmopEc.PrNoOne + "  for the Procurement of", false, 11);
                AddCellWithContent(nT, "              " + rmopEc.PrDescriptionOne + "  in the amount of  PHP", false, 11);
                AddCellWithContent(nT, "              " + rmopEc.PrAmountOne.ToString("##,#00.00") + " was referred to the Bids and Awards Committee for processing;", false, 11);
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
                AddCellWithContent(nT, "             " + rmopEc.PrNoOne + " ,  in  the  amount  of  PHP   " + rmopEc.PrAmountOne.ToString("##,#00.00") + "  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "             " + rmopEc.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 " + rmopEc.PrDate + ", Cebu City, Philippines.", false, 11);
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

        #region PRINT LEASE OF VENUE
        public IActionResult PrintLeaseOfVenue(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopLov = _context.RmopLov.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + rmopLov.BacNo + "-AMP s. " + currentYear, true, 11);
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
                AddCellWithContent(nT, "                                 WHEREAS, corollary thereto, Purchase Request No. " + rmopLov.PrNoOne + " for the Procurement of", false, 11);
                AddCellWithContent(nT, "              " + rmopLov.PrDescriptionOne + "  in the amount of PHP" + rmopLov.PrAmountOne.ToString("##,#00.00"), false, 11);
                AddCellWithContent(nT, "               was referred to the Bids and Awards Committee for processing;", false, 11);
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
                AddCellWithContent(nT, "              Republic  Act  No.  9184  for  Purchase  Request  No. " + rmopLov.PrAmountOne + " , in  the  amount of PHP", false, 11);
                AddCellWithContent(nT, "              " + rmopLov.PrAmountOne.ToString("##,#00.00") + " for the Procurement of " + rmopLov.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 "+ rmopLov.PrDate + ", Cebu City, Philippines.", false, 11);
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

        #region PRINT PUBLIC BIDDING
        public IActionResult PrintPublicBidding(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopPb = _context.RmopPb.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + rmopPb.BacNo + "-AMP s. " + currentYear, true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                         “RECOMMENDING THE USE OF COMPETITIVE BIDDING (PUBLIC", true, 11);
                AddCellWithContent(nT, "                                       BIDDING) UNDER SEC. 10 OF THE REVISED IMPLEMENTING RULES", true, 11);
                AddCellWithContent(nT, "                                                                     AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  pursuant to Sec. 10 of the IRR, all procurement shall be done through", false, 11);
                AddCellWithContent(nT, "                        competitive bidding, except as provided in Rule XVI;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  corollary  thereto,  Purchase  Request  No. " + rmopPb.PrNoOne, false, 11);
                AddCellWithContent(nT, "                        for  the  Procurement  of " + rmopPb.PrDescriptionOne + "   in  the amount  of  PHP", false, 11);
                AddCellWithContent(nT, "                        " + rmopPb.PrAmountOne + " was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS, after evaluation of  the  purchase  request  vis-à-vis  the  implementing ", false, 11);
                AddCellWithContent(nT, "                        rules and finding no substantial ground to depart from the general rule, the BAC has  come  to  a ", false, 11);
                AddCellWithContent(nT, "                        resolution that Competitive Bidding, as the Mode of Procurement, be adopted  for  the  purchase ", false, 11);
                AddCellWithContent(nT, "                        request referred to in the immediately preceding clause;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           NOW THEREFORE, above premises considered, resolved as it is hereby resolved,", false, 11);
                AddCellWithContent(nT, "                        to recommend the use of Competitive Bidding  (Public Bidding)  under  Sec. 10 of  the  Revised", false, 11);
                AddCellWithContent(nT, "                        Implementing Rules and Regulations of the Republic Act  No. 9184  for  Purchase  Request  No.", false, 11);
                AddCellWithContent(nT, "                        " + rmopPb.PrNoOne + ", in  the  amount  of  PHP " + rmopPb.PrAmountOne + "  for  the  Procurment  of  ", false, 11);
                AddCellWithContent(nT, "                        " + rmopPb.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                             " + rmopPb.PrDate + ", Cebu City, Philippines.", false, 11);
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

        #region PUBLIC BIDDING INFRA
        public IActionResult PrintPublicBiddingInfra(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopPb = _context.RmopPb.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + "" + "-AMP s. " +currentYear, true, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                         “RECOMMENDING THE USE OF COMPETITIVE BIDDING (PUBLIC", true, 11);
                AddCellWithContent(nT, "                                       BIDDING) UNDER SEC. 10 OF THE REVISED IMPLEMENTING RULES", true, 11);
                AddCellWithContent(nT, "                                                                     AND REGULATIONS OF RA 9184.”", true, 11);
                AddCellWithContent(nT, "               __________________________________________________________________________________________", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  pursuant to Sec. 10 of the IRR, all procurement shall be done through", false, 11);
                AddCellWithContent(nT, "                        competitive bidding, except as provided in Rule XVI;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS,  corollary  thereto,  Purchase  Request  No. " + "", false, 11);
                AddCellWithContent(nT, "                        for  the  Procurement  of " + "" + "   in  the amount  of  PHP", false, 11);
                AddCellWithContent(nT, "                        " + "" + " was referred to the Bids and Awards Committee for processing;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           WHEREAS, after evaluation of  the  purchase  request  vis-à-vis  the  implementing ", false, 11);
                AddCellWithContent(nT, "                        rules and finding no substantial ground to depart from the general rule, the BAC has  come  to  a ", false, 11);
                AddCellWithContent(nT, "                        resolution that Competitive Bidding, as the Mode of Procurement, be adopted  for  the  purchase ", false, 11);
                AddCellWithContent(nT, "                        request referred to in the immediately preceding clause;", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                           NOW THEREFORE, above premises considered, resolved as it is hereby resolved,", false, 11);
                AddCellWithContent(nT, "                        to recommend the use of Competitive Bidding  (Public Bidding)  under  Sec. 10 of  the  Revised", false, 11);
                AddCellWithContent(nT, "                        Implementing Rules and Regulations of the Republic Act  No. 9184  for  Purchase  Request  No.", false, 11);
                AddCellWithContent(nT, "                        " + "" + ", in  the  amount  of  PHP " + "" + "  for  the  Procurment  of  ", false, 11);
                AddCellWithContent(nT, "                        " + "", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                             " + "" + " 2023 , Cebu City, Philippines.", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             ENGR. JOEY SALAGANTIN" + "           " + "ATTY. MARISSA C. GOROSIN" + "                 " + "MR. RAMIL R. ABREA", true, 11);
                AddCellWithContent(nT, "                     Provisional Member" + "                                   " + "Provisional Member" + "                                          " + "Member", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "             MR. ROLDAN A. CUBILLO" + "              " + "ATTY. JO DAVID Z. BORCES" + "                 " + "DR. SOPHIA M. MANCAO", true, 11);
                AddCellWithContent(nT, "                            Member" + "                                                " + "Provisional Member" + "                                   " + "Vice-Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                           DR. JONATHAN NEIL V. ERASMO", true, 11);
                AddCellWithContent(nT, "                                                                                               Chairperson", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
   
                doc.Add(nT);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT PS DBM
        public IActionResult PrintPsDbm(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopPsdbm = _context.RmopPsDbm.FirstOrDefault(x => x.Id == id);

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

                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + rmopPsdbm.BacNo + "-AMP s. " + currentYear, true, 11);
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

        #region PRINT SCIENTIFIC SCHOLARLY
        public IActionResult PrintScientificScholarly(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                int currentYear = DateTime.Now.Year;

                var rmopSs = _context.RmopSs.FirstOrDefault(x => x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + rmopSs.BacNo + "-AMP s. " + currentYear, true, 11);
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
                AddCellWithContent(nT, "                                 WHEREAS,  corollary  thereto,  Purchase  Request  No. " + rmopSs.PrNoOne + "  for  the  Procurement  of", false, 11);
                AddCellWithContent(nT, "              " + rmopSs.PrDescriptionOne + " in  the ", false, 11);
                AddCellWithContent(nT, "              amount of PHP " + rmopSs.PrAmountOne.ToString("##,#0.00") + " was referred to the Bids and Awards Committee for processing;", false, 11);
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
                AddCellWithContent(nT, "              Rules and Regulations of the Republic Act No. 9184 for Purchase Request No. " + rmopSs.PrNoOne + ",  in  the", false, 11);
                AddCellWithContent(nT, "              amount of PHP" + rmopSs.PrAmountOne.ToString("##,#0.00") + " for the Procurment of " + rmopSs.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "              ", false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                 " + rmopSs.PrDate + " 2023 , Cebu City, Philippines.", false, 11);
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

        #region PRINT SMALL VALUE
        public IActionResult PrintSmallValue(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.LEGAL);
                doc.SetMargins(10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var smallValue = _context.RmopSvp.FirstOrDefault(x=>x.Id == id);

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
                AddCellWithContent(nT, "                                                         BAC RESOLUTION NO. " + smallValue.BacNo + "-AMP s. 2023", true, 11);
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
                AddCellWithContent(nT, "                                  WHEREAS,  corollary  thereto,  Purchase  Request  No. " + smallValue.PrNoOne + "  for  the Procurement ", false, 11);
                AddCellWithContent(nT, "              of " + smallValue.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "              in the amount of PHP " + smallValue.PrAmountOne.ToString("##,#00.00") + " was referred to the Bids and Awards Committee for processing;", false, 11);
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
                AddCellWithContent(nT, "              R9184  for  Purchase  Request  No. " + smallValue.PrNoOne + ", in  the  amount  of  PHP " + smallValue.PrAmountOne.ToString("##,#00.00"), false, 11);
                AddCellWithContent(nT, "              for  the Procurement of " + smallValue.PrDescriptionOne, false, 11);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "", false, 10, 10f);
                AddCellWithContent(nT, "                                                                 " + smallValue.PrDate + " 2023 , Cebu City, Philippines.", false, 11);
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

        #region PRINT CHECKLIST 1
        public IActionResult PrintChecklist1()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 33; i++)
                    {
                        if (i == 19 || i == 22  || i == 30 || i == 29) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 173f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 173f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 173f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 173f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 173f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }




                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();
                    
                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #1", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 129; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #1", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR VENUE,", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("MEALS, AND ACCOMMODATION", isBold: true, isUnderlined: true, fontSize: 11f);

                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);

                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize:11f);

                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR #, date, unit cost and correct total cost");
                JParagraph("                                             Title of Activity");
                JParagraph("                                             Meals and meal type");
                JParagraph("                                             Number of EXPECTED pax");
                JParagraph("                                             Number of GUARANTEED pax");
                JParagraph("                                             Date/Period of Activity");
                JParagraph("                                             Specify - Meals and Accommodation");
                JParagraph("                                             Check-in and Check-out time");
                JParagraph("                                             Specify - Meals only");
                JParagraph("                                             Preferred Menu, if desired");
                JParagraph("                                             Specify - Venue and Meals only");
                JParagraph("                                             Specify - Accommodation only");
                JParagraph("                                             Room Sharing");
                JParagraph("                                             Specify - Venue only");
                JParagraph("                                             Location of venue/hotel/office");
                JParagraph("                                             Classification of hotel/venue");
                JParagraph("                                             Function room preference");
                JParagraph("                                             Funding source");
                JParagraph("\n");
                JParagraph("                                             Specify others amenities, if applicable");
                JParagraph("                                             Others");
                JParagraph("\n");
             
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Specs/details in CN match specs in PR");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold:true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");


                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic:true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic:true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 2
        public IActionResult PrintChecklist2()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false, bool isUnderlined = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i == 5 || i == 13 || i == 20 || i == 21 ) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 280f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 280f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 280f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 280f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 280f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #2", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 236; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #2", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR REPRODUCTION OF,", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("IEC/ TRAINING MATERIALS, REPORTING", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("AND OTHER PRINTED FORMS", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("(such as Flyer, Brochure, Forms, Poster, ", isBold: true);
                CParagraph("Tarpaulin, Streamer, Sticker, Leaflet,", isBold: true);
                CParagraph("Flipchart, Manual, Newsletter, etc)", isBold: true);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                       With Complete description", isBold:true);
                JParagraph("                                               Paper Size");
                JParagraph("                                               Type of paper");
                JParagraph("                                               Number of pages");
                JParagraph("                                               Printing type (back-to-back or one-side)");
                JParagraph("                                               Type of binding");
                JParagraph("                                               Colored or black & white printing");
                JParagraph("                                               Others");
                JParagraph("                                              ");
                JParagraph("                                             Approved Allocation List or List of Recipients");
                JParagraph("                                             Date Needed, if applicable");
                JParagraph("                                             Approved Realignment, if applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 3
        public IActionResult PrintChecklist3()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 28; i++)
                    {
                        if (i == 11 || i == 24  || i == 25) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 248f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 248f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 248f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 248f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 248f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #3", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 204; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #3", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER PROVISION", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("OF IEC COLLATERALS", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("(Shirt, Cap Vest, Training Kit, Tarpaulin ", isBold: true);
                CParagraph("Bags, Jacket, Pants, Mug, Umbrella, etc.)", isBold: true);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Quantity");
                JParagraph("                                             Color");
                JParagraph("                                             Size(s)");
                JParagraph("                                             Text/Design");
                JParagraph("                                             Printed Sample Design");
                JParagraph("                                             With or without print/embroidery");
                JParagraph("                                       With Complete description:", isBold:true);
                JParagraph("                                                Polo shirt or T-shirt (for shirt only)");
                JParagraph("                                                Type of cloth/material");
                JParagraph("                                                Number of folds (umbrella only)");
                JParagraph("                                                With eyelet or none (for tarpaulin only)");
                JParagraph("                                                Ceramic or glass (for mug only)");
                JParagraph("                                             Approved Allocation  List or List of Recipients");
                JParagraph("                                             Date Needed, If applicable");
                JParagraph("                                             PR description match printed design");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 4
        public IActionResult PrintChecklist4()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 27; i++)
                    {
                        if ( i == 23 || i == 24) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 260f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 260f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 260f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 260f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 260f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }


                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #4", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 217; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #4", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR LABOR AND/OR", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("MATERIALS FOR REPAIR, INSTALLATION,", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("REFURBISH, REPLACEMENT, FABRICATION", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("RESTORATION, RENOVATION", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("PREVENTIVE MAINTENANCE, TERMITE", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("TREATMENT, OVERHAUL, etc.", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("(Plumbing, Carpentry, Painting, Vehicle, ", isBold: true);
                CParagraph("Electrical, IT, Termite Treatment, etc.)", isBold: true);
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR #, date, unit cost and correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Pre-Job Order Inspection Report");
                JParagraph("                                             IT Job Report (for IT request only)");
                JParagraph("                                             Scope of Work, if applicable");
                JParagraph("                                             Detailed Estimates, if applicable");
                JParagraph("                                             Design, dimensions, etc. if applicable");
                JParagraph("                                             Bill of Materials, if applicable");
                JParagraph("                                             Complete description of materials");
                JParagraph("                                             NO BRAND NAME");
                JParagraph("                                             Specify - Labor and materials, etc.");
                JParagraph("                                             Specify - Labor only or materials only");
                JParagraph("                                             Specify - Spare parts, if applicable");
                JParagraph("                                             Specify Vehicle Plante No. (for vehicle only)");
                JParagraph("                                             Specify Vehicle Model Only and Year");
                JParagraph("                                             Date Needed, if applicable");
                JParagraph("                                             PR description match printed design");
                JParagraph("                                             Warranty Period, if applicable");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 5
        public IActionResult PrintChecklist5()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 18; i++)
                    {
                        if (i == 14 || i == 15) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 248f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 248f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 248f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 248f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 248f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #5", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 204; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #5", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("VEHICLE RENTAL", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("(Passenger Bus/Van, Car, Shuttle Bus,", isBold: true);
                CParagraph("Cargo Van, Utility Vehicle)", isBold: true);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Date and Time of Activity or Travel");
                JParagraph("                                             Itinerary of Travel");
                JParagraph("                                             Number of Passengers");
                JParagraph("                                             Number of Vehicles");
                JParagraph("                                             Preferred Type of Vehicle");
                JParagraph("                                             Passenger Capacity of Vehicle");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 6
        public IActionResult PrintChecklist6()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i == 20 || i == 21) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 220f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #6", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 176; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #6", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR PUBLICATION", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("IN THE NEWSPAPER AND", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("RADIO PLUGGING", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Date/Period of Publication (for newspaper only)");
                JParagraph("                                             Announcement or Advertisement for Publication");
                JParagraph("                                             Number of Days to be Published");
                JParagraph("                                             Size of Newspaper Space");
                JParagraph("                                             Number of Newspaper to be Published");
                JParagraph("                                             Type of Newspaper (Local or National)");
                JParagraph("                                             Date/Period of Radio Plug (for radio only)");
                JParagraph("                                             Number of Days to be Plugged");
                JParagraph("                                             Length of Plug");
                JParagraph("                                             Frequency of Plug");
                JParagraph("                                             Number of Radio Stations");
                JParagraph("                                             Radio Coverage Areas");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 7
        public IActionResult PrintChecklist7()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 14; i++)
                    {
                        if (i == 10 || i == 11) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 203f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #7", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 160; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #7", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR DOCUMENT", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("NOTARIZATION", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Name of Document");
                JParagraph("                                             Number of Documents");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 8
        public IActionResult PrintChecklist8()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 30; i++)
                    {
                        if (i== 7 || i == 10 || i == 11 || i == 15 || i == 19 || i == 26 || i == 27) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 186f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 186f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 186f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 186f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 186f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #8", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 143; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #8", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("DRUGS AND MEDICINES", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             With stamp and signature of the Therapeutic Committee");
                JParagraph("                                             Complete description of item including unit, form, strength, size");
                JParagraph("                                             (for bottle or container) & quantity per container, bottle or strip");
                JParagraph("                                             NO BRAND NAME");
                JParagraph("                                             Must be fresh commercial stock with a total shelf life of 24");
                JParagraph("                                             months from the date of manufacture facture but not less");
                JParagraph("                                             than 18 months from the date of delivery");
                JParagraph("                                             For supplier to submit valid License to Operate");
                JParagraph("                                             (LTO) as Wholesaler, Manufacturer or Distributor");
                JParagraph("                                             For supplier to submit valid Certificate of Good");
                JParagraph("                                             Manufacturing Practice (CGMP)");
                JParagraph("                                             For supplier to submit EDPMS Certificate of Compliance");
                JParagraph("                                             For supplier to submit the result of Drug Analysis from FDA");
                JParagraph("                                             20% Retention shall be deducted pending FDA");
                JParagraph("                                             PASSED Result of Analysis");
                JParagraph("                                             Labelling instruction");
                JParagraph("                                             Preferred delivery period, if applicable");
                JParagraph("                                             Approved Allocation or Distribution List");
                JParagraph("                                             Approved Realignment, if applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 9
        public IActionResult PrintChecklist9()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 26; i++)
                    {
                        if (i == 9 || i == 11 || i == 13 || i == 22 || i == 23 ) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 220f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #9", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 176; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #9", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR MEDICAL/", isBold: true, fontSize: 11f); ;
                CParagraph("DENTAL/LABORATORY SUPPLIES AND", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("COMMODITIES, REAGENTS, ACTIVE", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("INGREDIENTS", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Complete description of item inlcuding unit, size, measurement");
                JParagraph("                                             make, strength, component, active ingredient, etc.");
                JParagraph("                                             NO BRAND NAME");
                JParagraph("                                             Expiry date of not less than 2 years upon receipt of");
                JParagraph("                                             delivery, if applicable");
                JParagraph("                                             For supplier to submit valid Certificate of Operate (LTO)");
                JParagraph("                                             as Wholesaler, Manufacturer or Distributor");
                JParagraph("                                             For supplier to submit valid Certificate of Product Registration");
                JParagraph("                                             (CPR), if applicable");
                JParagraph("                                             With stamp and signature of Therapeutic");
                JParagraph("                                             Committee, if applicable");
                JParagraph("                                             Items are with Full WHOPES approval, if applicable");
                JParagraph("                                             Approved Allocation or Distribution List, if applicable");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 10
        public IActionResult PrintChecklist10()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 26; i++)
                    {
                        if (i == 6 || i == 8 || i == 10 || i == 12 || i == 14 || i == 22 || i == 23) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 206f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 206f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 206f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 206f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 206f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #10", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 163; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #10", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("PURCHASE OR LEASE OF", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("EQUIPMENT", isBold: true, isUnderlined: true, fontSize: 13f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Pre-Job Order Inspection Report (if equipment is a replacement");
                JParagraph("                                             of defective unit)");
                JParagraph("                                             IT Job Report (together with Pre-Job Order Inspection Report");
                JParagraph("                                             for IT requests only)");
                JParagraph("                                             With stamp and signature of CLEARING HOUSE");
                JParagraph("                                             COMMITTEE, if medical equipment");
                JParagraph("                                             With Phrase LEASE FOR 3 MONTHS if fund source");
                JParagraph("                                             is not CAPITAL OUTLAY");
                JParagraph("                                             With complete description but not limited to size, dimension,");
                JParagraph("                                             color, make, type, horsepower, capacity, etc.");
                JParagraph("                                             NO BRAND NAME");
                JParagraph("                                             Allocation or Distribution List or List of Recipients, if applicable");
                JParagraph("                                             Approved Justification for Emergency Purchase");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 11
        public IActionResult PrintChecklist11()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 22; i++)
                    {
                        if (i == 6 || i == 8 || i == 10 || i == 18 || i == 19) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 190f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 190f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 190f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 190f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 190f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #11", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 146; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #11", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("FURNITURES & FIXTURES", isBold: true, isUnderlined: true, fontSize: 13f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Pre-Job Order Inspection Report (if equipment is a replacement");
                JParagraph("                                             of defective unit)");
                JParagraph("                                             With Phrase LEASE FOR 3 MONTHS if fund source");
                JParagraph("                                             is not CAPITAL OUTLAY");
                JParagraph("                                             With complete description but not limited to size, dimension,");
                JParagraph("                                             color, make, type, horsepower, capacity, etc.");
                JParagraph("                                             Provision of warranty, if applicable");
                JParagraph("                                             Name of user or location office");
                JParagraph("                                             Approved Justification, if applicable");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 12
        public IActionResult PrintChecklist12()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 25; i++)
                    {
                        if (i == 11 || i == 15 || i == 21 || i == 22) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 260f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 260f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 260f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 260f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 260f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #12", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 216; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #12", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR", isBold: true, fontSize: 11f); ;
                CParagraph("COMMON USE OFFICER", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("SUPPLIES AND MATERIALS", isBold: true, isUnderlined: true, fontSize: 10f);
                CParagraph("\n");
                CParagraph("(to include IT spare parts/consumables, copier", isBold: true);
                CParagraph("& other equipment consumables, carpentry,", isBold: true);
                CParagraph("plumbing & painting supplies & materials, etc.)", isBold: true);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Funding source");
                JParagraph("                                             Quality per Item");
                JParagraph("                                             Estimate Unit Cost");
                JParagraph("                                             Total Cost Per item");
                JParagraph("                                             Grand Total");
                JParagraph("                                             Approved Allocation List or List of Recipients, if applicable");
                JParagraph("                                             Complete description of the items to include type,");
                JParagraph("                                             make, color, size, measurement, shape etc.");
                JParagraph("                                             NO BRAND NAME");
                JParagraph("                                             Pre-Job Order Inspection Report, if applicable");
                JParagraph("                                             IT Job Report (together with Pre-Job Order Inspection Report");
                JParagraph("                                             for IT requests only)");
                JParagraph("                                             Provision for warranty, if applicable");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }

        #endregion

        #region PRINT CHECKLIST 13
        public IActionResult PrintChecklist13()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 22; i++)
                    {
                        if (i == 18 || i == 19) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 220f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #13", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 176; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #13", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR FREIGHT AND", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("HANDLING SERVICES AND", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("MAILING SERVICES", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of request");
                JParagraph("                                             Date or Period of Job Service");
                JParagraph("                                             Date/Time of Pick up");
                JParagraph("                                             Date/Time of Expected Arrival");
                JParagraph("                                             Value of Item to be Transported");
                JParagraph("                                             Type of Items/Package/Cargo");
                JParagraph("                                             Destination of Items/Cargo");
                JParagraph("                                             Name of Recipient");
                JParagraph("                                             Address of Recipient");
                JParagraph("                                             Means of transport");
                JParagraph("                                             Approve Terms and Conditions");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 14
        public IActionResult PrintChecklist14()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 18; i++)
                    {
                        if (i == 14 || i == 15) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 220f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 220f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 220f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #14", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 176; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #14", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("JOB ORDER FOR HIRING OF", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("TECHNICAL ASSISTANCE OR", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("CONSULTING SERVICES", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Description of Request");
                JParagraph("                                             Title of Activity");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Approve Terms or Reference");
                JParagraph("                                             Expected Outputs/Deliverables");
                JParagraph("                                             Contract Duration");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 15
        public IActionResult PrintChecklist15()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 17; i++)
                    {
                        if (i == 13 || i == 14) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 203f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #15", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 160; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #15", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("LEASE OF PRIVATELY-OWNED", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("REAL ESTATE AND VENUE", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Period of Lease");
                JParagraph("                                             Location of the real estate or venue");
                JParagraph("                                             Size of the area");
                JParagraph("                                             Approve Terms and Conditions");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 16
        public IActionResult PrintChecklist16()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 17; i++)
                    {
                        if (i == 13 || i == 14) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 187f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 187f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 187f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 187f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 187f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #16", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 143; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #16", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("SECURITY SERVICES", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Period of Lease");
                JParagraph("                                             Location of the real estate or venue");
                JParagraph("                                             Size of the area");
                JParagraph("                                             Approve Terms and Conditions");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 17
        public IActionResult PrintChecklist17()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 21; i++)
                    {
                        if (i == 12 || i == 17 || i == 18) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 187f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 187f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 187f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 187f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 187f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #17", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 143; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #17", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("JANITORIAL SERVICES", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Period of Service");
                JParagraph("                                             Number of Cleaning Professionals");
                JParagraph("                                             Approve Terms and Conditions");
                JParagraph("                                             Blank Form for Cost Distribution");
                JParagraph("                                             List of Janitorias Chemicals and Supplies");
                JParagraph("                                             List of Janitorial Equipments and Other Cleaning Materials");
                JParagraph("                                             Copy of Current DOLE Suggested Contract Rates");
                JParagraph("                                             for Manpower Agencies");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 18
        public IActionResult PrintChecklist18()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 17; i++)
                    {
                        if (i == 13 || i == 14) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 203f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #18", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 160; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #18", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("I.T SOFTWARE SUBSCRIPTION/", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("LICENSING", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Unit of Issue and Quantity");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Period of Subscription");
                JParagraph("                                             Complete specifications/requirements");
                JParagraph("                                             Complete specifications/requirements");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 19
        public IActionResult PrintChecklist19()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 19; i++)
                    {
                        if (i == 7 || i == 15 || i == 16) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 219f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 219f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 219f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 219f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 219f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #19", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 176; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #19", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("LED SCREEN WITH OR", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("WITHOUT SOUND SYSTEM", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("LIGHTS AND PHOTOBOOTH", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Unit of Issue and Quantity");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Complete specifications/requirements");
                JParagraph("                                             or approved Terms and Reference");
                JParagraph("                                             Date and time needed");
                JParagraph("                                             Venue and location of venue");
                JParagraph("                                             No. of hours needed");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 20
        public IActionResult PrintChecklist20()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 32; i++)
                    {
                        if (i == 28 || i == 29) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 156f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 156f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 156f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 156f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 156f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #20", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 113; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #20", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("INFRA PROJECTS", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Unit of Issue and Quantity");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Complete specifications/requirements");
                JParagraph("                                             Approved Bid Date Sheet");
                JParagraph("                                             Approved Special Conditions of the Contract");
                JParagraph("                                             Approved Technical Specifications");
                JParagraph("                                             Approved Plans/Drawings incl. cover");
                JParagraph("                                             Program of Works");
                JParagraph("                                             Bill of Quantities (BOQ)");
                JParagraph("                                             Detailed Bill of Quantities Forms");
                JParagraph("                                             Detailed Unit Prices Analysis (DUPA) Forms");
                JParagraph("                                             Annex A - Construction Schedule and S-curve Forms");
                JParagraph("                                             Annex B - Manpower Schedule Forms");
                JParagraph("                                             Annex C - Construction Methods Forms");
                JParagraph("                                             Annex D - Equipment Schedules Forms");
                JParagraph("                                             Annex E - Construction Safety and Health Programs");
                JParagraph("                                             Annex F - PERT/CPM");
                JParagraph("                                             Annex G - Cash Flow/Payment Schedule Forms");
                JParagraph("                                             Statement/List of Key Personnel Forms");
                JParagraph("                                             Statement/List of Construction Personnel Forms");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion

        #region PRINT CHECKLIST 21
        public IActionResult PrintChecklist21()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                doc.SetMargins(30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                void CParagraph(string text, bool isBold = false, bool isUnderlined = false, float fontSize = 10f)
                {
                    Font font = isBold ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, fontSize) : new Font(Font.FontFamily.HELVETICA, fontSize, Font.NORMAL);
                    if (isUnderlined)
                    {
                        font.SetStyle(Font.UNDERLINE);
                    }
                    Paragraph centeredParagraph = new Paragraph(new Chunk(text, font));
                    centeredParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(centeredParagraph);
                }

                void JParagraph(string text, bool isBold = false, bool centerAligned = false, bool isItalic = false)
                {
                    Font font;
                    if (isBold)
                    {
                        font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);
                    }
                    else if (isItalic)
                    {
                        font = FontFactory.GetFont(FontFactory.TIMES_ITALIC, 10f);
                    }
                    else
                    {
                        font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
                    }

                    Paragraph justifiedParagraph = new Paragraph();

                    PdfContentByte checkbox = writer.DirectContent;
                    for (int i = 0; i < 17; i++)
                    {
                        if (i == 8 || i == 13 || i == 14) continue; // Exclude certain values
                        checkbox.SetLineWidth(1f);
                        checkbox.Rectangle(doc.Left - -112f, doc.Top - 203f - (i * 16f), 10f, 10f);
                        checkbox.Stroke();

                        // Draw an X mark inside the rectangle
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 1f); // Move to the starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 9f); // Draw a line to create one part of the X
                        checkbox.MoveTo(doc.Left - -112f + 1f, doc.Top - 203f - (i * 16f) + 9f); // Move to another starting point of the X mark inside the rectangle
                        checkbox.LineTo(doc.Left - -112f + 9f, doc.Top - 203f - (i * 16f) + 1f); // Draw another line to create the other part of the X
                        checkbox.Stroke(); // Stroke the lines to make them visible
                    }

                    // Add the text next to the checkbox
                    justifiedParagraph.Add(new Chunk(" " + text, font));

                    // Set alignment based on centerAligned parameter
                    justifiedParagraph.Alignment = centerAligned ? Element.ALIGN_CENTER : Element.ALIGN_JUSTIFIED;
                    doc.Add(justifiedParagraph);
                }

                void DrawRectangle(float width, float height)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Calculate the left position to center the rectangle
                    float leftPosition = (PageSize.A4.Width - width) / 2;

                    // Draw the rectangle using the calculated left position
                    contentByte.Rectangle(leftPosition, doc.Top - height, width, height);
                    contentByte.Stroke();

                }

                // Calculate the width of the text "P.U CHECKLIST #1"
                var textWidth = new Chunk("P.U CHECKLIST #21", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20f)).GetWidthPoint();
                float boxWidth = textWidth + 200; // Add some padding
                float boxHeight = 40f; // Adjust the height as needed
                DrawRectangle(boxWidth, boxHeight);

                void DrawRectangleAtPosition(float width, float height, float leftPosition, float topPosition)
                {
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.SetLineWidth(1f);

                    // Draw the rectangle at the specified position
                    contentByte.Rectangle(leftPosition, topPosition - height, width, height);
                    contentByte.Stroke();
                }

                // Calculate the width of the text "SUPPORTING DOCUMENTS/DETAILS"
                var supportingDocsTextWidth = new Chunk("SUPPORTING DOCUMENTS/DETAILS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f)).GetWidthPoint();
                float supportingDocsBoxWidth = supportingDocsTextWidth + 180; // Add some padding
                float supportingDocsBoxHeight = 15f; // Adjust the height as needed

                // Calculate the left position to center the rectangle
                float supportingDocsLeftPosition = (PageSize.A4.Width - supportingDocsBoxWidth) / 2;

                // Determine the top position for the rectangle based on the previous paragraph's position
                float supportingDocsTopPosition = doc.Top - 160; // Adjust this value as needed

                // Draw the rectangle for "SUPPORTING DOCUMENTS/DETAILS"
                DrawRectangleAtPosition(supportingDocsBoxWidth, supportingDocsBoxHeight, supportingDocsLeftPosition, supportingDocsTopPosition);

                CParagraph("P.U CHECKLIST #21", isBold: true, fontSize: 20f);
                CParagraph("\n");
                CParagraph("PURCHASE REQUEST (PR) FOR ", isBold: true, fontSize: 11f); ;
                CParagraph("VARIOUS HARDWARE", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("SUPPLIES/MATERIALS", isBold: true, isUnderlined: true, fontSize: 11f);
                CParagraph("\n");
                CParagraph("PR NO. :    42424", isBold: false);
                CParagraph("DATE : 42424242", isBold: false);
                CParagraph("\n");
                CParagraph("SUPPORTING DOCUMENTS/DETAILS", isBold: true, fontSize: 11f);
                CParagraph("\n");
                JParagraph("                                             Complete signatures incl. approval");
                JParagraph("                                             Complete with PR # and date");
                JParagraph("                                             Unit of Issue and Quantity");
                JParagraph("                                             Estimated unit cost & correct total cost");
                JParagraph("                                             Purpose of Request");
                JParagraph("                                             Funding Source");
                JParagraph("                                             Complete specifications/requirements");
                JParagraph("                                             With stamp and signature of Clearinge");
                JParagraph("                                             House Committee, if applicable");
                JParagraph("                                             Approved Realignment, If applicable");
                JParagraph("                                             Approved WFP/Supplemental WFP");
                JParagraph("                                             Approved PPMP/Supplemental PPMP");
                JParagraph("                                             Others");
                JParagraph("\n");
                JParagraph("                                       REMARKS: ");
                JParagraph("                                             COMPLETE", isBold: true);
                JParagraph("                                             Please comply with item(s) marked X on or before", isBold: true);
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("\n");
                JParagraph("                                             CHECKED/REVIEWED BY:", isItalic: true);
                CParagraph("\n");
                JParagraph("                                                                        STEFANIE LORRAINE D. TRINIDAD");
                JParagraph("                                                                                              Printed name and Signature", isItalic: true);
                CParagraph("\n");

                doc.Close();
                return File(stream.ToArray(), "application/pdf");
            }
        }
        #endregion


        #region PU USERS
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        [Route("Procurement/Users")]
        public async Task<IActionResult> PuUser(string selectedEmployee)
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Users", "");

            var user = UserRole;

            var users = _dtsContext.users
                .Where(u => string.IsNullOrEmpty(selectedEmployee) || u.Username.Contains(selectedEmployee) || u.Email.Contains(selectedEmployee))
            .OrderBy(x => x.Fname)
                .ToList();

            var puUser = await _context.PuUser.ToListAsync();

            var viewModel = new CombineIndexFmisUser
            {
                Users = users,
                PuUser = puUser
            };

            ViewBag.userId = _context.PuUser.Select(x => x.UserId).ToList();

            return View(viewModel);
        }
        #endregion

        #region SAVE AND DELETE PU USERS
        [HttpPost]
        public IActionResult SavePuUsers(int userId)
        {
            //dohdtr.users.id = userId

            var dtrUser = _dtsContext.users.FirstOrDefault(x=>x.Id == userId);

            if(dtrUser != null)
            {
                var puUser = new PuUser
                {
                    UserId = dtrUser.Id.ToString(),
                    Username = dtrUser.Username,
                    Password = dtrUser.Password,
                    Email = dtrUser.Email,
                    Fname = dtrUser.Fname,
                    Lname = dtrUser.Lname,
                };

                _context.PuUser.Add(puUser);
                _context.SaveChanges();

                return Json(new { success = true });
            }
           

            return Json(new { success = false });

        }


        public async Task<IActionResult> DeletePuUser(int id)
        {
            var puUser = _context.PuUser.FirstOrDefault(x=>x.Id == id);

            _context.Remove(puUser);
            await _context.SaveChangesAsync();

            await Task.Delay(2000);

            return RedirectToAction("PuUser");
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
				return RedirectToAction("Checklist1", "Procurement");
			}
            else
            {
                return View();
            }
        }
        #endregion

        #region LOGIN POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (user, errorMessage) = await _userService.ValidatePuUserCredentialsAsync(model.Username, model.Password);

                if (user != null)
                {
                    await LoginAsync(user, model.RememberMe);

                    return RedirectToAction("Checklist1", "Procurement");
                }
                else
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ModelState.AddModelError("Validation", errorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError("User", "User not found in the database or invalid password");
                    }

                    return View(model);
                }

                // Additional logic for handling successful login if needed
            }

            ModelState.AddModelError("Username", "Username or Password is Incorrect");
            return View(model);
        }
        #endregion

        #region LOGOUT GET
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return await Logout();
        }
        #endregion

        #region LOGOUT POST
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Scheme4");
            return RedirectToAction("Login", "Procurement");
        }
        #endregion

        #region NOT FOUND
        public new IActionResult NotFound()
        {
            return View();
        }
        #endregion

        #region HELPERS
        private async Task LoginAsync(PuUser user, bool rememberMe)
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
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname),
            };
            claims.Add(new Claim(ClaimTypes.Role, "pu_admin"));

            var identity1 = new ClaimsIdentity(claims, "Scheme4");
            var principal1 = new ClaimsPrincipal(identity1);

            await HttpContext.SignInAsync("Scheme4", principal1);
        }
		#endregion

		#region COOKIES
		public string UserId { get { return User.FindFirstValue(ClaimTypes.Name); } }
		public string UserRole { get { return User.FindFirstValue(ClaimTypes.Role); } }
		#endregion


	}
}
