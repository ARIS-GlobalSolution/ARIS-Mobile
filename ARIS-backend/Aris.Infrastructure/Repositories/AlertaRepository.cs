using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;

namespace Aris.Infrastructure.Repositories;

public class AlertaRepository(ArisDbContext context) : Repository<Alerta>(context), IAlertaRepository
{
}
