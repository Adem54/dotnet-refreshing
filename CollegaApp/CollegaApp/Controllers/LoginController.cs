using CollegaApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[EnableCors(PolicyName ="AllowAll")]
    [Authorize(Roles = "Superadmin,Admin")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public  ActionResult Login (LoginDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }
            //username ve passwordun bos birakilmadan gonderildigi kontrol edildikten sonra, simdi de gelen username-password un dogrulugunu cek etmeliyiz...database den

            //Test icin db de su an datamiz olmadigi icin hardcoded olarak gostereceigz ama bu islem db den kontrol edilecek tabi ki
            if(model.UserName == "Adem" && model.Password = "Adem1234")
            {
                //O zaman tamam kullanici dogru kullanici...demektir..
            }
        }
    }

    

  
}
