using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Satellites
{
    public class List
    {
        public class Query : IRequest<List<Satellite>> { }

        public class Handler : IRequestHandler<Query, List<Satellite>>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<List<Satellite>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.Satellites.ToListAsync();
            }
        }
    }
}