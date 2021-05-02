using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Satellites.Any()) return;

            var satellites = new List<Satellite>
            {
                new Satellite {

                    Id = Guid.NewGuid(),
                    Name = "Kenobi",
                    Y = -200,
                    X = -500,
                    LastUpdatedDate = DateTime.Now
                },
                new Satellite{

                    Id = Guid.NewGuid(),
                    Name = "Skywalker",
                    Y = -100,
                    X = 100,
                    LastUpdatedDate = DateTime.Now
                },
                new Satellite{

                    Id = Guid.NewGuid(),
                    Name = "Sato",
                    Y = 100,
                    X = 500,
                    LastUpdatedDate = DateTime.Now
                }
            };

            await context.Satellites.AddRangeAsync(satellites);
            await context.SaveChangesAsync();
        }
    }
}