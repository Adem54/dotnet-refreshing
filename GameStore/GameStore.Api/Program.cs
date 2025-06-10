using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//tum endpointleri app(WebApplication type) extension method ekledik ve sayfayi temiz hale getirmeis olduk
app.MapGamesEndpoints();
app.Run();



