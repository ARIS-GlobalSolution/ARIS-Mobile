import React, { createContext, useContext, useState } from 'react';
import { setAuthToken } from '../services/api';
import { removerUsuario } from '../services/usuariosService';

const normalizeUser = (user = null) => {
  if (!user) {
    return null;
  }

  return {
    id: user.id ?? user.usuarioId ?? null,
    name: user.name ?? user.nome ?? '',
    email: user.email ?? '',
    dataCadastro: user.dataCadastro ?? user.data_cadastro ?? null,
  };
};

const AuthContext = createContext({
  user: null,
  token: null,
  signIn: () => {},
  signOut: () => {},
  deleteAccount: async () => {},
});

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);

  const signIn = ({ user: nextUser = null, token: nextToken = null } = {}) => {
    setUser(normalizeUser(nextUser));
    setToken(nextToken);
    setAuthToken(nextToken);
  };

  const signOut = () => {
    setUser(null);
    setToken(null);
    setAuthToken(null);
  };

  const deleteAccount = async () => {
    if (user?.id) {
      await removerUsuario(user.id);
    }

    signOut();
  };

  return (
    <AuthContext.Provider value={{ user, token, signIn, signOut, deleteAccount }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}

export default AuthContext;
