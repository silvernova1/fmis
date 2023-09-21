using fmis.Data.Accounting;
using fmis.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using fmis.Models.UserModels;
using System.Data;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;
using Microsoft.EntityFrameworkCore;

namespace fmis.Controllers.Accounting
{
    //[Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
    public class UsersController : Controller
    {
        
        private readonly fmisContext _fmisContext;
        private readonly MyDbContext _myDbContext;
        public UsersController(fmisContext FmisContext, MyDbContext myDbContext)
        {
            _fmisContext = FmisContext;
            _myDbContext = myDbContext;
          
        }

        
        //[Route("Accounting/Users")]
        public async Task<IActionResult> Index(string selectedEmployee)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var users = _fmisContext.users
                .Where(u => string.IsNullOrEmpty(selectedEmployee) || u.Username.Contains(selectedEmployee) || u.Email.Contains(selectedEmployee))
                .OrderBy(x => x.Fname)
                .ToList();

            var list_user = await _myDbContext.IndexUser.ToListAsync();

            var viewModel = new CombineIndexFmisUser
            {
                Users = users,
                ListUser = list_user
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveUsers(int selectedEmployee)
        {

              var userToSave = _fmisContext.users.FirstOrDefault(x => x.Id == selectedEmployee);
            
            /* var userToSave = _fmisContext.users
               .Where(u => u.Id == selectedEmployee)
               .Select(u => new
               {
                   u.Id,
                   u.Username,
                   u.Password,
                   u.Email
               }).FirstOrDefault();*/

            if (userToSave != null)
            {
               
                var uniqueEmail = await _myDbContext.IndexUser.FirstOrDefaultAsync(x => x.Username == userToSave.Username);

             if(uniqueEmail == null)
              { 
              var indexUser = new IndexUser
                {
                    Username = userToSave.Username,
                    Password = userToSave.Password,
                    Email = userToSave.Email,
                    Fname = userToSave.Fname,
                    Lname = userToSave.Lname,
                    UserId = userToSave.Id.ToString()
              };

                await _myDbContext.IndexUser.AddAsync(indexUser);
                await _myDbContext.SaveChangesAsync();
                   
                }
           

            }

            return RedirectToAction("Index");
        }// end of metod


        public async Task<IActionResult> DeleteUser(int id)
        {
            //var list_user = await _mydbcontext.indexuser.tolistasync();
            var deleteUser = await _myDbContext.IndexUser.FindAsync(id);
            if(deleteUser != null)
            {
                _myDbContext.IndexUser.Remove(deleteUser);
                await _myDbContext.SaveChangesAsync();
            }
            
           return RedirectToAction("Index");
           
        }


    }
}
