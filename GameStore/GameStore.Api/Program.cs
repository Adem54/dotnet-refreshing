using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//Connection to Sqllite 
var connString = "Data Source=GameStore.db";//connection string 
//register the services 
builder.Services.AddSqlite<GameStoreContext>(connString);
//iste burasi sayesinde bizim GameStoreContextimiz ile bir instance olusturulacak...ve tabi ki nereye gidyor Data/GameStoreContext te constructor daki parametrelleri de alip db ye connect oluyor


var app = builder.Build();

//tum endpointleri app(WebApplication type) extension method ekledik ve sayfayi temiz hale getirmeis olduk
app.MapGamesEndpoints();
app.Run();

//DbContext imiz yani GameStoreContext imizi application a nasil haber vercegiz nasil register edecegiz
//Bizi application a nasil, bizim sqllite db mizie bizim GameStoreConteximizi kullanark nasil connect olacagini soylememiz gerekiyor...
//Register etmemiz gerekiyor context imizi uygulamanin basinda...dikkat edelim...
//Simdi biz ne demistik bak dikkat...Builder objectimzi kullaniyorduk..bircok islem icin...burasi uygulamayi hazirliyor...du..Register services icinde Build objesini kullanacagiz

