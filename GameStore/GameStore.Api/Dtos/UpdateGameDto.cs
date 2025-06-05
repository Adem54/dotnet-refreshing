using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Dtos
{
    //Create, get, update hepsi icin ayri Dto olusturmamiz gerekiyor ki daha fleksibel, haraket edebilelim...biribrine bagli olmasinlar ilerde her birisinin ayri degisikliklere ihityaci olacaktir
    public record class UpdateGameDto(
        string Name,
        string Genre,
        decimal Price,
        DateOnly ReleaseDate
    );
     
       
}