using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int GenreId { get; set; }
        public decimal Price { get; set; }
        public Genre Genre { get; set; } = null!; // “Ben garanti ediyorum ki null olmayacak”

        //this property may or may not be null depending on if we decide to populate generate when we read data from the database via at framework..sometimes it may be just enough to have the genreId so that will be populated but not the genre
        //So it is not clear at this point if we are going to have for sure a value all the time so we can totally go ahead and just declare this as a null, just like that..
        //So this combination of two properties is what you do on Entity framework, when you want to do an Assosiation in this case one to one relationship between game and genre
        public DateOnly ReleaseData { get; set; }
    }
}