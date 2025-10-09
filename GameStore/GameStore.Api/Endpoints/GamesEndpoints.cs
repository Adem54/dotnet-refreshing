using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

namespace GameStore.Api.Endpoints;

//This is going to be extension methods 
//class has to be static
public static class GamesEndpoints
{
    // You cannot have instance fields in a static class.
    //A static class can only contain static members because it cannot be instantiated.
    //also readonly since we don't intent to assign new list data in the future, even if we will add-delete-modify in list..we don't change the reference address..
    // private static readonly List<GameSummaryDto> games = new List<GameSummaryDto>
    // {
    //     new(1, "The Legend of Eldoria", "RPG", 59.99m, new DateOnly(2022, 10, 15)),
    //     new(2, "Skyborne Rally", "Racing", 39.99m, new DateOnly(2023, 3, 8)),
    //     new(3, "Cyber Siege", "Shooter", 49.99m, new DateOnly(2021, 12, 5)),
    //     new (4, "Planet Forge", "Strategy", 29.99m, new DateOnly(2020, 7, 22)),
    //     new (5, "Shadow Tactics X", "Stealth", 44.99m, new DateOnly(2023, 6, 1)),
    //     new (6, "Pixel Farm Tycoon", "Simulation", 19.99m, new DateOnly(2024, 2, 14))
    // };
    
