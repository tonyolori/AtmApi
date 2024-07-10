using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Users.Commands
{
    public class UpdateUserCommand(User user):IRequest
    {
        public User User { get; set; } = user;
    }

    public class UpdateUserCommandHandler(IDataContext context) : IRequestHandler<UpdateUserCommand>
    {
        private readonly IDataContext _context = context;
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _context.Users.Update(request.User);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }

    }

}
