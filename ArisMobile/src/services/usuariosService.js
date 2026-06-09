import { getApiBaseUrl } from './api';

function buildUrl(path) {
  const baseUrl = getApiBaseUrl();
  return `${baseUrl}${path}`;
}

async function requestJson(path, options = {}) {
  const response = await fetch(buildUrl(path), {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {}),
    },
    ...options,
  });

  const rawText = await response.text();
  let payload = null;

  if (rawText) {
    try {
      payload = JSON.parse(rawText);
    } catch {
      payload = rawText;
    }
  }

  if (!response.ok) {
    const message =
      (payload && typeof payload === 'object' && (payload.message || payload.Message || payload.error || payload.Error)) ||
      (typeof payload === 'string' && payload) ||
      `Falha na requisição (${response.status}).`;

    const error = new Error(message);
    error.status = response.status;
    error.payload = payload;
    throw error;
  }

  return payload;
}

function normalizeUser(userPayload) {
  if (!userPayload || typeof userPayload !== 'object') {
    return null;
  }

  const id = userPayload.id ?? userPayload.Id ?? userPayload.idUsuario ?? userPayload.ID_USUARIO;

  return {
    id,
    name: userPayload.nome ?? userPayload.Nome ?? userPayload.name ?? userPayload.NOME ?? '',
    email: userPayload.email ?? userPayload.Email ?? userPayload.emailUsuario ?? userPayload.EMAIL ?? '',
  };
}

function normalizeLoginResult(payload) {
  if (!payload || typeof payload !== 'object') {
    return {
      token: null,
      user: null,
      raw: payload,
    };
  }

  const token =
    payload.token ??
    payload.Token ??
    payload.accessToken ??
    payload.AccessToken ??
    payload.jwt ??
    payload.Jwt ??
    null;

  const user = normalizeUser(payload.user ?? payload.User ?? payload.usuario ?? payload.Usuario);

  return {
    token,
    user,
    raw: payload,
  };
}

export async function loginUsuario(email, senha) {
  const payload = await requestJson('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, senha }),
  });

  return normalizeLoginResult(payload);
}

export async function criarUsuario({ nome, email, senha }) {
  return requestJson('/usuarios', {
    method: 'POST',
    body: JSON.stringify({ nome, email, senha }),
  });
}

export async function removerUsuario(id, token) {
  if (id === null || id === undefined) {
    throw new Error('Usuário inválido.');
  }

  const headers = {};

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  return requestJson(`/usuarios/${id}`, {
    method: 'DELETE',
    headers,
  });
}

export function extractLoginResult(payload) {
  return normalizeLoginResult(payload);
}
