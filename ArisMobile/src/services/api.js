import { Platform } from 'react-native';
import axios from 'axios';

const resolveDefaultApiUrl = () => {
  if (Platform.OS === 'android') {
    return 'http://10.0.2.2:5070/api';
  }

  return 'http://localhost:5070/api';
};

const API_BASE_URL = process.env.EXPO_PUBLIC_API_URL || resolveDefaultApiUrl();

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const setAuthToken = (token) => {
  if (token) {
    api.defaults.headers.common.Authorization = `Bearer ${token}`;
    return;
  }

  delete api.defaults.headers.common.Authorization;
};

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const message =
      error.response?.data?.message ||
      error.response?.data?.title ||
      error.response?.data?.error ||
      error.response?.data ||
      error.message ||
      'Nao foi possivel concluir a requisicao.';

    return Promise.reject(new Error(message));
  },
);

export const getApiBaseUrl = () => API_BASE_URL;

export default api;
