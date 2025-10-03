using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace GameStore.Api.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
    {
        public DbSet<Game> Games => Set<Game>();//Bu getter-only bir özelliktir (property).
                                                //=> Set<Game>() kısmındaki Set<T>() bir metot adıdır ve DbSet<Game> döndürür.“Set” burada fiil değil, EF Core’un DbContext içindeki metodunun adıdır.
                                                //Yani bu property’nin getteri çalıştığında, DbContext.Set<Game>() çağrılır ve EF’in tuttuğu DbSet<Game> referansı geri verilir.
                                                //EF Core bu DbSet’i context başına önbelleğe alır; her çağrıda yeni nesne üretmek yerine aynı referansı verir.
        public DbSet<Genre> Genres => Set<Genre>();
    }
}