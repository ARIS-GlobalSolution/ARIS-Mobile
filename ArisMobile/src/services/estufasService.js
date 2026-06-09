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
      `Falha ao salvar estufa (${response.status}).`;

    const error = new Error(message);
    error.status = response.status;
    error.payload = payload;
    throw error;
  }

  return payload;
}

export async function listarEstufas(token) {
  return request('/estufas', { token });
}

export async function criarEstufa(estufa, token) {
  return request('/estufas', {
    method: 'POST',
    body: estufa,
    token,
  });
}

export async function atualizarEstufa(id, estufa, token) {
  return request(`/estufas/${id}`, {
    method: 'PUT',
    body: estufa,
    token,
  });
}

export async function removerEstufa(id, token) {
  return request(`/estufas/${id}`, {
    method: 'DELETE',
    token,
  });
}
