using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Demo.Controllers
{

    //sealed ile ValuesController classinin baska classlar tarafindan inherit edilmesi engellenir.
    //Oncelikle bu class in icerisine endpointlerimzin action larini eklememiz, yani methodlarimiz tanimlamamiz ve o methodlarin da disardan erisilebilir olmasi gerekiyor
    //Web-Api metjhodlari geriye bir sey return etmek zorundadir...!!!!

    [ApiController]
    //[Route("api/values")]=>https://localhost:7070/api/values/Calculate
    //[Route("[controller]")]//https://localhost:7070/Values/Calculate
    //[Route("")]//Direk action-methodudaki attrubte de ne verilirse sadece o gelir=>https://localhost:7070/Calculate
    [Route("myapi/[controller]")] 
    //asagida methoda [HttpGet("Calculate")] ise, ve icinde bul.dosya ismi ValuesController  https://localhost:7070/myapi/Values/Calculate..
    public sealed class ValuesController 
    {
        //  [HttpGet("Calculate")] //[Route("")]=>//https://localhost:7070/Calculate
        //  [HttpGet("Calculate")] //[Route("api/values")]=>https://localhost:7070/api/values/Calculate
      //  [HttpGet("Calculate")] //[Route("[controller]")] ile birlikte =>https://localhost:7070/api/values/Calculate 

        // [HttpGet("/Calculate")] //Yukardaki Route da ne oldugu farketmetz burayi absolute yapinca bu sekidle olur=> baştaki "/" → absolute route => https://localhost:7070/Calculate
        [HttpGet("[action]")]//https://localhost:7070/myapi/Values/MyCalculate
        public object MyCalculate()
        {

            return new { Message = "Hello World" };
        }
    }
    //Peki bu methodu disardan calistirip nasil tetiklenmesini saglayacagiz...
    //Bunun icin once bu methodun icinde bulundugu ValuesController classini disariya paylasima acmamiz gerekiyor. 
    //Iste bunun icinde biz gelip class in ustune  [Route("[api/values]")] attributunu ekleyecegiz
    //[Route("api/Values")] dersek, artik disardan birisi api/values yazdiginda bu class imiza ValuesController clasimiza erisebilir, ama Calculate metodumuza erisemez, cunku henuz mehtodu paylasmadik sadece classi paylastik
    //Methodu nasil paylasacagiz.1- Methodun ustune mehtodun hangi http method tipi ile erisilecegini belirtiriz Get, Post, Put, Delete gibi=> [HttpGet("Calculate")] 
    //Yani diyoruz ki internet adresine endpointe   [Route("api/values")]/ [HttpGet("Calculate")]  yazarsan, yani api/values/calculate yazarsan bu methoda erisebilirsin diyoruz
    //Biz projemizi run ettgimzde, debug modda, ISS bize bir localhost adres verir, oraya gelip /api/values/calculate yazarsak bu methodumuz tetiklenir ve bize 9 sonucunu return eder
    //
}
