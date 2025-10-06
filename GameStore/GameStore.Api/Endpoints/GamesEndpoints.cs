using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using MiniValidation;

namespace GameStore.Api.Endpoints;

//This is going to be extension methods 
//class has to be static
public static class GamesEndpoints
{
    //❌ You cannot have instance fields in a static class.
    //A static class can only contain static members because it cannot be instantiated.
    //also readonly since we don't intent to assign new list data in the future, even if we will add-delete-modify in list..we don't change the reference address..
    private static readonly List<GameSummaryDto> games = new List<GameSummaryDto>
    {
        new(1, "The Legend of Eldoria", "RPG", 59.99m, new DateOnly(2022, 10, 15)),
        new(2, "Skyborne Rally", "Racing", 39.99m, new DateOnly(2023, 3, 8)),
        new(3, "Cyber Siege", "Shooter", 49.99m, new DateOnly(2021, 12, 5)),
        new (4, "Planet Forge", "Strategy", 29.99m, new DateOnly(2020, 7, 22)),
        new (5, "Shadow Tactics X", "Stealth", 44.99m, new DateOnly(2023, 6, 1)),
        new (6, "Pixel Farm Tycoon", "Simulation", 19.99m, new DateOnly(2024, 2, 14))
    };
    
    const string GetGameEndpointName = "GetGame";


    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                                .WithParameterValidation();
        //Get /games
        group.MapGet("/", () => games);

        //Get /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameSummaryDto? game = games.Find(g => g.Id == id);
            //it is very important to return always same type IResult
            return game is null
            ? Results.NotFound("id is not found")
            : Results.Ok(game);
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
         group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            //  game.Genre = dbContext.Genres.Find(newGame.GenreId)!;//GameDetailsDto ya bakacak edersek Genre objesi include edilmemis, sadece GenreId si bulunyor, o zaman bizim Genre objesini bulmaya calismamiza gerek yok...
            //We don't need to generate Genre object for the game entity, we were really only doing that because we needed that later on for the
            //mapping , but really is not needed anymore...

            dbContext.Games.Add(game);//keep track of changins here entityframework
            dbContext.SaveChanges();//But here entiytframwoerk transforms all of the changings...executing of sql statement...new game insert sql is executing

            //return status also, data.., message
            // GameSummaryDto gameDto = game.ToGameSummaryDto(); kullaniciya string genre donmek yerine(ToGameSummaryDto string Genre donuyor), GenreId donmek daha dogru bir mantiktir...unutmayalim....ondan dolayi biz de ToGameDetailsDto diye doneriz...
           //CreateGameDto aliyoruz..ama GameDetailsDto return ediyoruz
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
            //We should never return internal entities, we should always return dtos
        });
    

        //put isleminde id yine kullanicidan endpoint icinde gelecek, ama endpoint url icinde gelecek..ama updatedData ise payload-request body ile gelecek bunlari karistirmayalim..
        //PUT /games
        //replace the object completely that's the purpose of put
        group.MapPut("/{id}", (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            //What happend if we don't find the game
            Game? game = dbContext.Games.Find(id);//Dikkat burda cok hizli bir sekilde dbcontext icinden entity buluruz
                                                  //Find method Game i current setupgames halihazirdan zaten alinmis, daha cekilmis in-memory icerisinden bulmaya calisir, DbContext araciligi ile   su anda, ama su anki current DbSet in-memory icerisinden bulamazsam, o zaman database e gider ve onu bulur
                                                  //1-! vererek ok, ne yaptigimin farkindayim...bunun null gelmeyecegini garanti ediyorum sakin ol diyebiliriz
                                                  //2-Ya da Game? yaparak, bu Game null da olabilir bunun farkindayim diyorsun o da tamam sen olayin farkinda isen o zamn bende sana sknti cikarmiyorum der

            game.Name = updateGame.Name;
            game.Price = updateGame.Price;
            game.ReleaseData = updateGame.ReleaseDate;
            game.GenreId = dbContext.Genres.Find(updateGame.GenreId);
            if (game is null)
            {
                return Results.NotFound($"id:{id} is not found");
            }
            else
            {

                GameDto gameDto = game.ToDto();
                games[index] = game;
                return Results.Ok();
            }
            //Simdi dikkat edelim biz bir onceki MapPost ta biz GameDto return etmistik, ve burda Genre string idi, ancak biz GetById methodunda string olarak donmek istemeyiz...Cunku client muhtemelen genreId yi almak isteycek, UI da kullanabilmek icin,..User genre yi, dropdown listten sececektir...ve id sini kullanacak tabki ki...Orda seiclen genreId, bizim tarafimizdan retunr edilen genre id ilematch olmalidir..Genre yi string vermek isi zolastirir...Biz simdi gideriz ve Dtos altina GameDetailsDto.cs olusturuuz ve GameDetailsDto{int Id,string Name, decimal Price, DateOnly ReleaseDate}

        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            GameDto? game = games.Find(g => g.Id == id);
            if (game is not null)
            {
                games.RemoveAll(g => g.Id == id);
                return Results.Ok("data is deleted");
            }
            else
            {
                return Results.NotFound($"id: {id} is not found!");
            }
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

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