using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using WebApplicationMVC.Entities;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _dataContext;  //vt Ctrl. ile Constructor ekle
        private readonly IConfiguration _conf;          //ayarları al Ctrl. ile Add

        public AccountController(DatabaseContext dataContext, IConfiguration conf)
        {
            //her sefer çalışacak
            _dataContext = dataContext;
            _conf = conf;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(GirisViewModel model)
        {
            if (ModelState.IsValid)
            {
                //login işlemleri
                string mdTuz = _conf.GetValue<string>("AppSettings:md5Salt");
                string tuzluSifre = (model.Parola + mdTuz).MD5(); //Ctrl. 
                string hashPassword = tuzluSifre; //güvenlik şifre görülmesin

                User user = _dataContext.Users.SingleOrDefault(x => x.UserName.ToLower() == model.Ad.ToLower()
                                && x.Password == hashPassword && x.Locked == false);
                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();         //çanta
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.FullName ?? String.Empty)); //boş ise
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    claims.Add(new Claim("Username", user.UserName));

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("Giriş", nameof(model.Ad) + " hatalı giriş, kullanıcı kilitli olabilir");
                    return View(model);
                }
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(KayitViewModel model)
        {
            if (ModelState.IsValid)
            {
                //register işlemleri

                //öndecen kullanıcı ekli mi
                if (_dataContext.Users.Any(x => x.UserName.ToLower() == model.Ad.ToLower()))
                {
                    ModelState.AddModelError("Kayıt", nameof(model.Ad) + " üyesi zaten var");
                    View(model);
                }
                else
                {
                    User user = new User();
                    user.UserName = model.Ad;
                    user.Password = tuzluParola(model.Parola);                    

                    _dataContext.Users.Add(user);
                    int neoldu = _dataContext.SaveChanges();
                    if (neoldu == 0)
                    {
                        ModelState.AddModelError("Kayıt", "Üye eklenemedi");
                    }
                    else
                    {
                        //başarılı kullanıcı eklenme
                        return RedirectToAction(nameof(Login));
                    }
                }
            }
            return View();
        }

        public IActionResult Profile()
        {
            veriOku();
            return View();
        }

        private void veriOku()
        {
            Guid userID = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dataContext.Users.SingleOrDefault(x => x.Id == userID);
            ViewData["FullName"] = user.FullName;
        }

        [HttpPost]
        public IActionResult ProfilChangeFullName([Required][StringLength(30)] string FullName)
        {
            if (ModelState.IsValid)
            {
                Guid userID = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _dataContext.Users.SingleOrDefault(x => x.Id == userID);
                user.FullName = FullName;

                _dataContext.SaveChanges();

                ViewData["result"] = "AdDegisti";
            }
            else
            {
                ModelState.AddModelError("Hata", "Güncelleme yapılamadı");
            }

            veriOku();
            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfilChangePassword([Required][StringLength(30)] string Password)
        {
            if (ModelState.IsValid)
            {
                Guid userID = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _dataContext.Users.SingleOrDefault(x => x.Id == userID);

                user.Password = tuzluParola(Password);

                _dataContext.SaveChanges();

                ViewData["result"] = "ParolaDegisti";  
            }
            else
            {
                ModelState.AddModelError("Hata", "Güncelleme yapılamadı");
            }

            veriOku();
            return View("Profile");
        }

        private string tuzluParola(string Password)
        {
            string mdTuz = _conf.GetValue<string>("AppSettings:md5Salt");
            string tuzluSifre = (Password + mdTuz).MD5(); //Ctrl. 
            return tuzluSifre; //güvenlik şifre görülmesin                
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
