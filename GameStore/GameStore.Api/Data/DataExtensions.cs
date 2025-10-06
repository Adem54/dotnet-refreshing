using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data
{
    public static class DataExtensions
    {
        public static void MigrateDb(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
            dbContext.Database.Migrate();
        }
        //What we want to do here is to go ahead and migrate the database
        //Scoped life time olmasi gerekiyor ayni zamanda
        //Biz dependencyinjectin dan bahsediyoruz..Biz su and DbContext e direk olarak erisemiyoruz
        //We need to provision a scope that allows us to actually start interacting with the database
        //First we are going to be getting an instance of a scope to do that let's do this using bar scope
        //That gives us a scope and let's see what type IServices scope, we can use to request the service container of asp.net
        //  core to give us an instance of some of the services that have been registered in the application

        //Program.cs e gidersek   builder.Services.AddSqlite<GameStoreContext>(connString); bunu goruruz, thisline is going to register 
        // our GameStoreContext in the servicecontainer, right meaning that asp.net core knows about the type and is ready to provide use 
        // an instance whenever we request one(what do you mean..you mean when our api got a new request...??) 
        // And if you go back to into the extension what we can do is the following,we can now say..
    }
}