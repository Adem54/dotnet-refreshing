using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Dtos
{
    public record class GameDto(
        int Id,
        string Name,
        string Genre,
        decimal Price,
        DateOnly ReleaseDate);
        //we onlycare about the date part not time..for this reason we chose DateOnly
}