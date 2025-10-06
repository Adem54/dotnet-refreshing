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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genre>().HasData(
                new {Id=1, Name="Fighting"},
                new {Id=2, Name="Roleplaying"},
                new {Id=3, Name="Sports"},
                new {Id=4, Name="Racing"},
                new {Id=5, Name="Kids and Family"}
            );
        }
        //This method is executed as soon as migratino executed
        //This is opportunity for us to do things that slightly modify the model accroding to our needs
        //One of the other things that we can do here is to populate some versy staic data i am not going to do this:base.OnModelCreating(modelBuilder); for data requires much more complex operation
        //This is something just for some things that are very very simple, this is just list of categories..we are not adding categories...and we just want to add via here.. 
        //What you have to do is just say mode lbuilder.entity<Genre>().HasData type of enity genre...this(HasData) is going to make sure that whatever data we introduce here has to exist when the migration process completes
    }
}