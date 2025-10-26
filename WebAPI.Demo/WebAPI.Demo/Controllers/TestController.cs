using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Demo.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    [Route("api/test/[action]")]
    public class TestController : ControllerBase
    {

        [HttpGet("{x}/{y}/{name}")]//Bu yazdigmz yerine adi roottur unutmayalaim...
        public IActionResult GetData(int x, int y, string name)
        {
            return Created(new Uri(Request.GetEncodedUrl()), new { Message=$"x: {x} - y:{y} - name: {name}" });//201 diye gider
            //Uri sayesinde response da headers a: location: https://localhost:7070/api/test/GetData  bunu yazar..
        }
        //RESPONSE MANTIGINA BAKALIM DOTNETCORE WEB API DE..
        //Response donerken, her zaman response status codu ile birlikte doneriz ve de mesaji da doneriz, tabi data var ise data da doneriz...Http-response-status-codes 100-199(Information-response) 200-299(Successfull) 300-399(Redirection) 400-499(Client Error) 500-599(Server Error)
        //Iste kullaniciya ben standart bir response donebilmem icin dotnet core bize bir cok helper classlar hazir olarak sunuyor, bizde bu helper classlari kullanarak, kullaniciya standart response donebiliriz..Bu helper classlar Microsoft.AspNetCore.Http.HttpResults namespace i icerisinde bulunuyor..Ve ornek verecek olursak da bunlar:IActionResult, OkResult, OkObjectResult, NotFoundResult, NotFoundObjectResult, BadRequestResult, BadRequestObjectResult, CreatedResult, CreatedAtRouteResult, CreatedAtActionResult, NoContentResult vs vs
        //Sen git IActionResult return et, sonra da geriye bir numaratik kod donecek return cagir diyor...
        //Numaratik kod donen IActionResult i implemente etmis classlar var, mesela OkResult, NotFoundResult, BadRequestResult, CreatedResult, NoContentResult vs vs
        //200 Ok ve ben bir data da donmek istiyorum diyorsan Ok(data) kullanacaksin
        // 204 NoContent ve ben data donmek istemiyorum diyorsan NoContent() kullanacaksin
        //201 Created ve ben data donmek istiyorum diyorsan Created(uri, data) kullanacaksin
        //Iste su andaki endpointin durumu bestpractise lere uyuyor, cunku IActionResult return ediyor ve de 200 Ok ile birlikte data da donuyor
        //[HttpPost]
        //public IActionResult CreateProduct(ProductDto request)
        //{
          
        //    return StatusCode(200, new { request });//200 diye gider
        //}




    }

    //public class ProductDto
    //{ 
       
    //    public Product? Product { get; set; } 
    //    public Category Category { get; set; } = new Category();
    //}
    //public class Product
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = "";
    //    public decimal Price { get; set; }

    //}
    //public class Category
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = "";  
    //}


    //return Ok(new { Message = "Successfull result"});//200 diye gider
    //return NoContent();//204 diye gider
    //return Created(new Uri(Request.GetEncodedUrl()), new { Message = "Created" });//201 diye gider
    //Uri sayesinde response da headers a: location: https://localhost:7070/api/test/GetData  bunu yazar..
    // NotFound() -404
    //return Problem(detail: "Bir hata olustu", statusCode: 500, title: "Server Error");
    //return NoContent();//204 diye gider 
    //method isimleri insensitive..yani kucuk harf buyuk harf farketmiyor default olarak endpoint isimlendirmesinde, ama istersek bunu sensitive hale getirebiliriz
}