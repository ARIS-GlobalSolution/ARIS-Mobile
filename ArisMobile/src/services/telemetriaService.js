import { getApiBaseUrl } from './api';

function normalizeTelemetry(data) {
  const temperature = data?.temperature ?? data?.Temperature ?? null;
  const humidity = data?.humidity ?? data?.Humidity ?? null;
  const waterLevel = data?.waterLevel ?? data?.WaterLevel ?? null;

  return {
    capturedAt: data?.capturedAt ?? data?.CapturedAt ?? null,
    temperature: temperature === null ? null : Number(temperature),
    humidity: humidity === null ? null : Number(humidity),
    waterLevel: waterLevel === null ? null : Number(waterLevel),
    alert: Boolean(data?.alert ?? data?.Alert),
    status: data?.status ?? data?.Status ?? 'Sem leitura',
    message: data?.message ?? data?.Message ?? '',
  };
}

export async function getLatestTelemetry() {
  const response = await fetch(`${getApiBaseUrl()}/telemetrias/latest`);

  if (!response.ok) {
    throw new Error('Não foi possível carregar a telemetria.');
  }

  const data = await response.json();
  return normalizeTelemetry(data);
}
