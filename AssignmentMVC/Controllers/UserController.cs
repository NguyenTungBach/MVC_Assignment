using AssignmentMVC.Data;
using AssignmentMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AssignmentMVC.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private MyIdentityDbContext myIdentityDbContext; // Bên Xác thực và Phân quyền
        private UserManager<User> userManager; //Bên database
        private RoleManager<Role> roleManager; //Bên database
        public UserController()
        {
            myIdentityDbContext = new MyIdentityDbContext();  // giống Connection Helper
            UserStore<User> userStore = new UserStore<User>(myIdentityDbContext); // create, update, delete giống UserModel
            userManager = new UserManager<User>(userStore); // giống Service, xử lý các vấn đề liên quan đến logic
            RoleStore<Role> roleStore = new RoleStore<Role>(myIdentityDbContext); // create, update, delete giống UserModel
            roleManager = new RoleManager<Role>(roleStore); // giống Service, xử lý các vấn đề liên quan đến logic
        }
        public ActionResult Index()
        {
            return View(myIdentityDbContext.Users.ToList());
        }

        public ActionResult Register()
        {
            return View();
        }

        public async Task<bool> AddUserToRoleAsync(string UserId, string RoleName)
        {
            var user = myIdentityDbContext.Users.Find(UserId);
            var role = myIdentityDbContext.Roles.AsQueryable().Where(roleFind => roleFind.Name.Contains(RoleName)).FirstOrDefault();
            if (user == null || role == null)
            {
                return false;
            }
            var result = await userManager.AddToRoleAsync(user.Id, role.Name);
            //string roleName1 = "Admin";
            //string roleName2 = "User";
            ////var result = await userManager.AddToRoleAsync(userId, roleName);
            //var result = await userManager.AddToRolesAsync(userId, roleName1, roleName2); // Thêm nhiều Role cho 1 User
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                ViewBag.Errors = result.Errors;
                System.Diagnostics.Debug.WriteLine("Lỗi tạo quyền có lỗi là ", result.Errors);
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string Email, string PasswordHash, string PhoneNumber, string UserName, string IdentityCard)
        {
            User user = new User()
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                IdentityCard = IdentityCard,
            };

            var result = await userManager.CreateAsync(user, PasswordHash);
            if (result.Succeeded)
            {
                var queryUser = myIdentityDbContext.Users.AsQueryable().Where(userFind => userFind.UserName.Contains(UserName)).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine("Tìm user có name là: ", UserName);
                System.Diagnostics.Debug.WriteLine("Tạo quyền User cho user có id là: ", queryUser.Id);
                if (queryUser == null)
                {
                    ViewBag.ErrorNull = "Không tìm thấy khi queryUser";
                    System.Diagnostics.Debug.WriteLine("Tạo quyền User cho user có id là: ", queryUser.Id);
                    return View("ViewError");
                }
                var check = await AddUserToRoleAsync(queryUser.Id, "User");
                if (check)
                {
                    return View("ViewSuccess");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi tạo quyền");
                    return View("ViewError");
                }
            }
            else
            {
                ViewBag.Errors = result.Errors;
                System.Diagnostics.Debug.WriteLine("Lỗi đăng ký là ", result.Errors);
                return View("ViewError");
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string UserName, string PasswordHash)
        {
            var user = await userManager.FindAsync(UserName, PasswordHash);
            Debug.WriteLine("user đăng nhập là ", user);
            if (user == null)
            {
                ViewBag.Errors = new string[] { "Không tìm thấy user" };
                return View("ViewError");
            }
            else
            {
                SignInManager<User, string> signInManager = new SignInManager<User, string>(userManager, Request.GetOwinContext().Authentication);
                await signInManager.SignInAsync(user, false, false);

                return Redirect("/Home");
            }
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return Redirect("/Home");
        }
    }
}