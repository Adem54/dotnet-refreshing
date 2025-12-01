using EFDBFirst.Models;
using Microsoft.EntityFrameworkCore;

//
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
//Eger sadece 1 tane AddPolicy kullanacasak boyle kullaniriz yoksa options dan sonra suslu parantezlerle acip icerisine arka arkaya birden fazla options.AddPolicy ekleyebilirz
//builder.Services.AddCors(options => options.AddPolicy("TestCORS", policy => policy.WithOrigins("http://example.com")));

builder.Services.AddCors(options =>
{
    //Mesela burda AddPolicy yerne AddDefaultPolicy secenegini de kullanablirz..eger oyle uygun ise..Ama AddPolicy kullaniyorsa named policy eklemek istiyruz demektir
   // options.AddPolicy(name: MyAllowSpecificOrigins,
   //Direk string i de yazabilirz
    options.AddDefaultPolicy(
        policy =>
        {
            //policy.WithOrigins("http://example.com", "http://www.contoso.com");
            //How to allow all the header, all the origins and all the methods... 
            //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            //AllowAnyMethod=>get,post,put,delete,options...
            //Headers can be user agent,content type, content language
            //Origins(https(schema)+domain withe extension(www.example.com)+port number(443)
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();//=>allows all origins

        });

    //options.AddPolicy("AllowAll", policy =>
    //{
    //    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    //});

    options.AddPolicy("AllowOnlyLocalhost", policy =>
    {
        // policy.WithOrigins("https://test.website.com");
        policy.WithOrigins("https://test.website.com").WithHeaders("Accept", "sdf", "").WithMethods("GET", "POST");//Eger ozelliikle header, method vs de vermek istersek, bu sekilde verebilriz
    });

    options.AddPolicy("OnlyGoogleApplications", policy =>
    {
        policy.WithOrigins("https://google.com"," http://gmail.com", "http://drive.google.com" );
    });

    options.AddPolicy("AllowOnlyMicrosoft", policy =>
    {
        policy.WithOrigins("http://outlook.com", "http://microsoft.com", "http://onedrive.com");
    });
});
//Iste bu sekilde 4 farkli name policy olusturduk...ve asagidaki middleware kisminda AllowAll cors policy yi sececegiz(app.UseHttpsRedirection() app.UseCors("AllowAll");app.UseAuthorization(); )...Ama tabi madem ki boyle yapacagiz yani 1 tanesini alip da tamaminda gecerli yapacaksak yani daha dogrusu cogunda gecerli yapacaksak o zamn sunu yapariz iste herseye izin verdigmzi giip Default olarak yapariz..ki bu hepsinde gecerli olur,default olarak yapinca da app.UseCors(); bu sekilde yapariz sadece named i implement ederken UserCors parametresi icerisine cors ismini gireriz.....digerlerini ise named yaparak controller a ozel olarak en ustte Controller class larinda attributte vererek o controller in verilen cors poolicy uygulamasini saglariz
//o tum controller da gecerli olacaktir ama yukardaki geri kalanlaridanda istedgimzi Controller in en ustunde belirterek o Controllerde onun gecerli olmasihi saglayabilirz

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NorthwindContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("AppDBConnection");
    Console.WriteLine($"ConnectionString; {cs}");
    options.UseSqlServer(cs);
});
/*
     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True;Trust Server Certificate=True");
 
 */

builder.Services.AddAuthentication()

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapGet("api/testingendpoint",
    //    context => context.Response.WriteAsync("Test Response"))
    //    .RequireCors("AllowOnlyLocalhost");

    //endpoints.MapControllers()
    //        .RequireCors("AllowOnlyMicrosoft");
});
app.Run();
