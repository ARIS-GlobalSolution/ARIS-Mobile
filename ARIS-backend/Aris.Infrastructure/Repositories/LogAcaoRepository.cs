using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;

namespace Aris.Infrastructure.Repositories;

public class LogAcaoRepository(ArisDbContext context) : Repository<LogAcao>(context), ILogAcaoRepository
{
}
