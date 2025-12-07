using CollegaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[EnableCors(PolicyName ="AllowAll")]
    //[Authorize]=>“Bu controller’daki bütün action’lar için önce authentication yap.Token yoksa / geçersizse → 401 Unauthorized dön.”Yani Login action’ı da korumalı olmuş oluyor.
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration  _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }


        [HttpPost]
        public  ActionResult Login (LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }
            //username ve passwordun bos birakilmadan gonderildigi kontrol edildikten sonra, simdi de gelen username-password un dogrulugunu cek etmeliyiz...database den
            //assign the username here
            LoginResponseDTO response = new() {UserName = model.UserName};

            //Test icin db de su an datamiz olmadigi icin hardcoded olarak gostereceigz ama bu islem db den kontrol edilecek tabi ki
            //User credential validation gerceklesiyor burda
            if (model.UserName == "Adem" && model.Password == "Adem1234")
            {
                //O zaman tamam kullanici dogru kullanici...demektir..VE EVET ARTIK BURDA JWT TOKEN URETECEGIZ...
                //1-Oncelikle secret-keyi appsettgings.json dan alalim
                //var secretKey = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTSecret") ?? "");
                //Eger ki norvece ozl spesifk karakterler icerir ise secret key orn:øæ gibi o zamna UTF8 kullanmak daha mantikli olur ASCII YERINE BUNLAR ASCII TABLOSUNDAKI VEYA UTF8 DEKKI BYTE KARSILNI ALIR
                var secretKey = Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecret") ?? "");
                //Dikkat biz secretkey i appsettings.jsondan alirken Configuration uzerinden almistikl o zaman bizim burda da Configuration i bu class injekte etmemiz gerekkior ku kulanaibelim
                //Token handler i olusturmamiz gerekiyor..yani token i olustrmada yardim edecek 
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        //Username
                        new Claim(ClaimTypes.Name, model.UserName),
                        //if any role, we enter the role, but for now we just use hard-coded but next tutors, we can get from DB
                        //Role
                        new Claim(ClaimTypes.Role, "Admin")
                    }),//Claimsleri girdikten sonra(yani kimlik ile ilgili verilen datalar), sonra expire time a gecilir 
                    Expires = DateTime.Now.AddHours(4),//4 saat gecerli olmasini istiyoruz
                    //Simdi de signing credentaials i gireriz...
                    SigningCredentials = new(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
                    //HmacSha512Signature bu algoritmayi kualllaniryoz bu serviste
                    /*
                     new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256
                     */
                };
                //tokenDescriptor bittingden sonra ok , tamam biz token i uretebiliriz.artik...
                var token = tokenHandler.CreateToken(tokenDescriptor);
                //token is being generated 
                var tokenGenerated = tokenHandler.WriteToken(token);
                //token olusturluyor burda
                //Token is generated..we can put this inside respsenes
                //And we assigned the toem here
                response.Token = tokenGenerated;
            }
            else
            {
                //Burda garip bir return yaptik hem success gonderdik, yani senin gonderdigin request e cevap veriyoruz ama username ve passwordun valid degil...diyoruz..
                return Ok("Invalid username and password");
            }

            return Ok(response);

        }

        [HttpGet("GetAllData")]
        [Authorize]
        public ActionResult GetAll()
        {
            return Ok("This is the test result");
        }
    }

    

  
}
