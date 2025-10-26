using CollegaApp.MyLogging;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()//log level i Information level dan baslasin diyrouz..yani normalde appsetting.json dan vermistik biz bu level ayarini , burda ise burdan verebilyoruz
     //parametre nerye hangi path e kaydedecegini bekliyor
    .WriteTo.File("Log/log.txt", rollingInterval:RollingInterval.Minute)//her gun e bir dosya olusturmasi icin bu rollingInterval:RollingInterval.Day bu ayari yapariz..Ama tabi hemen test edip gormek icin burayi simdilik minute yapariz bu sekiklde her 1 dkkada yeni bir log dosyasi olusturur, burayi Hour yaparsak da her 1 saatte bir yeni bir dosya olusturur calistgini gormek icin
    .CreateLogger();

Log.Information("Starting web application");
//builder.Services.AddSerilog(); //Boyle kullanirsak da serilog inbuild loglari override eder
//Eger builder.Host.UserSerilog() diye kullanirsak YA DA builder.Services.AddSerilog();  o zamn inBuild loglari override eder ve gostermez...hem inbuiltleri hem de seriolog u gormek istersek o zaman, AddSerilog diye eklemeliyiz yukardaki satirdaki gbii
builder.Logging.AddSerilog();//hem inbuild hem de serilog u gostermesi icin buirayi kullaniriz


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IMyLogger, LogToFile>();
builder.Services.AddScoped<IMyLogger,LogToDB>();

//json formati disinda bir format gonderilirse biz json disindaki formati desteklemesin istersek, bunu yaptigimzda artik desteklenmeyen formatlara exception firlatacak hata verecek
//builder.Services.AddControllers(options=>options.ReturnHttpNotAcceptable = true).AddNewtonsoftJson();
//xml e izin vermek iicn ise..xml data formatin desteklemesi icin asagidaki satiri ekleriz

//builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();



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
