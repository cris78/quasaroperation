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
    /// <summary>
    /// CQRS Handler for Command - Update, calculate ship position and decode message
    /// </summary>
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

                //Iterate for each satellite in order to update data and process the message to decode
                foreach (var satellite in request.Satellites)
                {
                    var satelliteFromDb = await context.Satellites.Where(x => x.Name == satellite.Name).FirstOrDefaultAsync(cancellationToken: cancellationToken);

                    if (satelliteFromDb == null) return Result<ShipResponse>.Failure(string.Format("Failed to find a satellite named '{0}'", satellite.Name));

                    satelliteFromDb.Distance = satellite.Distance;
                    satelliteFromDb.LastUpdatedDate = DateTime.Now;

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

                messageItems.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                //Save changes
                await context.SaveChangesAsync(cancellationToken);

                var satellites = await context.Satellites.ToListAsync(cancellationToken: cancellationToken);

                //Process the response
                var shipResponse = new ShipResponse();

                //Calculate Ship Position by Trilateration
                double[] position = Trilateration.Compute(new Point(satellites[0].Y, satellites[0].X, satellites[0].Distance),
                                                          new Point(satellites[1].Y, satellites[1].X, satellites[1].Distance),
                                                          new Point(satellites[2].Y, satellites[2].X, satellites[2].Distance));

                if (position == null || messageItems.Count == 0)
                    return Result<ShipResponse>.Success(null);

                shipResponse.Position = new Position { Y = Math.Round(position[0], 2), X = Math.Round(position[1], 2) };
                shipResponse.Message = string.Join(" ", messageItems);

                return Result<ShipResponse>.Success(shipResponse);
            }
        }
    }
}