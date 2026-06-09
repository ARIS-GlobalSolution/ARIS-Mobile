using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;

namespace Aris.Infrastructure.Repositories;

public class SensorRepository(ArisDbContext context) : Repository<Sensor>(context), ISensorRepository
{
}
