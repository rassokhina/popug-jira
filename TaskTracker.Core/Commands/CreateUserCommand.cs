using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskTracker.Core.Data;
using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Commands
{
    public class CreateUserCommand : IRequest<User>
    {
        public string PublicId { get; set; }

        public string Role { get; set; }

        public string Username { get; set; }

        public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
        {
            private readonly DefaultContext defaultContext;

            public CreateUserCommandHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
            {
                var user = await defaultContext.Users
                    .FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken)
                    .ConfigureAwait(false);
                if (user != null)
                {
                    return user;
                }
                user = new Entities.User
                {
                    Id = Guid.NewGuid(),
                    PublicId = command.PublicId,
                    Username = command.Username,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Role = command.Role
                };
                await defaultContext.AddAsync(user, cancellationToken);
                await defaultContext.SaveChangesAsync(cancellationToken);

                return user;
            }
        }
    }
}
