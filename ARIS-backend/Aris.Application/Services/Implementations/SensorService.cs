using Aris.Application.DTOs.Sensores;
using Aris.Application.Helpers;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Interfaces;
using Aris.Domain.Entities;

namespace Aris.Application.Services.Implementations;

public class SensorService(
    ISensorRepository sensorRepository,
    IEstufaRepository estufaRepository,
    ITipoSensorRepository tipoSensorRepository) : ISensorService
{
    public async Task<List<SensorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sensors = await sensorRepository.GetAllAsync(cancellationToken);
        return sensors.Select(Map).ToList();
    }

    public async Task<SensorResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sensor = await sensorRepository.GetByIdAsync(id, cancellationToken);
        return sensor is null ? null : Map(sensor);
    }

    public async Task<SensorResponseDto> CreateAsync(CreateSensorDto request, CancellationToken cancellationToken = default)
    {
        await EnsureDependenciesExist(request.TipoSensorId, request.EstufaId, cancellationToken);
        var sensor = new Sensor(request.TipoSensorId, request.EstufaId);
        EntityIdHelper.SetId(sensor, await EntityIdHelper.GetNextIdAsync(sensorRepository, cancellationToken));
        await sensorRepository.AddAsync(sensor, cancellationToken);
        await sensorRepository.SaveChangesAsync(cancellationToken);
        return Map(sensor);
    }

    public async Task<SensorResponseDto> UpdateAsync(int id, UpdateSensorDto request, CancellationToken cancellationToken = default)
    {
        var sensor = await sensorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Sensor not found.");

        await EnsureDependenciesExist(request.TipoSensorId, request.EstufaId, cancellationToken);
        sensor.Update(request.TipoSensorId, request.EstufaId);
        sensorRepository.Update(sensor);
        await sensorRepository.SaveChangesAsync(cancellationToken);
        return Map(sensor);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sensor = await sensorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Sensor not found.");

        sensorRepository.Remove(sensor);
        await sensorRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDependenciesExist(int tipoSensorId, int estufaId, CancellationToken cancellationToken)
    {
        if (await tipoSensorRepository.GetByIdAsync(tipoSensorId, cancellationToken) is null)
            throw new KeyNotFoundException("TipoSensor not found.");

        if (await estufaRepository.GetByIdAsync(estufaId, cancellationToken) is null)
            throw new KeyNotFoundException("Estufa not found.");
    }

    private static SensorResponseDto Map(Sensor sensor) => new(sensor.Id, sensor.TipoSensorId, sensor.EstufaId);
}
