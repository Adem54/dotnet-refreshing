#region Serilog Settings
/* 
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()//log level i Information level dan baslasin diyrouz..yani normalde appsetting.json dan vermistik biz bu level ayarini , burda ise burdan verebilyoruz
                               //parametre nerye hangi path e kaydedecegini bekliyor
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Minute)//her gun e bir dosya olusturmasi icin bu rollingInterval:RollingInterval.Day bu ayari yapariz..Ama tabi hemen test edip gormek icin burayi simdilik minute yapariz bu sekiklde her 1 dkkada yeni bir log dosyasi olusturur, burayi Hour yaparsak da her 1 saatte bir yeni bir dosya olusturur calistgini gormek icin
    .CreateLogger();

Log.Information("Starting web application");
//builder.Services.AddSerilog(); //Boyle kullanirsak da serilog inbuild loglari override eder
//Eger builder.Host.UserSerilog() diye kullanirsak YA DA builder.Services.AddSerilog();  o zamn inBuild loglari override eder ve gostermez...hem inbuiltleri hem de seriolog u gormek istersek o zaman, AddSerilog diye eklemeliyiz yukardaki satirdaki gbii
builder.Logging.AddSerilog();//hem inbuild hem de serilog u gostermesi icin buirayi kullaniriz
*/
#endregion
using CollegaApp.Configurations;
using CollegaApp.Data;
using CollegaApp.Data.Repostory;
using CollegaApp.MyLogging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();//log4Net i kullanmadan once inbuild loglari temizliyoruz ki sadece log4Net i kullandigmzdan emin olmak icin
//Eklenilen Log4Net nuget paketini bu sekilde register ediyoruz
builder.Logging.AddLog4Net();


// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository eşlemesi (ARAYÜZ -> SINIF)
//builder.Services.AddScoped<IStudentRepostory, StudentRepostory>();
//Simdi bu register islemini de yapmamiz gerekiyor...
//builder.Services.AddScoped<IEntityRepostory<Student>, EntityRepostory<Student>>();
//Bu sadece Student için çalışır. Yarın Course da isterse, ayrı ayrı kayıt yazman gerekir. O yüzden genelde tercih edilmez (özel bir implementasyon/özel yaşam döngüsü vermek istediğinde istisnaen kullanılır).
//Not: Her iki kaydı birden eklersen ve IEntityRepostory<Student> istersen, kapalı (Student) olan daha spesifik kayda gider; aynı service type için birden fazla kayıt varsa son eklenen kazanır.

//GEneric types..registriation for DEP-INJ(DI)..BESTPRACTISE...Genel (open generic) kayıt yapiliyor.
builder.Services.AddScoped(typeof(IEntityRepostory<>), typeof(EntityRepostory<>));
//Bu satır, IEntityRepostory<Student>, IEntityRepostory<Course>, IEntityRepostory<Teacher>… gibi tüm kapalı tipleri otomatik çözer. Controller’da yaptığın:
//public StudentController(IEntityRepostory<Student> repo, ...)



//builder.Services.AddScoped<IMyLogger, LogToFile>();
builder.Services.AddScoped<IMyLogger, LogToDB>();

//json formati disinda bir format gonderilirse biz json disindaki formati desteklemesin istersek, bunu yaptigimzda artik desteklenmeyen formatlara exception firlatacak hata verecek
//builder.Services.AddControllers(options=>options.ReturnHttpNotAcceptable = true).AddNewtonsoftJson();
//xml e izin vermek iicn ise..xml data formatin desteklemesi icin asagidaki satiri ekleriz

//builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();


//Diyorum ki haci bak ben sana soylyorum, register ediyorum CollegeDBContext i ve senbunu al, bundan instance olustur ve her yeni requestte ve o request boyunc o instanceyi kullandir ve bir sonrkai rquest icin ise yeni bir instance olustur..Ve de connectino string i de appsettings.json da default tan al..ve context in bunu kullanarak db olustururken kullanmasini sagla...

builder.Services.AddDbContext<CollegeDBContext>(options =>
{
    //options uzerinden hangi Db yi kullaiyrosak onu soylememiz gerekkiyor..ve sonra da o db ye baglanamk icin gerekli stringler...
    //options.UseSqlServer("Data Source=localhost;Initial Catalog=CollegeAppDB;Integrated Security=True;Trust Server Certificate=True");
    var cs = builder.Configuration.GetConnectionString("CollegeAppDBConnection");
    Console.WriteLine($"ConnectionString: {cs}");//Dikkat edelim GetConnectionString bir extension yardimci fonksiyonudur
    options.UseSqlServer(cs);
    //Boyle de kulanabliriz ama bu sekilde tavsiye edilmez...direk boyle hassa bilgilerin aciktan yazilmasi
    //Db yi sifirdan olusturmak ve tablolari da koda yazip kodun calismasi ile olusturulmasi yaklasimi olan code first ile olustururken burda var olan bir db ismi degil olusturulmaisn istedigmz db ismini gireriz burda yazdimgz gibi CollegeAppDB
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));//Connectinostring
});

builder.Services.AddAutoMapper(cfg =>
{

}, typeof(AutoMapperConfig));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();//Burada controller'ları route'lara eşliyoruz.Eger biz Controller icreisinde, route lari belirtmezsek ve birden fazla ornegin HttpGet methodu varsa, hata aliriz,cunku route 'lar carpisir, ve hangi methodun cagrilacagini burasi bilemez...ve hata verir.Iste burada MapControllers() methodu ile bu route'lari esliyoruz...Burasi request uzerinden route islemi ile dogrudan hangi methodun cagrilacagini biliyor...ama route attributeunu ve HttpGet gibi attribute leri birden fazla kullanirsak onlari spesifik isimlerini de vermeliyiz ki burasi karisikligi cozebilsin...

app.Run();
