using CollegaApp.Dtos;
using CollegaApp.Models;
using CollegaApp.MyLogging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegaApp.Controllers
{
     [ApiController] //Bu sınıfın bir API denetleyicisi olduğunu belirtir. Yani, ornegin 400 hatasi gibi otomatik davranislar ekler.
    [Route("api/[controller]")] //yazdığında, [controller] sınıf adından “Controller” ekini atıp kalanını koyar.Ornegin StudentController ise api/Student olur veya StudentCourseController ise api/StudentCourse olur...
    //[Route("api/[controller]/[action]")] action da bu controllerdaki method ismini de dinamik bir sekilde eklemeyi saglar
    public class StudentController : ControllerBase//ControllerBase sayesinde IActionResult gibi tipleri kullanabiliriz.
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        //[HttpGet("GetStudents")] //HTTP GET isteği için bu metodu kullanır.//https://localhost:7014/api/Student/GetStudents
        //[HttpGet][Route("All")]//https://localhost:7014/api/Student/All
        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        public ActionResult<IEnumerable<Student>> GetAll()
        {
            _logger.LogInformation("GetAll Students method is called");
            return CollegeRepostory.Students;
        }

        // [HttpGet("GetStudent/{id:int?}")]
        //[HttpGet("GetStudent/{studentId:int}")]
        [HttpGet]
        [Route("GetStudent/{studentId:int?}")] //https://localhost:7014/api/Student/GetStudent/1  veya https://localhost:7014/api/Student/GetStudent/1
        // public Student? Get(int? studentId)
        public ActionResult<Student?> Get([FromRoute(Name = "studentId")] int? id)
        {
            _logger.LogInformation(" Get Student by ID method is called");
            Student? student = CollegeRepostory.Students.Find(s => s.Id == id);//Student? List<Student>.Find(Predicate<Student> match);,the default value type T, reutrns the first element...Find(Predicate<T>) ⇒ tek öğe döndürür (yoksa null).
            return student == null ? NotFound() : student;
        }
        //id yi range yani bir sayi araliginda da verebiliriz :range(1,100)..Bunlara route constraints denir...
        //List of route constraints diyerek, google da aratabiliriz...Client tarafindan girilecek datayi - server tarafindan route-constraintslerle sinirlayabiliriz...
        //Bir adim daha ileri giderek eger ki hazir route-constraintleri de bizim isimize yaramazsa kendi custom route-constraintimzi de yazabiliriz IHttpRouteConstraint interface ini implement ederek...
        [HttpGet]
        // [Route("{id:int}", Name = "GetStudentById")]
        //  [Route("{id:min(1):max(100)}", Name = "GetStudentById")]
        [Route("{id:range(1,100)}", Name = "GetStudentById")]
        public ActionResult<Student> GetStudentById(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning($"Bad request..{id} must be greater than 0 and int");
                return BadRequest();
            }

            Student? student = CollegeRepostory.Students.FirstOrDefault(s => s.Id == id);
            if(student is null)
            {
                _logger.LogWarning("Not found, there is no student");
                return NotFound();
            }
            return Ok(student);
        }
        //alpahabetik karakterler icin :alpha...Bunlara route constraints denir...
        [HttpGet("by-name/{name:alpha}", Name = "GetStudentByName")]

        public ActionResult<Student?> GetStudentByName(string name)
        {
            //Burda name null veya empty gelebilir, bunu kontrol edelim.....ONEMLI...
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot  be null or empty");
            }
            //ActionResult<T> kullanarak ister direk T tipinde data dönebiliriz, istersek de IActionResult gibi davranabiliriz...
            //return CollegeRepostory.Students.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            Student? student = CollegeRepostory.Students.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (student is null)
            {
                return NotFound($"{name} is not found!");
            }
            return Ok(student);
        }

        [HttpGet("by-email/{email}", Name = "GetStudentByEmail")]//GetStudentByName bu isim bu route a ozel isimdir
        // HttpVerb attribute ile de route u tanimlayabilirz, istersek de Route attribute ile de tanimlayabiliriz...
        //[Route("GetByName/{name}", Name = "GetStudentByName")]
        public ActionResult<Student?> GetStudentByEmail(string email)
        {
            Student? student = CollegeRepostory.Students.Find(s => s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return student is null ? NotFound() : student;
        }

        [HttpGet("save")]//Buraya hicbirsey yazmassak hata aliriz..
        public IActionResult Save()
        {
            return Ok();//200 OK durum kodu döner
            // return CreatedAtAction(nameof(Save), new { studentId = 5 }, null);//201 Created durum kodu döner ve yeni oluşturulan kaynağın URL'sini belirtir.
            //return NoContent();//204 No Content durum kodu döner, genellikle başarılı bir işlemi belirtir ancak geri döndürülecek içerik yoktur.
            // return NotFound();//404 Not Found durum kodu döner, istenen kaynağın bulunamadığını belirtir.
            // return BadRequest("Geçersiz istek.");//400 Bad Request durum kodu döner, genellikle istemciden gelen hatalı bir isteği belirtir.
            //Redirection status code için: 301 Moved Permanently, 302 Found, 307 Temporary Redirect, 308 Permanent Redirect gibi durum kodları kullanılabilir.
            //500 Internal Server Error durum kodu döner, genellikle sunucu tarafında bir hata olduğunu belirtir.
        }

        [HttpGet("mystudent/{id:int}")]

        // [ProducesResponseType(typeof(Student), 200)]//Bu endpoint 200 durum kodu ile Student tipinde data döner
        //[ProducesResponseType(200, Type=typeof(Student))]//Bu endpoint 200 durum kodu ile Student tipinde data döner, yukardaki ile ayni sey aslinda
        //[ProducesResponseType(200)]//ActionResult<Student> oldugu icin 200 durum kodu ile Student tipinde data donecektir yukardakiler gibi illa ki bildirmemiz sart degildir
        [ProducesResponseType(StatusCodes.Status200OK)]//Bu endpoint 200 durum kodu ile Student tipinde data döner
                                                       // [ProducesResponseType(400)]//Bu endpoint 400 durum kodu dönebilir
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Bu endpoint 400 durum kodu dönebilir
        //[ProducesResponseType(404)]//Bu endpoint 404 durum kodu dönebilir
        [ProducesResponseType(StatusCodes.Status404NotFound)]//Bu endpoint 404 durum kodu dönebilir
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]//Bu endpoint 500 durum kodu dönebilir
        public ActionResult<StudentDto> GetMyStudent(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }
            var student = CollegeRepostory.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                _logger.LogError($"There is no student exist with this id: {id}");
                return NotFound();
            }
            //response data as DTO ALWAYS
            StudentDto studentDto = new StudentDto {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Address = student.Address
            };
            return Ok(studentDto);

        }


        [HttpPost("create", Name = "SaveStudent") ]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> Create([FromBody] CreateStudentDto studentDto)
        {
            //Custom validation logic for AdmissionDate
            //1.Directly adding error message to ModelState 
            /* if (studentDto.AdmissionDate < DateTime.Now)
             {

                 ModelState.AddModelError("AdmissionDate Error", "Admission date cannot be in the past");
                 return BadRequest(ModelState);
                 //2.Using Custom attribute to add error to ModelState

             } */

            //2.Using Custom attribute to add error to ModelState=>Check CustomValidators/DateCheckAttribute.cs this works like built-in data annotation attributes..If modelState is invalid, thanks to [ApiController] attribute on the controller class, automatic 400 BadRequest response will be returned with validation errors in the response body.


            //[ApiController] i yoruma alirsak, burda validation i kendimz yapmaliyiz... 
            // if(ModelState.IsValid == false) return BadRequest(ModelState);

            //Validation i once tum obje icin yapriz null mi diye...null ise BadRequest doneriz
            //dto olarak gelir, ve validatino yapilmalidir..FluentVaidatin, Annotatino veya kendimize ait extensino-validation metholarimz 
            //Gelen dto veritabanina kaydetmek icin entiye ya automapper ya da kendi extension  methodumz ya da direk manuel olarak converting yapabilirz 
            //Sonra entiyjmizi veritabanina kaydederiz..asyn-await ile yapariz genellikle 
            //Aardindan enity framework sayesinde kaydedilen entity id si otomatk olarak entiydde set edilir 
            //Ve biz db de olsuturulan entiymizi aliriz kullaniciya respnse olarak donemek istedigmiz dto ya donusturerek kullanicya da dto olarak response ederiz datamizi...
            if (studentDto is null) return BadRequest("Student object can not be null");
            //Name, Email, Address 
            if(string.IsNullOrWhiteSpace(studentDto.Name) ||
                (studentDto.Name.Length <= 3) || !(studentDto.Email.Contains("@")) ||
                string.IsNullOrWhiteSpace(studentDto.Email) ||
                string.IsNullOrWhiteSpace(studentDto.Address))
            {
                return BadRequest("Name, Email and Address are required fields");
            }

            //Manuel conversion from dto to entity
            Student student = new Student
            {
                Name = studentDto.Name,
                Email = studentDto.Email,
                Address = studentDto.Address
            };
            student.Id = CollegeRepostory.Students.Count() + 1;

            //Normally here we will save the student entity to database using async-await and entity framework
            CollegeRepostory.Students.Add(student);

            //Return the created student dto with 201 status code
            StudentDto responseDto = new StudentDto { Id = student.Id, Name = student.Name, Email = student.Email, Address = student.Address };

            //1.parameter route u hangi action ın kullanacagiz onu belirtiyoruz, 2.parametrede ise o action a gonderilecek route parametrelerini veriyoruz..yani GetStudentById action ına id parametresini gonderiyoruz
            //"GetStudentById", new { id=responseDto.Id  } bu iki data, link olarak newly created resource un url sini belirtir...
            //newly created studentDto yu donecegiz 3.parametrede de...
            return CreatedAtRoute("GetStudentById", new { id=responseDto.Id  }, responseDto);
            // Status-201, DATAYI RESPONSE EDIYOR ARDINDAN DA RESPOSNE HEADERS DA location: olarak bu url li verecektir...  https://localhost:7014/api/Student/5 , 
        }

        [HttpDelete("{id:int}")]
        public ActionResult<bool> Delete(int id)
        {
            if (id <= 0) return BadRequest($"{id} is either 0 or less");
            var myStudent = CollegeRepostory.Students.FirstOrDefault(s => s.Id == id);
            if (myStudent is null)
            {
                return NotFound($"{id} is not found");
                // return false;
            }

            CollegeRepostory.Students.Remove(myStudent);
            return true;
        }

     //   [HttpPut("update/{id:int}", Name = "ModifyStudent")] //FromRoute, 
        [HttpPut] //FromRoute,
        [Route("update/{id}")]
        //Burdan id:int sayesinde route constraint ile, id string gonderilme durumunda hata alinir 400 hatasi yani daha action a datalari gonderemeden 400 bad request hatasi alinir...
        //Ama eger ki id yi route constraint ile sinirlamazsak ve string bir deger gonderilirse, action a geldiginde model binding basarisiz olur ve id 0 olarak gelir...Bu durumda da id<=0 kontrolu ile badrequest donebiliriz...
        //Ama id int gonderilir ama veritabaninda o id li kayit yoksa, bu durumda notfound 404 donebiliriz...
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> Update([FromRoute]  int id, UpdateStudentDto updateStudentDto)
        {
            //1.Validation..gonderilen obje dto null mi? 
            if(updateStudentDto is null)return BadRequest("Student object can not be null");
            //updateStudentDto icerisindeki Name,Email, Address valid olma durumunu biz Data Annotations ile belirttik...Ama [ApiController] attribute unu controller classina eklemezsek, burda manuel olarak validation yapmaliyiz...ModelState.IsValid kontrolu nu ControllerBase yapar arkada ve ModelState valid degilse bunu ApiController farkeder ve otomatik 400 badrequest doner...
            //id yi updateStudentDto icerisinde donenlerde olabiliyor..bu da farkli bir yaklasimdir
            //2.Validatio i yaptik sonra, gonderilen id veritabaninda var mi? Bul, yokse NotFound don 
            //3.Id var ve Student veritabanindan bulundu ise simdi de gondeirlen dto yu bulunan Student e update et...mapping islemi yap..Manuel yapabiliriz, kendi extension metodlarimzi olusturabilirz ya da AutoMapper kullanabilirz 
            //4.Gelen veriyi donusturdugmz Student i Entityframework de veritabaninda update etmeliyz       dbContext.Entry(existingProduct).CurrentValues.SetValues(updateProductDto.ToEntity());	Tabi biz Studente mapping yapma islemini entityframwork update islemi parametresinde de yapabiliriz burdaki gibi..ama kafamiz ned olmasi icin ayri ayri ilk basta daha temiz olur
            //5.Bu islem de hallolduguna gore geriye donecegimz response u dto olarak donmek..Mevcut var olan student update edilen student i alip reseponse edecegimz StudentDetailDto ya mapping yapip kullanicya return ederiz...		
            var existingStudent = CollegeRepostory.Students.FirstOrDefault(s => s.Id == id);
            if (existingStudent is null) return NotFound($"{id} is not found");
            existingStudent.Name = updateStudentDto.Name;
            existingStudent.Email = updateStudentDto.Email;
            existingStudent.Address = updateStudentDto.Address;

            var studentDto = new StudentDto
            {
                Name = existingStudent.Name,
                Email = existingStudent.Email, 
                Address = existingStudent.Address
            };

            // return NoContent();//204 No Content doneriz cunku update islemi basarili oldu ama geriye donecek data yok..Eger NoContent donerrsek o zaman ActionResult<StudentDto> degil de IActionResult kullanmaliyiz method signature da
            return Ok(studentDto); 
        }

        //Student entity sinin ornegin 5 farkli propertysi var ise, client sadece mesela Name propertysi veya Email proprtysnin update edecek ise o zaman diger tum propertleri gondermek zorunda kalmaz ki diger tum propertyleri gereksiz yere gondermek zaten hem performans acisindan kotudur hem de guvenlik acisindan kotudur...O zaman ne yapmaliyiz?Partial update yapmaliyiz...Yani sadece guncellemek istedigimiz property leri gondeririz...Bunun icin HTTP PATCH methodu kullanilir...PUT methodu ile de partial update yapilabilir ama genellikle PATCH kullanilir bu is icin...
        [HttpPatch("partial-update/{id:int}")]///api/Student/partial-update/{id}
        /*
         [HttpPatch]
        [Route("partial-update/{id:int?}")]  Yukarda sadece HttpPatch icerisinde yapilan islem simdi Route ve HttpPatch ile bu sekilde yapilyor
         */
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //JsonPatchDocument sayesinde de partial update yapabiliriz ve bu sekilde UpdatePartialStudentDto kullanimina gerek kalmaz..
        public ActionResult<StudentDto> PartialUpdate([FromRoute] int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        {

            //1.Validation gonderilen Dto null mu, id veritabaninda var mi?(id nin route constraint ile int olmasi saglandi..int olmazsa zaten actiona gelmez 400 badrequest doner)
            if (patchDocument is null  || id <= 0 ) return BadRequest("Student object can not be null");
            Student existingStudent = CollegeRepostory.Students.Find(s => s.Id == id)!;
            if (existingStudent is null) return NotFound($"{id} is not found");

            var studentDTO = new StudentDto
            {
                Id = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email,
                Address = existingStudent.Address

            };
            //Burda clienttan gelen tum degisiklkleri patchDocument ile studentDTO ya uyguluyoruz
            patchDocument.ApplyTo(studentDTO,ModelState);//if anything goes wrong during applying the patchDocument to studentDTO, the errors will be added to ModelState
            //Bundan dolayi ModelState i kontrol etmeliyiz
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Simdi studentDTO dan gelen degerlerle existingStudent entity sini update edelim
            existingStudent.Name = studentDTO.Name;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Address;

            //Artik response edecegimz dto yu hazirlariz ve update olmus entity den datalari aliriz...
            /* YA BOYLE RETURN OK() ICINDDE DONEBILIRZ YA DA DIREK STUDNTDTO YI RETURN EDEBILIRIZ...IKISI DE DOGRU
            return new StudentDto
            {
                Id = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email,
                Address = existingStudent.Address

            }; */
            //YA DA NOCONTENT() OLARAK DA RETURN YAPABILIRIZ BU SEFER DE BOYLE YAPALIM
            return NoContent();
            //Patchi yaparken asagidaki paketleri de kullanarak daha profesyonel yapabiliriz...Biz biraz daha manuel yaptik!!
            //Microsoft.AspNetCore.JsonPatch nu kullanarak da patch islemi yapabiliriz...Burda manuel olarak yaptik...
            //Microsoft.AspNetCore.Mvc.NewtonsoftJson paketini yuklememiz lazim...Startup.cs veya Program.cs de AddNewtonsoftJson() eklemeliyiz...
            //Burda dotnet core versiyonumuz hangisi ise o versiyona uygun paketleri yuklemeliyiz bu cok onemlidir
            /*
                <TargetFramework>net8.0</TargetFramework>
                <PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="8.0.0" />
                <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.0" />

            Simdi dostum biz ne yaptik yeni bir service yukledik ve bunu kullanacagiz...HER ZAMAN ICIN ILK YAPACAGIMZ IS SU GIDIP PROGRAM.CS DE ADDNEWTONSOFTJSON I EKLEMEK OLACAK...BU COK ONEMLIDIR...YOKSA CALISMAZ...
            builder.Services.AddControllers().AddNewtonsoftJson();
            Soru su..2 paket yukledim... <PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="8.0.0" />
                <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.0" /> ama 1 tane bunu ekledim veya register ettim...neden her ikisini de register etmedim...ve ben nerdeen bilecegim her nuget paketi yukledigmde hangi yukledegimi register etmemm lazim hangsini register a gerek yok...ve neye gore yapacagim bu register islemini..JSONPATCH TUM CRUD OPERATSYONLARINI DESTEKLIYOR AMA BIZ BURDA SADECE PATCH ILE ILGILENDIK...YANI PARTIAL UPDATE ICIN KULLANACAGIZ
            The patch
            [
              { "op": "replace", "path": "/baz", "value": "boo" },=>update..icin
              { "op": "add", "path": "/hello", "value": ["world"] },=>create icin
              { "op": "remove", "path": "/foo" }=>delete icin
            ]
            Biz bunu kullanacagiz: { "op": "replace", "path": "/baz", "value": "boo" },=>update..icin
            RequestBody:[
              {
                "path": "/name",//hangi alani modify etmek istiyoruz..
                "op": "replace",//replace i kendisi veriyor yukarda update icin olan kisimda..
                "value": "My doughter Zehra"
              }
            ]
            Evet bu sekilde istek gonerilince sadece name propertisini gonderiyor digerlerin gondermiyor

            [
              {
                "path": "/email",
                "op": "replace",
                "value": "zehramodified@gmail.com"
              }
            ]
            boyle yaparsak da bu sefer de...neyi modify eder... dikkat...email i modify eder...sadece...
            PEKI MULTIPLE PROPERTY UPDATE I NASIL YAPARIZ...O ZAMAN DA ASAGIDAKI GIBI GONDEREBILRIZ...
            [
              {
                "path": "/name",
                "op": "replace",
                "value": "ZehraModified"
              },
            {
                "path": "/email",
                "op": "replace",
                "value": "zehraupdated@gmail.com"
              }
            ]
            Microsoft.AspNetCore.JsonPatch bir “kütüphane” (tipler/yardımcılar) sağlar; “servis” değildir.BU KUTUPHANEYI DE BU ARADA NASIL KULLANACAGIZ, CLIENT TARAFINDAN DATA GONDERIRKEN NASIL GONDERILMELI VS jsonpatch.com dan kontrol edeblriiz
            Microsoft.AspNetCore.Mvc.NewtonsoftJson ise MVC’ye bir formatlayıcı (serializer) entegre eder; bu yüzden “register” edilir.

            Soru:ormal create,update de data annotatin ile ModelState in hata eklenmesi BaseController uzerinden gerceklesiyordu ve ModelState te hata olunca ApiController buna 400 hata gonderiyordu da...burda nedden o gerceklesmiyor

            cevap: [ApiController]’ın otomatik 400 davranışı yalnızca model binding aşamasında (aksiyon parametreleri bağlanırken) oluşan hatalara çalışır.
            Sen ise hataları aksiyon içinde, patchDocument.ApplyTo(studentDTO, ModelState) çağrısıyla sonradan ModelState’e ekliyorsun. Bu yüzden otomatik 400 tetiklenmez; senin elle kontrol etmen gerekir.

             */
        }
    }
}
/*
 Microsoft.AspNetCore.Routing.Matching.AmbiguousMatchException: The request matched multiple endpoints. Matches: CollegaApp.Controllers.StudentController.GetStudentByEmail (CollegaApp) CollegaApp.Controllers.StudentController.GetStudentById (CollegaApp) CollegaApp.Controllers.StudentController.GetStudentByName (CollegaApp)

     AmbiguousMatchException sebebi: birden fazla endpoint aynı URL şablonunu yakalıyor.

    Senin denetleyicide çakışanlar:

    [Route("{id}")] → her şeyle eşleşir (string de olur, int de)

    [HttpGet("{name}")]

    [HttpGet("{email}", Name="GetStudentByName")] → (ayrıca yanlış ad)

    Ayrıca [Route("GetStudent/{studentId:int?}")] opsiyonel int ile bazı çağrılarda çakışmayı büyütüyor.

    Attribute routing’de ekleme sırası önemli değildir; şablonlar aynı yolu yakalıyorsa framework hangi action’a gideceğini bilemez ve bu hatayı atar.


ISTE ASAGIDAKI ENDPOINTLER I BIRLIKTE KULLANDIGMZDAN DOLAYI  YUKARDA KI HATA MESAJIN IALDIK:

  //[HttpGet("GetStudent/{studentId:int}")]
        [HttpGet]
        [Route("GetStudent/{studentId:int?}")] //https://localhost:7014/api/Student/GetStudent/1  veya https://localhost:7014/api/Student/GetStudent/1
        // public Student? Get(int? studentId)
        public Student? Get([FromRoute(Name ="studentId")] int? id)
        {
            Student?  student = CollegeRepostory.Students.Find(s=>s.Id == id);//Student? List<Student>.Find(Predicate<Student> match);,the default value type T, reutrns the first element...Find(Predicate<T>) ⇒ tek öğe döndürür (yoksa null).
            return student == null ? null : student;
        }

        [HttpGet]
        [Route("{id}")]
        public Student GetStudentById(int id)
        {
            return CollegeRepostory.Students.FirstOrDefault(s => s.Id == id)!;
        }

        [HttpGet("{name}")]
        public Student? GetStudentByName(string name)
        {
            return CollegeRepostory.Students.FirstOrDefault(s=>s.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
        }

        [HttpGet("{email}", Name = "GetStudentByName")]//GetStudentByName bu isim bu route a ozel isimdir
        // HttpVerb attribute ile de route u tanimlayabilirz, istersek de Route attribute ile de tanimlayabiliriz...
        //[Route("GetByName/{name}", Name = "GetStudentByName")]
        public Student? GetStudentByEmail(string email)
        {
            Student? student = CollegeRepostory.Students.Find(s => s.Name.Equals(email, StringComparison.OrdinalIgnoreCase));
            return student == null ? null : student;
        }
}


Nasıl düzeltiriz? (temiz ve çakışmasız)

ID için tip kısıtı kullan: {id:int}
Ad ve e-posta için statik segment ekle: by-name/{name}, by-email/{email}
Route “Name”’leri doğru ver (CreatedAtRoute vb. için)
E-posta aksiyonunda yanlışlıkla Name ile arama yapmışsın; Email ile ara.

COZUM ASAGDAKI GIBIDIR:

     [HttpGet]
        [Route("GetStudent/{studentId:int?}")] //https://localhost:7014/api/Student/GetStudent/1  veya https://localhost:7014/api/Student/GetStudent/1
        // public Student? Get(int? studentId)
        public Student? Get([FromRoute(Name ="studentId")] int? id)
        {
            Student?  student = CollegeRepostory.Students.Find(s=>s.Id == id);//Student? List<Student>.Find(Predicate<Student> match);,the default value type T, reutrns the first element...Find(Predicate<T>) ⇒ tek öğe döndürür (yoksa null).
            return student == null ? null : student;
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        public Student GetStudentById(int id)
        {
            return CollegeRepostory.Students.FirstOrDefault(s => s.Id == id)!;
        }

        [HttpGet("by-name/{name}", Name = "GetStudentByName")]
        public Student? GetStudentByName(string name)
        {
            return CollegeRepostory.Students.FirstOrDefault(s=>s.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
        }

        [HttpGet("by-email/{email}", Name = "GetStudentByEmail")]//GetStudentByName bu isim bu route a ozel isimdir
        // HttpVerb attribute ile de route u tanimlayabilirz, istersek de Route attribute ile de tanimlayabiliriz...
        //[Route("GetByName/{name}", Name = "GetStudentByName")]
        public Student? GetStudentByEmail(string email)
        {
            Student? student = CollegeRepostory.Students.Find(s => s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return student == null ? null : student;
        }


Neden bu çözüm çalışıyor?

{id:int} kısıtı, sayısal olmayan çağrıların ID endpoint’ine düşmesini engeller.

by-name/... ve by-email/... statik ön ekleri sayesinde URL uzayı ayrışır, “tek parametreli her şey” şablonları kalmaz.

Route Name değerleri benzersiz ve doğru: GetStudentById, GetStudentByName, GetStudentByEmail.
 */