    const string GetGameEndpointName = "GetGame";

/*
System.NullReferenceException: Object reference not set to an instance of an object.
   at GameStore.Api.Mapping.GameMapping.ToGameSummaryDto(Game game) in C:\Users\Hp35\Desktop\Projects\Dotnet\dotnet-refreshing\GameStore\GameStore.Api\Mapping\GameMapping.cs:line 27
   at GameStore.Api.Endpoints.GamesEndpoints.<>c.<MapGamesEndpoints>b__2_1(Int32 id, GameStoreContext dbContext) in C:\Users\Hp35\Desktop\Projects\Dotnet\dotnet-refreshing\GameStore\GameStore.Api\Endpoints\GamesEndpoints.cs:line 46
   at lambda_method14(Closure, Object, HttpContext)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

ToGameSummaryDto(Game game) içinde NullReferenceException.
%99 sebep: game.Genre null ama ToGameSummaryDto içinde game.Genre.Name gibi bir şeye dokunuyorsun.
*/
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                                .WithParameterValidation();
        //Get /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
         await dbContext.Games
                   .Include(game => game.Genre)//Dikkat Game icnde
                   .Select(game => game.ToGameSummaryDto())
                   .AsNoTracking()
                   .ToListAsync());//Burdaki taski yani, bu games datasini getirme isini bitirmesini istiyor, client a return donmeden once..
                                   //Diyorki sen bekle..bu task bitsin ondna sonra return yap clienta diyor...

        //Dikkat edelim donerken return olarak dtos donmemiz gerekiyr ve SummaryDtos donecegiz...
        //Ama bir eksigimiz var,   game.Genre!.Name, biz bu game icinde Game entitysi public int GenreId, ve Genre obj yi tutuyor, zaman biz burda GEnre objesini include ile attach edecegiz...onemli..
        //Iste bu skilde include mehtodu ile Genre objelerini dahil ediyoruz, etmezsek include ile dahil etmezse, Genre objesi null olarak gelir..
        //Entityframework keep track of every single entity that we're pulling out of here is goint to do change tracking for every entity
        //If you have many, that's really wasted resources right
        //We don't need to keep track of anything here because we are going to return that back into the client right away just one operation. So we're not going to be performing into the return entities...
        //Becasuse of that one of very good optimization i should always do in this cases is just to say AsNoTracking like that..By doing that you are saying hey at the framework I don't need to do any tracking of the return entities just send them back into the client in this case as is right so that's going to improve the performance of your operation


        //Sadece DTO projeksiyonu yapıyorsan → çoğu zaman Include GEREKSİZ
        //g.Genre.Name yazdığında EF Core JOIN’i kendisi kurar ve tek sorguda Genre.Name’i getirir.
        //Entity materyalize etmediğin (Game/Genre nesnesi dönmediğin) sürece Include’a ihtiyaç yok.
        //Bu, performans olarak da daha iyi: sadece ihtiyacın olan kolonları çekersin (projection). 
        //Senin örneğinde Select(game => game.ToGameSummaryDto()) zaten bir projection. ToGameSummaryDto içinde Genre.Name kullanıyorsan EF bunu JOIN’e çevirir; Include eklemene gerek yok.
       
        //Bunu ben kendim test icin yaptim..
        group.MapGet("/games", async (GameStoreContext dbContext) =>
        {
            return await dbContext.Games.Select(g => new GameSummaryDto(
                g.Id,
                g.Name,
                g.Genre.Name,
                g.Price,
                g.ReleaseData
             )).AsNoTracking().ToListAsync();
        });

        //Tam entity (navigasyonlar dolu şekilde) istiyorsan → Include GEREK
        //Entity’leri kendileriyle (navigasyonlarıyla birlikte) döndüreceksen:
        //var game = await dbContext.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);     // ✅ navigasyon entity yüklenir 
        //Burada game.Genre nesnesine entity olarak erişmek istiyorsun → Include gerekli.
        //Koleksiyonlar için: .Include(g => g.Tags).ThenInclude(t => t.Something).
        //Not: Minimal API’de entity döndürmektense her zaman DTO döndür (güvenlik/şişkinlik). DTO döneceksen yine projection daha iyi; Include’sız çalış.

        //Get /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);
          
            //it is very important to return always same type IResult
            return game is null
            ? Results.NotFound("id is not found")
            : Results.Ok(game.ToGameDetailsDto());
        })
            .WithName(GetGameEndpointName)
            .Produces<GameSummaryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        //Bu sekilde bu endpointe isim verebiliyrouz...yani asinda ayni ecpontin methodunun ismi gibi...

        //So remember only thingg that we are receiving right now, in the handler this method is the CreateGameDto,
        //So we are going to go ahead and inject the instance of gain store context that has been registered 
        //in the service provider so for that all we have to do,is just add GameStoreContext as parameter,
        //At run time, asp.net core is going to take care of resolving and providing us an instance of that context right here without us having to do anyghing else
        //Post /games
         group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            //  game.Genre = dbContext.Genres.Find(newGame.GenreId)!;//GameDetailsDto ya bakacak edersek Genre objesi include edilmemis, sadece GenreId si bulunyor, o zaman bizim Genre objesini bulmaya calismamiza gerek yok...
            //We don't need to generate Genre object for the game entity, we were really only doing that because we needed that later on for the
            //mapping , but really is not needed anymore...

            dbContext.Games.Add(game);//keep track of changins here entityframework
            await dbContext.SaveChangesAsync();//But here entiytframwoerk transforms all of the changings...executing of sql statement...new game insert sql is executing

            //return status also, data.., message
            // GameSummaryDto gameDto = game.ToGameSummaryDto(); kullaniciya string genre donmek yerine(ToGameSummaryDto string Genre donuyor), GenreId donmek daha dogru bir mantiktir...unutmayalim....ondan dolayi biz de ToGameDetailsDto diye doneriz...
           //CreateGameDto aliyoruz..ama GameDetailsDto return ediyoruz
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
            //We should never return internal entities, we should always return dtos
        });


        //put isleminde id yine kullanicidan endpoint icinde gelecek, ama endpoint url icinde gelecek..ama updatedData ise payload-request body ile gelecek bunlari karistirmayalim..
        //PUT /games
        //replace the object completely that's the purpose of put..
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            //What happend if we don't find the game
            Game? existingGame = await dbContext.Games.FindAsync(id);//Dikkat burda cok hizli bir sekilde dbcontext icinden entity buluruz
                                                          //Find method Game i current setupgames halihazirdan zaten alinmis, daha cekilmis in-memory icerisinden bulmaya calisir, DbContext araciligi ile   su anda, ama su anki current DbSet in-memory icerisinden bulamazsam, o zaman database e gider ve onu bulur
                                                          //1-! vererek ok, ne yaptigimin farkindayim...bunun null gelmeyecegini garanti ediyorum sakin ol diyebiliriz
                                                          //2-Ya da Game? yaparak, bu Game null da olabilir bunun farkindayim diyorsun o da tamam sen olayin farkinda isen o zamn bende sana sknti cikarmiyorum der

            //existingGame zaten Find/FirstOrDefault ile yüklenmiş ve ChangeTracker tarafından tracked durumda.
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            //locate existing entry inside our dbContext, and replace it with a brandnew entity
            dbContext.Entry(existingGame)
                      .CurrentValues
                      .SetValues(updateGame.ToEntity(id));
            //dbContext.Entry methodunu biz locate the current entity inside that DbContext, and inside that we can say current values right pulls out the current values of that entity and then right there we're going to say setvalues, we can specify the new entity that's going to replace all of the values of the old entity
            //dbContext.Entry(existingGame) sana bu nesnenin “gözlemci”sini verir (durum, property değerleri, vs).
            //Entry.CurrentValues = PropertyValues nesnesi-existingGame üzerindeki tüm scalar (primitive) alanların mevcut değerleri burada.
            //SetValues(newEntity)-newEntity (senin updateGame.ToEntity(id)) ile existingGame’in eşleşen property adları bulunur.
            //Scalar property’ler için existingGame’in değerleri newEntity’den gelenlerle değiştirilir-Değeri değişen property’ler Modified işaretlenir (SaveChanges’ta UPDATE’e dönüşür)
            //Durum (State) nasıl işaretlenir?
            //SetValues sonrası, değişen her property için Entry.Property(p).IsModified = true olur.
            //SaveChanges() çağrısında EF Core sadece değişen alanları UPDATE eder (column bazlı).
            //UpdateGameDto → ToEntity(id) ile tam bir fotoğraf üretiyorsun.
            //SetValues “body’de ne varsa eski entity’nin üstüne yaz” semantiğine uyuyor (PUT).
            // Navigasyonları yönetmek istersen (çoğu zaman sadece FK yeter):(GEnre obj)
            // existing.GenreId = dto.GenreId; // zaten SetValues yaptıysa gerek yok
            await dbContext.SaveChangesAsync();//update sql is executing
                                    //Tracked değişiklikleri (Insert/Update/Delete) tespit eder — örn. Entry.State = Modified, SetValues(...), Add(...), Remove(...).
                                    //Bunları SQL komutlarına çevirir (UPDATE/INSERT/DELETE).Hepsini tek bir transaction içinde sırayla çalıştırır.
                                    //Etkilenen satır sayısını int olarak döner.
                                    //Değişiklik yoksa SQL göndermez (0 döner).
            
            //return Results.Ok(existingGame.ToGameDetailsDto());
            return Results.NoContent();

        });
        // .Produces(StatusCodes.Status200OK)
        // .Produces(StatusCodes.Status404NotFound);

        //DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
           await dbContext.Games
                      .Where(game => game.Id == id)
                      .ExecuteDeleteAsync();
            //known as batch delete, it is very efficient opertion, becuase we don't have to first find entity, just to say hey entityframework 
            //keep track of that i am going to delete it, and go ahead and save the deletion 
            //This line is just one shot is going to go straight in to the database find the entiyt and delete it right away, there is no need to do anything else here..and then we just return no content
            return Results.NoContent();
        });

        return group;
    }
}
/*
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Burda app WebApplicationi type indadir...dolayis iile biz app. diyerke kullanabilecegiz olusturacak oldugmz extension methodi

*/

/*
 private  List<GameDto> games = new List<GameDto>
 Bunu yaparken makelist readonly uyarisi aliyoruz..cunku 
 biz bu listeyi dikkat edelim tekrardan yeni bir atama yapmayi planlamiyoruz..tabi ki add, delete, update yapacagiz ama listenin refereansi ayni kalacak...unutma buraya dikkat et..yeni bir liste atamasi yapmayi dusunmuyoruz...o zaman readonly yapmak mantikli boyle durumlar icin  
*/