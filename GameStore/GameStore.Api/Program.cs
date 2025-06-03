var builder = WebApplication.CreateBuilder(args);
//instance of webapplication...as host..host our application
//represent httpserver implementation for our app, and so it can start listening for http requests..
//it stands up a bunch of mware components, a login services dependency injection services, configuration services
//Bunch of services we are going to talking about across this
//And you can cofigure over here if we expand this between these two lines...var app = builder..and var builder = Web.. 
//you could go ahead and always just type builder. and depend on your needs
//We are going to work alot this builder object to introduct services as we go across this prosject...

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

//You can do bunch of things by using app. member operator..so..

app.Run();

/*  GameStore.Api.csproj   dosyasi nedir, proje dosyasi diye adlandirdimgiz dosya nedir ne ise yarar
This file defines some of the information
*/


