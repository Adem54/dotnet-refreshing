using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Dtos
{
    //updateGamedto, get, update hepsi icin ayri Dto olusturmamiz gerekiyor ki daha fleksibel, haraket edebilelim...biribrine bagli olmasinlar ilerde her birisinin ayri degisikliklere ihityaci olacaktir
    public record class UpdateGameDto(
        [Required][StringLength(50)]string Name,
       // [Required][StringLength(20)]string Genre,
        int GenreId,
        [Range(1,100)]decimal Price,
        DateOnly ReleaseDate
    );
}