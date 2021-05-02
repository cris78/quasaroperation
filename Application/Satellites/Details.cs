using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Satellites
{
    /// <summary>
    /// CQRS Handler for Queries - Get Satellite info by Name
    /// </summary>
    public class Details
    {
        public class Query : IRequest<Satellite>
        {
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Satellite>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Satellite> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.Satellites.FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken: cancellationToken);
            }
        }
    }
}