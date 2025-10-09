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
public static class GenresEndpoints
{
    const string GetGameEndpointName = "GetGenre";
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres");
        //Get /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
         await dbContext.Genres
                   .Select(genre => genre.ToDto())
                   .AsNoTracking()
                   .ToListAsync());

        //Get /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);
          
            //it is very important to return always same type IResult
            return genre is null
            ? Results.NotFound("id is not found")
            : Results.Ok(genre.ToDto());
        })
            .WithName(GetGameEndpointName)
            .Produces<GameSummaryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

             //Post /games
         group.MapPost("/", async (GenreDto newGenre, GameStoreContext dbContext) =>
        {
            Genre genre = newGenre.ToEntity();
            dbContext.Genres.Add(genre);//keep track of changins here entityframework
            await dbContext.SaveChangesAsync();//But here entiytframwoerk transforms all of the changings...executing of sql statement...new game insert sql is executing

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = genre.Id }, genre.ToDto());
            //We should never return internal entities, we should always return dtos
        });


        //put isleminde id yine kullanicidan endpoint icinde gelecek, ama endpoint url icinde gelecek..ama updatedData ise payload-request body ile gelecek bunlari karistirmayalim..
        //PUT /games
        //replace the object completely that's the purpose of put..
        group.MapPut("/{id}", async (int id, GenreDto updateGenre, GameStoreContext dbContext) =>
        {
            //What happend if we don't find the game
            Genre? existingGenre = await dbContext.Genres.FindAsync(id);//Dikkat burda cok hizli bir sekilde dbcontext icinden entity buluruz
                                                         
            if (existingGenre is null)
            {
                return Results.NotFound();
            }
            //locate existing entry inside our dbContext, and replace it with a brandnew entity
            dbContext.Entry(existingGenre)
                      .CurrentValues
                      .SetValues(updateGenre.ToEntity(id));
          
            await dbContext.SaveChangesAsync();//update sql is executing
            //return Results.Ok(existingGame.ToGameDetailsDto());
            return Results.NoContent();

        });
       

        //DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
           await dbContext.Genres
                      .Where(genre => genre.Id == id)
                      .ExecuteDeleteAsync();
      
            return Results.NoContent();
        });

        return group;
    }
}
