using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Satellites
{
    public class Edit
    {
        public class Command : IRequest<Result<ShipResponse>>
        {
            public List<SatelliteDto> Satellites { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.Satellites).SetValidator(new SatelliteValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<ShipResponse>>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Result<ShipResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                List<string> messageItems = new List<string>();

                foreach (var satellite in request.Satellites)
                {
                    var satelliteFromDb = await context.Satellites.Where(x => x.Name == satellite.Name).FirstOrDefaultAsync();

                    if (satelliteFromDb == null) return Result<ShipResponse>.Failure(string.Format("Failed to find a satellite named '{0}'", satellite.Name));

                    satelliteFromDb.Distance = satellite.Distance;

                    if (satellite.Message != null && satellite.Message.Count > 0)
                    {
                        satelliteFromDb.Message = satellite.Message;

                        if (messageItems.Count == 0)
                        {
                            for (int c = 0; c < satellite.Message.Count; c++)
                            {
                                messageItems.Add("");
                            }
                        }

                        for (int c = 0; c < satellite.Message.Count; c++)
                        {
                            if (satellite.Message[c] != string.Empty)
                            {
                                var itemFound = messageItems.IndexOf(satellite.Message[c]);

                                if (itemFound == -1)
                                    messageItems[c] = satellite.Message[c];
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();

                var satellites = await context.Satellites.ToListAsync();

                var shipResponse = new ShipResponse();

                double[] position = Trilateration.Compute(new Point(satellites[0].Y, satellites[0].X, satellites[0].Distance),
                                                          new Point(satellites[1].Y, satellites[1].X, satellites[1].Distance),
                                                          new Point(satellites[2].Y, satellites[2].X, satellites[2].Distance));

                messageItems.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                if (position == null || messageItems.Count == 0)
                    return Result<ShipResponse>.Success(null);

                shipResponse.Position = new Position { Y = Math.Round(position[0], 2), X = Math.Round(position[1], 2) };
                shipResponse.Message = string.Join(" ", messageItems);

                return Result<ShipResponse>.Success(shipResponse);
            }
        }
    }
}