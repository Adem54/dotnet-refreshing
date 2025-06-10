using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Dtos
{
    //Client yeni olusturulaarak database kaydedilecek bir data nin id si client dan gelmez tabi ki..bu api tarafinda olusturlur..
    //Autoincrement yaklamisi ile db ye kaydedeceksek direk otomatik olarak db de olusturulabilir ya da biz uniq id yi api tarfinda-server tarafinda olusturup da db ye de kaydedebiliriz 
    public record class CreateGameDto(
        [Required][StringLength(50)]string Name,
        [Required(ErrorMessage = "Genre is required!")][StringLength(20)]string Genre,
        [Range(1,100)]decimal Price,//1-100 dollar arasinda olsun
        DateOnly ReleaseDate
    );
    //we onlycare about the date part not time..for this reason we chose DateOnly
}