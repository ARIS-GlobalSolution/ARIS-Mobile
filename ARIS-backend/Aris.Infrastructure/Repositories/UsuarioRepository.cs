using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aris.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    private readonly ArisDbContext _context;

    public UsuarioRepository(ArisDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
}
