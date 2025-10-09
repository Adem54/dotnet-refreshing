using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping
{
    public static class GenreMapping//static class yapariz
    {
        //CreateGameDto olarak client tarafindan gonderilecek ve biz bunu alip Game Entity sine donusturecegiz...
        //!!!Bunu benim duzeltmem gerekiyor bu mantigimi!!!!!!!
        //Isimlendirmde benim boyla hatalarim var bu mantik dogru degil..yani gidip de ToGenreDto demek yerine zaten Genre icin ayri bir klasordde GenreMapping icinde yapyorsun o zaman buna ToDto demen yeterlidir karismayacakki GameMapping.cs deki ToDto ile....
        public static GenreDto ToDto(this Genre genre)
        {
            return new GenreDto(
               genre.Id,
               genre.Name
           );
        }

        public static Genre ToEntity(this GenreDto genre)
        {
            return new Genre()
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
        
        public static Genre ToEntity(this GenreDto genre, int id)
        {
            return new Genre()
            {
                Id = id,
                Name = genre.Name
            };
        }


    }
}