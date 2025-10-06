using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping
{
    public static class GameMapping//static class yapariz
    {
        //CreateGameDto olarak client tarafindan gonderilecek ve biz bunu alip Game Entity sine donusturecegiz...
        public static Game ToEntity(this CreateGameDto game)
        {
            return new Game()
            {
                Name = game.Name,
                //Genre = dbContext.Genres.Find(newGame.GenreId)!,//Burasi yine endpoint icinde hallolacak biz burda sadece CreateGameDto dan gelenleri Game de karsiklarin yazariz
                GenreId = game.GenreId,
                Price = game.Price,
                ReleaseData = game.ReleaseDate
            };
        }

        public static GameSummaryDto ToGameSummaryDto(this Game game)
        {
            return new GameSummaryDto(
               game.Id,
               game.Name,
               game.Genre!.Name,//demek Genre is never going to be null..ben garanti ediyorum sana demek...
               game.Price,
               game.ReleaseData
           );
        }
        
        public static GameDetailsDto ToGameDetailsDto(this Game game)
        {
             return  new(
                game.Id,
                game.Name,
                game.GenreId,//demek Genre is never going to be null..ben garanti ediyorum sana demek...
                game.Price,
                game.ReleaseData
            );
        }
    }
}