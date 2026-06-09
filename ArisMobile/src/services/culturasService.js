import { getApiBaseUrl } from './api';

function buildHeaders(token, extraHeaders = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...extraHeaders,
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  return headers;
}

async function request(path, { method = 'GET', body, token } = {}) {
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    method,
    headers: buildHeaders(token),
    body: body ? JSON.stringify(body) : undefined,
  });

  const text = await response.text();
  let payload = null;

  if (text) {
    try {
      payload = JSON.parse(text);
    } catch {
      payload = text;
    }
  }

  if (!response.ok) {
    const message =
      (payload && typeof payload === 'object' && (payload.message || payload.Message || payload.error || payload.Error)) ||
      (typeof payload === 'string' && payload) ||
      `Falha ao salvar cultura (${response.status}).`;

    const error = new Error(message);
    error.status = response.status;
    error.payload = payload;
    throw error;
  }

  return payload;
}

export async function listarCulturas(token) {
  return request('/culturas', { token });
}

export async function criarCultura(cultura, token) {
  return request('/culturas', {
    method: 'POST',
    body: cultura,
    token,
  });
}

export async function atualizarCultura(id, cultura, token) {
  return request(`/culturas/${id}`, {
    method: 'PUT',
    body: cultura,
    token,
  });
}

export async function removerCultura(id, token) {
  return request(`/culturas/${id}`, {
    method: 'DELETE',
    token,
  });
}
