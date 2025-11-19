using AutoMapper;
using CollegaApp.Data;
using CollegaApp.Data.Repostory;
using CollegaApp.Dtos;
using CollegaApp.Migrations;
using CollegaApp.MyLogging;
using log4net.Util.TypeConverters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CollegaApp.Controllers
{
     [ApiController] //Bu sınıfın bir API denetleyicisi olduğunu belirtir. Yani, ornegin 400 hatasi gibi otomatik davranislar ekler.
    [Route("api/[controller]")] //yazdığında, [controller] sınıf adından “Controller” ekini atıp kalanını koyar.Ornegin StudentController ise api/Student olur veya StudentCourseController ise api/StudentCourse olur...
    //[Route("api/[controller]/[action]")] action da bu controllerdaki method ismini de dinamik bir sekilde eklemeyi saglar
    public class StudentController : ControllerBase//ControllerBase sayesinde IActionResult gibi tipleri kullanabiliriz.
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        //private readonly IStudentRepostory _studentRepostory;
      //  private readonly IEntityRepostory<Student> _studentRepostory;
        private readonly IStudentRepostory _studentRepostory;
        //IEntityRepostory we are using application level repostory pattern..not studentlevel.

        public StudentController(ILogger<StudentController> logger, CollegeDBContext dbContext, IMapper mapper, IStudentRepostory studentRepostory)
        {
            _logger = logger;
            _mapper = mapper;
            _studentRepostory = studentRepostory;
        }

        //[HttpGet("GetStudents")] //HTTP GET isteği için bu metodu kullanır.//https://localhost:7014/api/Student/GetStudents
        //[HttpGet][Route("All")]//https://localhost:7014/api/Student/All
        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            _logger.LogInformation("GetAll Students method is called");
            //return _dbContext.Students;//Direk Database deki data yi donmeyiz..1.security icin onerilmez, 2.data transfrom objects leri kullaniriz request-response larda data olarak..
            /*  return await _dbContext.Students.AsNoTracking().Select(s=>new StudentDto()
              {
                  Id = s.Id,
                  Name = s.Name,
                  Email = s.Email,
                  Address = s.Address,
                  DOB = s.DOB.ToShortDateString()

              }).ToListAsync(); */

            //var students = await _dbContext.Students.ToListAsync();
            var students = await _studentRepostory.GetAllAsync();
            var studentDTOData = _mapper.Map<List<StudentDto>>(students);
            //destination StudentDto, source is students..This is how we can auomatically transform, or copy from entity object to dto object
            //students is List, StudentDto is single class..
            return Ok(studentDTOData);

            //var result = _mapper.Map<List<StudentDto>>(students);
            //Ya kendi helper-extension metodlarimizla .ToStudentDto...deriz ya da direk manuel olarak burda yapariz..yada AutoMapper da kullanabiliriz..
            /*
            NEDEN Select(...) ile DTO’ya projekte ediyoruz?
            “Projeksiyon” (project etmek) = veriyi başka bir şekle dönüştürmek demek.LINQ/SQL dünyasında projection, bir tablodaki satırları farklı bir yapıya veya alanların bir alt kümesine çevirmektir.
            C#’ta bu genelde Select(...) ile yapılır: Entity → DTO (veya anonim tip) gibi.
            Sonuç: DB’den yalnızca gerekli sütunlar (Id, Name, Email) gelir ve API bu şekle döner.

            NEDEN Select(...) ile DTO’ya projekte ediyoruz?
            1.Security / veri sızıntısı
            Entity’de istemediğin alanlar olabilir (örn. PasswordHash, IsDeleted, teknik sütunlar).
            return _dbContext.Students.ToList() dersen hepsi JSON’a gider.
            Select(new StudentDto { ... }) ile sadece gereken alanları döndürürsün.

            2.Şema bağımsızlığı & versiyonlama
            DB şeman değişse bile DTO sözleşmesini korursun. Frontend entity’ye kilitlenmez.

            Sık yapılan hatalar
            ToList()’i Select’ten önce çağırmak → tüm satırları RAM’e alır, sonra dönüştürür (kaçın).
            Entity döndürmek → fazla kolon, güvenlik riski, büyük JSON.
            AsNoTracking() unutmak → gereksiz change tracker overhead.
            
            3.Performans (kolon/row azaltma)
            Select SQL düzeyinde sadece seçtiğin kolonları getirir:
            Entity döndürürsen tüm kolonlar gelir (gereksiz I/O + payload).

            4.Takip maliyeti (tracking)
            Okuma endpoint’lerinde genelde AsNoTracking() isteriz. DTO projeksiyonunda bunu rahat uygularız.
            Entity döndürmek genelde tracking açık anlamına gelir (gereksiz).

            5.Döngüsel referans / navigasyon sorunları
            Entity’de Courses -> Students -> Courses gibi navigasyonlar varsa JSON’da döngü ve büyük payload riski.
            6.AutoMapper / ProjectTo avantajı
            ProjectTo<StudentDto>() ile projeksiyon SQL’e çevrilir ve sadece gerekli alanlar gelir.

            DIKKATET
            Entity döndürme: kolay ama riskli (güvenlik, performans, sözleşme).
            DTO’ya Select: doğru pratik; yalnızca gereken kolonlar, daha güvenli ve stabil API.
            Üretimde: AsNoTracking + Select DTO (+ AutoMapper ProjectTo) kalıbını kullan

             */
        }


        //[HttpGet("GetStudent/{id:int?}")]
        //[HttpGet("GetStudent/{studentId:int}")]
        [HttpGet]
        [Route("GetStudent/{studentId:int?}")] //https://localhost:7014/api/Student/GetStudent/1  veya https://localhost:7014/api/Student/GetStudent/1
        // public Student? Get(int? studentId)
        public async Task<ActionResult<StudentDto?>> Get([FromRoute(Name = "studentId")] int id)
        {
            _logger.LogInformation(" Get Student by ID method is called");
            //Student? student = await _dbContext.Students.FindAsync(id);//Student?
            /*Student? student = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s=>s.Id == id );*///Student?

           // Student? student = await _studentRepostory.GetByIdAsync(id);
            Student? student = await _studentRepostory.GetAsync(s=>s.Id == id);

            //Student? student2 = _dbContext.Students.FirstOrDefault(s=>s.Id == id);
            //Find(id) imzasi bu sekildedir IQUERYABLE da, yani db de yapilan sorguda, ama Find(IPredicate) olarak calisir ram tarafinda dotnette...Find(s=>s.Id==id) seklinde... 
            //FirsOrDefault=>Once match olan first dataya bakar bulursa onu alir devam eder, bulamazsa Default olarak null verir..

            if (student is null)
            {
                _logger.LogWarning("Not found, there is no student");
                return NotFound();
            }
            //var studentDto = new StudentDto()
            //{
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Address = student.Address,
            //    DOB = student.DOB.ToShortDateString()
            //};
            var studentDto = _mapper.Map<StudentDto>(student);
            //return Dto not entity
            return student == null ? NotFound() : studentDto;
        }
        
        [HttpGet]
        // [Route("{id:int}", Name = "GetStudentById")]
        //  [Route("{id:min(1):max(100)}", Name = "GetStudentById")]
        [Route("{id:range(1,100)}", Name = "GetStudentById")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning($"Bad request..{id} must be greater than 0 and int");
                return BadRequest();
            }

          //  Student? student = await _studentRepostory.GetByIdAsync(id);
            Student? student = await _studentRepostory.GetAsync(s=>s.Id == id);
            if (student is null)
            {
                _logger.LogWarning("Not found, there is no student");
                return NotFound();
            }

            //var studentDto = new StudentDto()
            //{
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Address = student.Address,
            //    DOB = student.DOB.ToShortDateString()
            //};

            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }
        //alpahabetik karakterler icin :alpha...Bunlara route constraints denir...
      //[HttpGet("by-name/{name:alpha}", Name = "GetStudentByName")]
        [HttpGet("by-name/", Name = "GetStudentByName")]

        public async Task<ActionResult<StudentDto?>> GetStudentByName(string name)
        {
            //Burda name null veya empty gelebilir, bunu kontrol edelim.....ONEMLI...
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot  be null or empty");
            }
            //ActionResult<T> kullanarak ister direk T tipinde data dönebiliriz, istersek de IActionResult gibi davranabiliriz...
            //return CollegeRepostory.Students.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            //Artik parametre de predcate-lambda(arrow-func) olacak..
           // Student? student = await _studentRepostory.GetByNameAsync(name); bu sadece Student e ozel repostory yazdigmzda boyle idi ama artik tum, entiytlere yonelik bir repostry i olusturduk
            Student? student = await _studentRepostory.GetAsync(s => EF.Functions.Collate(s.Name, "Norwegian_100_CI_AS") == name);
            if (student is null)
            {
                return NotFound($"{name} is not found!");
            }

            //var studentDto = new StudentDto()
            //{
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Address = student.Address,
            //    DOB = student.DOB.ToShortDateString()
            //};
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }

        [HttpGet("by-email/{email}", Name = "GetStudentByEmail")]//GetStudentByName bu isim bu route a ozel isimdir
        // HttpVerb attribute ile de route u tanimlayabilirz, istersek de Route attribute ile de tanimlayabiliriz...
        //[Route("GetByName/{name}", Name = "GetStudentByName")]
        public async Task<ActionResult<Student?>> GetStudentByEmail(string email)
        {
           // Student? student = await _studentRepostory.GetByEmailAsync(email);
            Student? student = await _studentRepostory.GetAsync(s=>s.Email== email);

            if (student is null)
            {
                return NotFound($"{email} is not found!");
            }
            //var studentDto = new StudentDto()
            //{
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Address = student.Address,
            //    DOB = student.DOB.ToShortDateString()
            //};

            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
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
        public async Task<ActionResult<StudentDto>> GetMyStudent(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }
          //  var student = await _studentRepostory.GetByIdAsync(id);
            var student = await _studentRepostory.GetAsync(s=>s.Id == id);
            if (student == null)
            {
                _logger.LogError($"There is no student exist with this id: {id}");
                return NotFound();
            }
            //response data as DTO ALWAYS
            //StudentDto studentDto = new StudentDto {
            //    Id = student.Id,
            //    Name = student.Name,
            //    Email = student.Email,
            //    Address = student.Address,
            //    DOB = student.DOB.ToShortDateString()
            //};
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }
        [HttpPost("create", Name = "SaveStudent") ]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto studentDto)
        {
           
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
            //DIKKKAT ENTITY EKLEMESI OLACAGI ICIN BURDA ENTIYTCLASS I  KULLANILMALIDIR
            //BILMEMIZ GEREKEN BIRSEY ID GONDERILMEZ CREATE ISLEMINDE, ID GENELLIKLE DB DE AUTOMATIK BIR SEKILDE OLUSTURULACAKTIR...
            //Student student = new Student
            //{
            //    Name = studentDto.Name,
            //    Email = studentDto.Email,
            //    Address = studentDto.Address,
            //    DOB = studentDto.AdmissionDate
            //};

            var student = _mapper.Map<Student>(studentDto);
            var createdStudent = await _studentRepostory.CreateAsync(student);
            //Normally here we will save the student entity to database using async-await and entity framework..AYRICA DA DIKKAT EDELIM...KI BURDA SAVECHANGES GERCEKLESTGINDE..ARTIK student direk DB DE KAYDETTIT DATA ILE DOLDUREACK student i yani ID de yine doldurulmus olacak....

            //Return the created student dto with 201 status code
            StudentDto responseDto = new StudentDto { Id = createdStudent.Id, StudentName = createdStudent.Name, Email = createdStudent.Email, Address = createdStudent.Address };

            //1.parameter route u hangi action ın kullanacagiz onu belirtiyoruz, 2.parametrede ise o action a gonderilecek route parametrelerini veriyoruz..yani GetStudentById action ına id parametresini gonderiyoruz
            //"GetStudentById", new { id=responseDto.Id  } bu iki data, link olarak newly created resource un url sini belirtir...
            //newly created studentDto yu donecegiz 3.parametrede de...
            return CreatedAtRoute("GetStudentById", new { id= responseDto.Id  }, responseDto);
            // Status-201, DATAYI RESPONSE EDIYOR ARDINDAN DA RESPOSNE HEADERS DA location: olarak bu url li verecektir...  https://localhost:7014/api/Student/5 , 
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (id <= 0) return BadRequest($"{id} is either 0 or less");
          //  var studentToDelete = await _studentRepostory.GetByIdAsync(id);
            var studentToDelete = await _studentRepostory.GetAsync(s=>s.Id == id);
            if (studentToDelete is null)
            {
                //return false;
               return NotFound($"No student found with id: {id}");
            }
            await _studentRepostory.DeleteAsync(studentToDelete);
            //_dbContext.Students.Where(s => s.Id == id).ExecuteDelete(); bu da tek basina delete islemine alternatif bu direk db den siler bu sekilde
            return true;
        }

        //[HttpPut("update/{id:int}", Name = "ModifyStudent")] //FromRoute, 
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
        public async Task<ActionResult<StudentDto>> Update([FromRoute]  int id, UpdateStudentDto updateStudentDto)
        {
            //1.Validation..gonderilen obje dto null mi? 
            if (updateStudentDto is null)return BadRequest("Student object can not be null");
            //updateStudentDto icerisindeki Name,Email, Address valid olma durumunu biz Data Annotations ile belirttik...Ama [ApiController] attribute unu controller classina eklemezsek, burda manuel olarak validation yapmaliyiz...ModelState.IsValid kontrolu nu ControllerBase yapar arkada ve ModelState valid degilse bunu ApiController farkeder ve otomatik 400 badrequest doner...
            //id yi updateStudentDto icerisinde donenlerde olabiliyor..bu da farkli bir yaklasimdir
            //2.Validatio i yaptik sonra, gonderilen id veritabaninda var mi? Bul, yokse NotFound don 
            //3.Id var ve Student veritabanindan bulundu ise simdi de gondeirlen dto yu bulunan Student e update et...mapping islemi yap..Manuel yapabiliriz, kendi extension metodlarimzi olusturabilirz ya da AutoMapper kullanabilirz 
            //4.Gelen veriyi donusturdugmz Student i Entityframework de veritabaninda update etmeliyz       dbContext.Entry(existingProduct).CurrentValues.SetValues(updateProductDto.ToEntity());	Tabi biz Studente mapping yapma islemini entityframwork update islemi parametresinde de yapabiliriz burdaki gibi..ama kafamiz ned olmasi icin ayri ayri ilk basta daha temiz olur
            //5.Bu islem de hallolduguna gore geriye donecegimz response u dto olarak donmek..Mevcut var olan student update edilen student i alip reseponse edecegimz StudentDetailDto ya mapping yapip kullanicya return ederiz...		
            //var existingStudent = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            //if (existingStudent is null) return NotFound($"{id} is not found");

            //1.yontem
            //  _dbContext.Entry(existingStudent)
            //.CurrentValues
            //.SetValues(updateStudentDto);
            //  _dbContext.SaveChanges();

            //  _dbContext.SaveChanges();

            //2.yontem manuel olarak existingStudent.Name = updateStudentDto.Name sekklinde kolonlari ayri ayri guncelleriz ve en son _dbContext.SaveChanges(); yapariz

            //3-.YONTEM var newRecord = new Student(){ icerisini parametrede gelen degerle doldururuz..sadece Id=existingStudent.Id olacak, Name=updateStudentDto.Name....gibi gerceklesecek...Sonra _dbContext.Students.Update(newRecord); _dbContext.SaveChanges(); seklinde yapabiliriz...
            //var newRecord = new Student()
            //{
            //    Id = existingStudent.Id,
            //    Name = updateStudentDto.Name,
            //    Address = updateStudentDto.Address,
            //    Email = updateStudentDto.Email,
            //    DOB = updateStudentDto.DOB
            //};
          //  Student? existingStudent = await _studentRepostory.GetByIdAsync(id);
            Student? existingStudent = await _studentRepostory.GetAsync(s=>s.Id == id);

            if (existingStudent is null) return NotFound($"{id} is not found");
             _mapper.Map(updateStudentDto, existingStudent);

            // _dbContext.Students.Update(newRecord);
            //await _dbContext.SaveChangesAsync();
            await _studentRepostory.UpdateAsync(existingStudent);

            /*
             'Student' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the 
             */

            var studentDto = new StudentDto
            {
                StudentName = existingStudent.Name,
                Email = existingStudent.Email, 
                Address = existingStudent.Address,
                DOB = existingStudent.DOB.ToShortDateString()
            }; 

            // return NoContent();//204 No Content doneriz cunku update islemi basarili oldu ama geriye donecek data yok..Eger NoContent donerrsek o zaman ActionResult<StudentDto> degil de IActionResult kullanmaliyiz method signature da
            return Ok(studentDto);
            /* update e alternatif olarak exstingStuden in tek tek Name,email,Address ini gelen updateStudentDto datasinin degerleri ile guncelleriz ki existingStudent db den cekecegimzdata..vede en son SAveChanges dersek o zaaman da direk veritabainda da artik updat dilmis olur...BU DA SU ANKI UPDATE ISLEMININ BIZRAZ DAHA ESKI VERSIYONU DIYEBLIRIZ..*/
        }


        [HttpGet("studentindex")]
        public IActionResult Index()
        {
            _logger.LogTrace("Log mesage from trace method");
            _logger.LogDebug("Log mesage from Debug method");
            _logger.LogInformation("Log mesage from Information method");
            _logger.LogWarning("Log mesage from warning method");
            _logger.LogError("Log mesage from error method");
            _logger.LogCritical("Log mesage from critical method");
            return Ok();
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
        public async Task<ActionResult<StudentDto>> PartialUpdate([FromRoute] int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        {

            //1.Validation gonderilen Dto null mu, id veritabaninda var mi?(id nin route constraint ile int olmasi saglandi..int olmazsa zaten actiona gelmez 400 badrequest doner)
            if (patchDocument is null  || id <= 0 ) return BadRequest("Student object can not be null");
          //  Student? existingStudent = await _studentRepostory.GetByIdAsync(id, true);
            Student? existingStudent = await _studentRepostory.GetAsync(s=>s.Id == id, true);
            if (existingStudent is null) return NotFound($"{id} is not found");

            //var studentDTO = new StudentDto
            //{
            //    Id = existingStudent.Id,
            //    Name = existingStudent.Name,
            //    Email = existingStudent.Email,
            //    Address = existingStudent.Address

            //};

            //Efcore existingStudent i track ediyor...ve bizim bunu studentDto ya cevirmemiz existinDto nun track edilmeisni degistirmiyor o hala track ediliyor,
            var studentDto = _mapper.Map<StudentDto>(existingStudent);
            //Burda clienttan gelen tum degisiklkleri patchDocument ile studentDTO ya uyguluyoruz
            patchDocument.ApplyTo(studentDto, ModelState);//if anything goes wrong during applying the patchDocument to studentDTO, the errors will be added to ModelState
            //Bundan dolayi ModelState i kontrol etmeliyiz
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Simdi studentDTO dan gelen degerlerle existingStudent entity sini update edelim
            //existingStudent.Name = studentDto.Name;
            //existingStudent.Email = studentDto.Email;
            //existingStudent.Address = studentDto.Address;
            //existingStudent.DOB = Convert.ToDateTime(studentDto.DOB);//string date i DateTime a cevirmemiz gerekiyor

            //Biz boyle yaptik olmadi ama su anki calisan kod gibi kullaninca mapper i o zaman update gerceklesti...Yukarda existingStudent i biz AsNOTracking yaptik ve tracker e takip ettirmedigmz icin burda hata almadan, database de burasinin takip edilmesini ve update eedilmesni sagldik cunku id ler uzerinden takip yaptigi icin, 2 kez ayni id yi farkli db oprasynlarinda takip edemez...
             existingStudent = _mapper.Map<Student>(studentDto);
            //_dbContext.Students.Update(newStudent);
            await _studentRepostory.UpdateAsync(existingStudent);
            // await _dbContext.SaveChangesAsync();
            //_mapper.Map(studentDto, existingStudent);

            //   _dbContext.Entry(existingStudent)
            //.CurrentValues
            //.SetValues(patchDocument);
            // await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        /*
            EGER ORNEGIN SADECE DOB DateTime property sini degistirmek istersek:
        endpoint’in şöyle bir şey bekliyor:
        [HttpPatch("partial-update/{id:int}")]
        public ActionResult<StudentDto> PartialUpdate(
        [FromRoute] int id, 
        [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        Bu da demek oluyor ki body bir JSON Patch dokümanı olacak:
        Yani bir dizi operasyon: [{ op, path, value }] gibi.'

        Sadece DOB alanını güncellemek
        Property adı DOB olduğu için JSON Patch’te path şu olacak:
        "/dob"   veya   "/DOB"
        Eğer AddNewtonsoftJson() ile default ayar kullanıyorsan, property isimleri PascalCase kalır → DOB.
        Swagger’da response JSON’unda alan adın dob mu DOB mu diye bak; aynı casing’i path’te kullan.
        Örnek body (sadece DOB’u değiştirmek)
        Swagger’da PATCH endpointine gidip body’yi şöyle yaz:

        [
          {
            "op": "replace",
            "path": "/dob",
            "value": "2026-11-02T10:38:58.457Z"
          }
        ]

        VEYA..ASAGIDAKI GIBI:
        [
          {
            "op": "replace",
            "path": "/dob",
            "value": "2026-11-03T10:38:58.457Z"
          }
        ]

        Bu arada bir kere biz bu icerigi ekldgimzde de swagger de execute edince hata alabilirz...Ondan dolayi oncelikle su asagdaki gerekliliklerin yapildigindan emin olalim:
        a) NuGet paketleri

            Proje (.csproj) içinde şunlar olmalı:
             <ItemGroup>
              <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.0" />
              <PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="8.0.0" />
            </ItemGroup>

        (Version numaraları senin .NET/ASP.NET Core sürümüne uygun olmalı; örnek diye 8.0.0 yazdım.)
        b) Program.cs / Startup.cs içine kayıt

        builder.Services
            .AddControllers()
            .AddNewtonsoftJson();  // ⬅️ BUNU EKLEMEK ZORUNDA

        // Eski hali sadece şuna benzemesin:
        // builder.Services.AddControllers();
        Eğer builder.Services.AddControllers(); yazıp AddNewtonsoftJson() eklemezsen, ASP.NET Core System.Text.Json ile çalışır ve JsonPatchDocument<StudentDto> body’yi parse edemez → senin gördüğün hata.

        Swagger’da doğru Content-Type

        [
          {
            "op": "replace",
            "path": "/dob",
            "value": "2026-11-02T10:38:58.457Z"
          }
        ]
        Ama üstte Media type kısmında:
        application/json yerine
        application/json-patch+json seçersen daha doğru olur.
        Çoğu zaman ASP.NET Core ikisini de kabul eder ama JSON Patch için resmi tip application/json-patch+json.
         */
    }
}
