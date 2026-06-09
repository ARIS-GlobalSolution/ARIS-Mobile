using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;

namespace Aris.Infrastructure.Repositories;

public class TipoSensorRepository(ArisDbContext context) : Repository<TipoSensor>(context), ITipoSensorRepository
{
}
