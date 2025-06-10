using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Dtos;
using MiniValidation;

namespace GameStore.Api.Endpoints;

//This is going to be extension methods 
//class has to be static
public static class GamesEndpoints
{
    //❌ You cannot have instance fields in a static class.
    //A static class can only contain static members because it cannot be instantiated.
    //also readonly since we don't intent to assign new list data in the future, even if we will add-delete-modify in list..we don't change the reference address..
    private static readonly List<GameDto> games = new List<GameDto>
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
            GameDto? game = games.Find(g => g.Id == id);
            //it is very important to return always same type IResult
            return game is null
            ? Results.NotFound("id is not found")
            : Results.Ok(game);
        })
            .WithName(GetGameEndpointName)
            .Produces<GameDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        //Bu sekilde bu endpointe isim verebiliyrouz...yani asinda ayni ecpontin methodunun ismi gibi...


        //Post /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            games.Add(game);
            //return status also, data.., message 
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });
    

        //put isleminde id yine kullanicidan endpoint icinde gelecek, ama endpoint url icinde gelecek..ama updatedData ise payload-request body ile gelecek bunlari karistirmayalim..
        //PUT /games
        //replace the object completely that's the purpose of put
        group.MapPut("/{id}", (int id, UpdateGameDto updateGame) =>
        {
            //What happend if we don't find the game
            int index = games.FindIndex(g => g.Id == id);
            if (index == -1)
            {
                return Results.NotFound($"id:{id} is not found");
            }
            else
            {
                GameDto game = new(id, updateGame.Name, updateGame.Genre, updateGame.Price, updateGame.ReleaseDate);
                games[index] = game;
                return Results.Ok();
            }

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