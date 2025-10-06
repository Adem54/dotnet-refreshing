using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);


//Connection to Sqllite 
//var connString = "Data Source=GameStore.db";//connection string 
var connString = builder.Configuration.GetConnectionString("GameStore");//parametre de appsettings.json icindeki connectionsstring deki key i verirriz..value ye erismek icin..Burdaki Configuration objesi IConfiguration interface ini implement ediyhor ki bu IConfiguration tum, configurtion lari topluyor...
//builder.Configuration uzerine mouse ile gittgimzde bize sunu gosterir zaten=>ConfigurationManager WebApplicationBuilder.Configuration { get; }
//register the services, burda da register yapiliyor
builder.Services.AddSqlite<GameStoreContext>(connString);
//Arkada builder.Services.AddScoped<GameStoreContext> calisiyor...
//Peki neden Scoped lifetime olarak kaydedliyor GameStoreContext
//Her bir httprequest icin yeni bir ServiceScope olusturulur... 
//Cunku DB connections are limited and they are relatively expensive resource
//DbContext i guvenli bir sekilde acip kapatmis oluyor, ve de threat safe degil ayni zamanda DbContext..
//Havin a single instance acroos multiple requests would lead into concurrency issues, we don't want that..data tutarliligi acisindan sorunlara sebep olabilir.  
//Make easier transactions...unit of work...without interference from multiple requests 
//Performance icin de daha kullanisli

//iste burasi sayesinde bizim GameStoreContextimiz ile bir instance olusturulacak...ve tabi ki nereye gidyor Data/GameStoreContext te constructor daki parametrelleri de alip db ye connect oluyor

var app = builder.Build();

//tum endpointleri app(WebApplication type) extension method ekledik ve sayfayi temiz hale getirmeis olduk
app.MapGamesEndpoints();

//Exthention methodu uygulariz burda..
app.MigrateDb();//DataExtensions.MigrateDb(app);


app.Run();

//DbContext imiz yani GameStoreContext imizi application a nasil haber vercegiz nasil register edecegiz
//Bizi application a nasil, bizim sqllite db mizie bizim GameStoreConteximizi kullanark nasil connect olacagini soylememiz gerekiyor...
//Register etmemiz gerekiyor context imizi uygulamanin basinda...dikkat edelim...
//Simdi biz ne demistik bak dikkat...Builder objectimzi kullaniyorduk..bircok islem icin...burasi uygulamayi hazirliyor...du..Register services icinde Build objesini kullanacagiz

