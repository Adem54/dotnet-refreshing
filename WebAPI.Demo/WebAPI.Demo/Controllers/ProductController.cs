using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;

namespace WebAPI.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //Static liste nedir.. 
        //Start veya run a bastigimzda normalde bu class icindeki static olmayan fieldlar, propertyler, constructorlar vs vs hersey sifirlanir, yani baslangic degerlerine gelir, Her requestte tekrar tekrar baslangic degerlerine gelir,yani tekrar tekrar olusturulur.
        //Ama staticler uygulama basladiginda bir kere olusturulur ve uygulama kapatilana kadar bellekte kalir, yani static fieldlar, propertyler, constructorlar vs vs hersey uygulama kapatilana kadar bellekte kalir ve degerleri korunur. Yani her requestte tekrar tekrar olusturulmaz, sadece bir kere olusturulur ve degerleri korunur. Bu static lerin icerisine attgimiz datalar , degerler, uygulama kapatilana kadar bellekte kalir ve degerleri korunur aynen database gibi...Ama dikkat static ler sadece uygulama kapatilana kadar bellekte kalir, uygulama kapatilinca static ler de silinir, yani degerleri kaybolur.Staticler class leveldir, instance level degildir.

        public static List<Product> Products { get; set; } = new ();


        [HttpPost]
        public IActionResult CreateProduct(CreateProductDto dto)
        {
            //Validation yapalim-Data annotation ile yapalim...bunu model binding den once yapmaliyiz
            //1- Gelen data null mi, bos mu, degil mi, bunu kontrol edelim yani validation yapalim
            // Name 3 karakterden kucuk olamaz, 100 karakterden buyuk olamaz..Bunlar annotation ile halledildi

            //2.Gelen Dto yu Product a yani entity e cevirelim
            Product? product = dto.ToEntity();
            //Product product2 = ExtensionMethods.ToEntity(productDto);

            //3.Artik product u veritabanina kaydedebiliriz
            //DbContext i biz nasil aliyorduk..hatirlayalim...Bir tane ProductDbContext class i olusturuyorduk ve orda DbContext i inherit ediyorduk ve de orda DbSet<Product> Products { get; set; } tanimliyorduk, constructor parametre olarak DbContextOptions<ProductDbContext> options aliyorduk ve base e gonderiyorduk, sonra da, Entity lerle veritabanindaki tablolari mapping ediyorduk.. 
            //Hatirlayalim...bu dependency injeciton islemleri dotnet core tarafindan otomatik olarak yapilabilyordu dotnet 6 dan sonra Program.cs de builder.Services.AddDbContext<ProductDbContext>(options => options.UseInMemoryDatabase("ProductsDb")); seklinde yapiliyordu..Ve de builder.Build() den sonra IServiceProvidersi kullanarak, yani app.Services.CreateScope() ile scope olusturuyorduk ve de scope.ServiceProvider.GetRequiredService<ProductDbContext>() ile dbContext i aliyorduk ve de artik dbContext i kullanarak veritabanina erisebiliyorduk..Hatirlayalim...await dbContext.Products.AddAsync(product); await dbContext.SaveChangesAsync(); seklinde veritabanina kaydediyorduk.Yani Ioc Container dan dbContext i alip kullanabiliyorduk
            //Ama simdilik biz static listeye ekleyelim
            Products.Add(product);
            //SaveChanges da database e kaydederdi, dikkat Add isleminde kaydetmezdi, Add isleminde entity Added state e alinir ve de Id si 0 olurdu, SaveChanges yapinca veritabanina kaydederdi ve de Id si veritabaninin otomatik olarak verdigi Id ile update edilirdi
            //Ve son olarak artik response dto muzu return edebiliriz 
            //Bu product icinde Id de kaydedildi...DIKKAT UNUTMA...
            return Created("", product.ToDetailDto());//201 diye gider
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //1.si burda biz kullaniciya ne donecegiz,
            //return dbContext.Products.Include(p=>p.Category).Select(p=>p.ToDetailDto()).AsNoTracking().ToList();
            //Seklinde ilk olarak Include ile Category i yukleriz, sonra da Select ile Product i ProductDetailDto ya cevirebiliriz, AsNoTracking ile de performans artisi saglayabiliriz, cunku biz sadece okuma yapiyoruz, degil mi...Ve de son olarak ToList ile listeye ceviririz
            //Include ile Category deyince Entity framework core arka planda bir join islemi yaparak, Category Id uzerinden Category i yukler
            return Ok(Products);//ProductController.Products da olabilir, ama static oldugu icin this olmaz

        }

        [HttpGet("{id}")] //api/product/5
        public IActionResult Get(int id)
        {
            //1.si gelen id uzerinden veritabaninda bu id li data var mi onu aryacagiz.. 
            //Arama islemini yaparken dogru arac ile yapmaliyiz...performans vs..acisindan..
            //Product? product = dbContext.Products.Find(id);
            Product? product = Products.Find(p=>p.Id == id);

            return product is null
                    ? NotFound()//404 ile gider..
                    : Ok(product.ToDetailDto());//200 ile gider
        }
        //Bu senaryoda BadRequest() değil, NotFound() doğru. 404 NotFound: İstek doğru ama aradığın kaynak yok.
        //Örn: /products/123 — format doğru, ama 123 id’li ürün veritabanında yok.
        //400 BadRequest: İstek hatalı veya geçersiz.
        //Örn: id formatı yanlış (/products/abc), zorunlu alan eksik, body JSON’ı bozuk, kurala aykırı query vb.
        //Sende /products/{id:int} gibi bir route varsa:
        //id zaten int’e parse edilemezse endpoint’e hiç gelmez → framework 404 döner (route eşleşmedi).
        //Endpoint’e geldiyse id geçerli demektir; bulunamadığında 404 döndür.


        [HttpPut("{id}")]//Rootparam olarak id yi aliyoruz //api/product/5
        public IActionResult Update(int id, UpdateProductDto updateProductDto)
        {
            //1.Validation-Oncelikle gelen data validatioin i annotation uzerinden yapilmali ya da fluentValidation, yada kendi extension methodlairmz 
            //2.Gelen id uzerinden veritabaninda bu id li data var mi onu aryacagiz.. 
          //  Product? existingProduct = dbContext.Products.Find(id);
            Product? existingProduct = Products.Find(p => p.Id == id);

            if (existingProduct is null)
            {
                return NotFound();//404 ile gider
            }
           
            //3.Artik existingProduct u veritabaninda update edecegiz ama 
           // dbContext.Entry(existingProduct).CurrentValues.SetValues(updateProductDto.ToEntity());
            Products.Where(p=>p.Id == id).Select(Product=>updateProductDto.ToEntity()).ToList();
            //Buraya dikkat edelim, existingProduct ile mevcut datayi aliyoruz, sonra da Entry ile o entity i aliyoruz, sonra da CurrentValues ile o entity in current values larini aliyoruz, sonra da SetValues ile gelen updateProductDto yu entity e cevirip set ediyoruz, yani update ediyoruz
            //dbContext.SaveChanges();

            //4.Artik response dto muzu return edebiliriz
            return Ok(existingProduct.ToDetailDto());//200 ile gider
            //return NoContent();//204 ile gider
            /*
             200 OK + body: Güncellenmiş kaydın temsilini döndürmek istiyorsan (client anında yeni değeri görsün) gayet doğru.
             204 No Content: “Güncellendi ama gövde göndermiyorum” dersen doğru. Özellikle client zaten son halini biliyorsa tercih edilir.
            İkisi de standarttır; takımın API sözleşmesine göre birini seç ve tutarlı ol. (Ben çoğu CRUD’da PUT = 200 + updated DTO tercih ederim.)
            Döndüğüm DTO, DB’deki güncellenmiş değer mi?”
            Evet, existingProduct EF Core tarafından track ediliyor.
            dbContext.Entry(existingProduct).CurrentValues.SetValues(updateProductDto.ToEntity());
            dbContext.SaveChanges(); // (öneri: await SaveChangesAsync)
            SetValues ile tracked entity üzerinde değişiklikler işaretlenir.
            SaveChanges döndüğünde DB’ye yazılmıştır; computed/DB-generated alanlar varsa (ör. ModifiedAt, RowVersion) bu aşamada entity’ye geri yansır.
            Dolayısıyla existingProduct.ToDetailDto() DB’ye yazılmış son haldir (DB’nin ürettiği alanlar da dahil).

             */
        }
        //


        [HttpDelete("{id}")]//Rootparam ile id yi aliyoruz..    //api/product/5    
        public IActionResult Delete(int id)
        {
            //Product? product = dbContext.Products.Find(id);
            Product? product = Products.Find(p => p.Id == id);

            if (product is null)
            {
                return NotFound();//404 ile gider
            }
           
           // dbContext.Products.Where(p=>p.Id == id).ExecuteDelete();//Bunu yapinca arka planda direkt delete from Products where Id = id seklinde sql sorgusu calisir.This line is just one shot is going to go straight in to the database find the entiyt and delete it right away, there is no need to do anything else here..and then we just return no content
            //ExecuteDelete() / ExecuteDeleteAsync() (EF Core 7+): LINQ ifadenizden tek seferde DELETE ... WHERE ... üretip hemen çalıştırır.
            //YANI BU ASLINDA Doğrudan SQL’le silme (tracking yok, tek atış) — Önerilir...
            Products.Remove(product);
            // //dbContext.Products.Remove(product);=>Remove(product) sadece entity’yi Deleted olarak işaretler (Change Tracker’da).Bundan dolayi sonrasinda SaveChanges yapman gerekir=>await dbContext.SaveChangesAsync();
            return NoContent();//204 ile gider
        }
    }

    //CreateProductDto ile kullanicinin form da inputlara girdigi datalar, submit yapilmasi ile gelir, Product a mapping yapilir ve veritabanina kaydedilir, sonra da ProductDetailDto ile kullaniciya donulur........
    //GELEN DTO DATASINI AL, ENTITY YE CEVIR, VERITABANINA KAYDET, SONRA DA ENTITY ILE KULLANICIYA RESPONSE EDILECEK DTO YA CEVIR, KAYDEDILMIS DATAYI DTO YA CEVIR VE DTO ILE KULLANICIYA RESPONSE YAP...
    //1.Gelen data null mi, bos mu, degil mi, bunu kontrol edelim yani validation yapalim
    //Gelen data nedir , CreateProductDto dur..Product degildir...yani aldigmiz data ProductDto dur reequstten gelen ve response ile giden Dto dur...Proudct in kendisi veritabanina kaydedilir...dikkat edelim... 
    //Aldgimz CreateProductionDto da dikkat...id yok...cunku id yi veritabanina kaydederken veritabanimiz otomatik olarak verecek...
    //Ama Production icinde id var...Cunku veritabanindan gelen data icinde id var...
    //Gelen Dto validation kontrolu ...Data Annotations,FluentValidation veya...kendimiz yazabiliriz..kullandiktan sonra burdan gecmis ise 
    //CreateProductionDto muzu, mapping ile Production a cevirmemiz gerekir...extension method kullanabiliriz..veya automapper kullanabiliriz..Herzaman aklimizda olsun...Production ile CreateProductionDto farkli seylerdir...Dto lar data transfer objeleridir...Dto lar ile veritabanina kaydetmeyiz...Dto lar ile veritabanimizdan data getirmeyiz...Dto lar sadece dis dunya ile iletisimi saglar...ProductionId, Name,Price,Category,CategoryId..vardir..ortak diger alanlar disinda, ama CreateProductionDto Name,Price,CategoryId vardir
    //Sonra artik Product u veritabanina kaydedebiliriz...
    //Peki return donerken ne donecegiz...dikkat..Product veritabaninda olustu..ve SaveChanges yaptgimzda otomatik olarak bizim CreateProductDto dan Product a cevirdigmz ama Id si hala 0 olan, Product in Id si yeni olusturulan Id ile update edilmis oluyor ve de return ederken ise ProductDetailDto ile, ProductId,Name,Price ve CategoryId datlari return ediliyor...
    //dbContext.Games.Add(game) ile entity Added state’e alınır ve game.Id (int ise) 0’dır.
    //await dbContext.SaveChangesAsync() çalışınca EF Core, veritabanının ürettiği kimliği (IDENTITY/AUTO INCREMENT) geri okur ve game.Id alanına yazar.
    //Yani SaveChanges’ten sonra game.Id artık yeni kaydın gerçek ID’sidir.
    //Not: Eğer anahtar tipin Guid ise, çoğu zaman ID’yi kaydetmeden önce sen üretirsin (game.Id = Guid.NewGuid()), yine SaveChanges’ten sonra değer aynı kalır.
    //Burda productDto yu aliyoruz ve veritabanina kaydediyoruz 
    // Created(new Uri(productDto.GetEncodedUrl()), new { productDto });//201 diye gider
    //Neden Genre nesnesini set etmiyoruz?
    //POST’ta yalnızca GenreId yeter. Genre navigation’ını set etmek zorunda değilsin; FK (GenreId) set ise EF ilişkiyi bilir
    //Okuma tarafında Genre lazım olursa .Include(x => x.Genre) ile yüklersin veya sadece GenreId dönen DTO kullanırsın (senin yaptığın gibi).

    //Klasik sınıf (mutable class)
    //Daha geleneksel; set edilebilir özellikler:
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; } = 0;
       // public Category Category { get; set; } = new Category();
        public Category Category { get; set; } = null!;

        public DateOnly ReleaseData { get; set; }

    }
    /*
     Neden record? DTO’larda değer-semantiği (eşitlik/with kopyaları) ve immutability (değişmezlik) isteriz. record bu ikisini doğal verir. record class = referans tip record (yazmasan da record → record class demektir).
    Sadece istek (POST/PUT) taşıyan DTO ve endpoint içinde değiştirmeye ihtiyaç yoksa: record + init rahat ve güvenli.İçeride özellikleri değiştireceksen: class + set pratik.
    Özet: record DTO için gayet uygun, ama özellikleri public yapmayı (ve init/set vermeyi) unutma; yoksa model binding çalışmaz.
     */
    public record class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(100,1000)] 
        public decimal Price { get; set; }
        public int CategoryId { get; set; } = 0;
        public DateOnly ReleaseData { get; init; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; } = 0;

        public DateOnly ReleaseData { get; set; }

    }


    //Dikkat edelim id, UpdateProductDto da olmayacak, id zaten ayri gelecek...
    public record UpdateProductDto
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; } = 0;
        public DateOnly ReleaseData { get; init; }
    }

    public static class  ExtensionMethods 
    {
        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ReleaseData = dto.ReleaseData
            };
        }


        public static Product ToEntity(this UpdateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ReleaseData = dto.ReleaseData
            };
        }

        public static ProductDetailDto ToDetailDto(this Product product)
        {
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ReleaseData = product.ReleaseData
            };
        }

    }

   
}
