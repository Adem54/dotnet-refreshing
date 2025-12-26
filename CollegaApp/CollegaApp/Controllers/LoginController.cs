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
    //[Authorize]=>“Bu controller’daki bütün action’lar için önce authentication yap.Token yoksa / geçersizse → 401 Unauthorized dön.”Yani Login action’ı da korumalı olmuş oluyor.bu login de olmayacak...login e herkes girebilmeli..
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

            var secretKeyForGoogle = Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecretForGoogle") ?? "");
            var secKeyForMicr = _config.GetValue<string>("JWTSecretForMicrosoft") ?? "";
            var secretKeyForMicrosoft = Encoding.UTF8.GetBytes(secKeyForMicr);
            var secretKeyForLocal = Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecretForLocal") ?? "");
            //Test icin db de su an datamiz olmadigi icin hardcoded olarak gostereceigz ama bu islem db den kontrol edilecek tabi ki
            var issuerForGoogle = _config.GetValue<string>("GoogleIssuer") ?? "";
            var issuerForMicrosoft = _config.GetValue<string>("MicrosoftIssuer") ?? "";
            var issuerForLocal = _config.GetValue<string>("LocalIssuer") ?? "";
            var audienceForGoogle = _config.GetValue<string>("GoogleAudience") ?? "";
            var audienceForMicrosoft = _config.GetValue<string>("MicrosoftAudience") ?? "";
            var audienceForLocal = _config.GetValue<string>("LocalAudience") ?? "";

            //User credential validation gerceklesiyor burda
            if (model.UserName == "Adem" && model.Password == "Adem1234")
            {
                //model.Policy front-end tarafindan gonderilecek..fetch yaparken payload gonderiken veya body gonderirken icerisinde username, password ve de policy="Local", policy="Google"vs seklinde gonderir..
                /*
                 {
                      "policy": "Google",
                      "userName": "Adem",
                      "password": "Adem1234"
                    }
                 */
                byte[] secretKey = null;
                string audience = string.Empty;
                string issuer = string.Empty;
                    switch (model.Policy)
                    {
                    case "Google":
                        secretKey = secretKeyForGoogle;
                        issuer = issuerForGoogle;
                        audience = audienceForGoogle;
                        break;

                    case "Local":
                        secretKey = secretKeyForLocal;
                        issuer = issuerForLocal;
                        audience = audienceForLocal;

                        break;

                    case "Microsoft":
                        secretKey = secretKeyForMicrosoft;
                        issuer = issuerForMicrosoft;
                        audience = audienceForMicrosoft;
                        break;

                    default:
                        secretKey = secretKeyForLocal;
                        issuer = issuerForLocal;
                        audience = audienceForLocal;
                        break;


                    }

                    //O zaman tamam kullanici dogru kullanici...demektir..VE EVET ARTIK BURDA JWT TOKEN URETECEGIZ...
                    //1-Oncelikle secret-keyi appsettgings.json dan alalim
                    //var secretKey = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTSecret") ?? "");
                    //Eger ki norvece ozl spesifk karakterler icerir ise secret key orn:øæ gibi o zamna UTF8 kullanmak daha mantikli olur ASCII YERINE BUNLAR ASCII TABLOSUNDAKI VEYA UTF8 DEKKI BYTE KARSILNI ALIR
                  //  var secretKey = Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecretForLocal") ?? "");
                //Dikkat biz secretkey i appsettings.jsondan alirken Configuration uzerinden almistikl o zaman bizim burda da Configuration i bu class injekte etmemiz gerekkior ku kulanaibelim
                //Token handler i olusturmamiz gerekiyor..yani token i olustrmada yardim edecek 
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    //issuer ve audience yi bizim buraya gecmemiz gerekiyor ki token olusturulmasinda kullanilsin..tokendescriptor token generate ederken issuer ve audience yi kullanacak
                    //Yani bu 2 property otomatik claims e eklenecek..ve kullanicinin bunlari dogru gondermesi gerekecek..login olurken
                    Issuer = issuer,
                    Audience = audience,
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
        [Authorize(AuthenticationSchemes = "LoginForGoogleUsers", Roles ="Superadmin,Admin")]
        //[Authorize(AuthenticationSchemes = "LoginForGoogleUsers", Roles ="Superadmin")]
        //403 Forbidden gelir cunku, token olustururken,  new Claim(ClaimTypes.Role, "Admin") da Role Admin olarak girerek token olusturuldu Login endpointinde......ok..ki bunu anlamak icin de zaten...jwt.io ya gidip decode kisminda token i yapistirip, secret key kismina da google icin olan secretkeyimiz i yapistirirsak bize header, payloud u verir paylaod da role un admin geldingi gorebiliriz...
        /*
         "unique_name": "Adem",
          "role": "Admin",
          "nbf": 1765147375,
          "exp": 1765161775,
          "iat": 1765147375
            }
        */

        [ProducesResponseType(StatusCodes.Status200OK)]
        //Swaggerda authenticiaon mekanizamsini kullanabilmek icin once buraya bunlari ekleriz...
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//token hatali(yani credentaials hatali)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]//token dogru ama role yanlis, yetki yok
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetAll()
        {
            return Ok("This is the test result");
        }
    }

    

  
}
