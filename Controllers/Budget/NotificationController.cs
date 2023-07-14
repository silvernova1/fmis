using fmis.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace fmis.Controllers.Budget
{
    public class NotificationController : Controller
    {
        private readonly LogsContext _LogsContext;
        public NotificationController(LogsContext logsContext) { 
            _LogsContext = logsContext;
        }

        // GET: NotificationController
        public ActionResult notificationBody()
        {
            return View("~/Views/Budget/Notification/NotificationBody.cshtml");
        }

        public async Task<IActionResult> NotificationList(string logs_type)
        {
            var obligation_notification = await _LogsContext.Logs.Where(x => x.logs_type.Equals(logs_type)).AsNoTracking().ToListAsync();
            return View("~/Views/Budget/Notification/NotificationList.cshtml",obligation_notification);
        }

        // POST: NotificationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NotificationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NotificationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NotificationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NotificationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
