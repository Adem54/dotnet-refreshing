using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Dtos
{
    public record class GameDetailsDto(
        int Id,
        string Name,
        int GenreId,
        decimal Price,
        DateOnly ReleaseDate);
        //Burda dikkat edersek Genre objesi include edilmemis, sadece GenreId si bulunyor
}