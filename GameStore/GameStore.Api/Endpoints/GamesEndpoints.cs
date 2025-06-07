using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

//This is going to be extension methods 
//class has to be static
public static class GamesEndpoints
{
    private List<GameDto> games = new List<GameDto>
    {
        new(1, "The Legend of Eldoria", "RPG", 59.99m, new DateOnly(2022, 10, 15)),
        new(2, "Skyborne Rally", "Racing", 39.99m, new DateOnly(2023, 3, 8)),
        new(3, "Cyber Siege", "Shooter", 49.99m, new DateOnly(2021, 12, 5)),
        new (4, "Planet Forge", "Strategy", 29.99m, new DateOnly(2020, 7, 22)),
        new (5, "Shadow Tactics X", "Stealth", 44.99m, new DateOnly(2023, 6, 1)),
        new (6, "Pixel Farm Tycoon", "Simulation", 19.99m, new DateOnly(2024, 2, 14))
    };
}