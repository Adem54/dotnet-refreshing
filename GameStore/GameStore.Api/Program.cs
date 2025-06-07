using GameStore.Api.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
//instance of webapplication...as host..host our application
//represent httpserver implementation for our app, and so it can start listening for http requests..
//it stands up a bunch of mware components, a login services dependency injection services, configuration services
//Bunch of services we are going to talking about across this
//And you can cofigure over here if we expand this between these two lines...var app = builder..and var builder = Web.. 
//you could go ahead and always just type builder. and depend on your needs
//We are going to work alot this builder object to introduct services as we go across this prosject...

var app = builder.Build();

const string GetGameEndpointName = "GetGame";

var games = new List<GameDto>
{
    new(1, "The Legend of Eldoria", "RPG", 59.99m, new DateOnly(2022, 10, 15)),
    new(2, "Skyborne Rally", "Racing", 39.99m, new DateOnly(2023, 3, 8)),
    new(3, "Cyber Siege", "Shooter", 49.99m, new DateOnly(2021, 12, 5)),
    new (4, "Planet Forge", "Strategy", 29.99m, new DateOnly(2020, 7, 22)),
    new (5, "Shadow Tactics X", "Stealth", 44.99m, new DateOnly(2023, 6, 1)),
    new (6, "Pixel Farm Tycoon", "Simulation", 19.99m, new DateOnly(2024, 2, 14))
};

//These types of api is minimal api!!!

//Get /games
app.MapGet("/games", () => games);

//Get /games/1
app.MapGet("/games/{id}", (int id) =>
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
app.MapPost("/games", (CreateGameDto newGame) =>
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
app.MapPut("/games/{id}", (int id, UpdateGameDto updateGame) =>
{
    //What happend if we don't find the game
    int index = games.FindIndex(g => g.Id == id);
    if (index == -1)
    {
        return Results.NotFound($"id:{id} is not found");
    } else
    {
        GameDto game = new(id, updateGame.Name, updateGame.Genre, updateGame.Price, updateGame.ReleaseDate);
        games[index] = game;
        return Results.Ok();
    }

})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

//DELETE /games/1
app.MapDelete("/games/{id}", (int id) =>
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

app.Run();

/*
🔍 What is Results.CreatedAtRoute(...)?
It’s a built-in helper method in .NET Minimal APIs that returns:
✅ HTTP status code 201 Created
✅ A Location header pointing to a route that can retrieve the newly created object (e.g., by ID)
✅ The created object itself in the response body(Payload)
What is paylod:
Payload = The actual data you send or receive in the body of an HTTP request or response.
*/
//Data Transfer Object-DTO..is an object that carries dta between process or application 
//Encapsules data in a simple and standardized format that can easily transmitted across different layers of application
//You can do bunch of things by using app. member operator..so..
/*  GameStore.Api.csproj   dosyasi nedir, proje dosyasi diye adlandirdimgiz dosya nedir ne ise yarar
This file defines some of the information
*/


