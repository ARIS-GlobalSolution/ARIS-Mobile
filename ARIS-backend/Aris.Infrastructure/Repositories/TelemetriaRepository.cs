using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;

namespace Aris.Infrastructure.Repositories;

public class TelemetriaRepository(ArisDbContext context) : Repository<Telemetria>(context), ITelemetriaRepository
{
}
