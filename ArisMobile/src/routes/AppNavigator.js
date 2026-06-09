import React from 'react';
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useAuth } from '../context/AuthContext';
import Cadastro from '../screens/Cadastro';
import Dashboard from '../screens/Dashboard';
import Estufas from '../screens/Estufas';
import Inicial from '../screens/Inicial';
import Login from '../screens/Login';
import Alertas from '../screens/Alertas';
import Perfil from '../screens/Perfil';
import Plantacoes from '../screens/Plantacoes';
import { theme } from '../styles/tema';

const Stack = createNativeStackNavigator();

const navigationTheme = {
  ...DefaultTheme,
  colors: {
    ...DefaultTheme.colors,
    background: theme.colors.background,
    card: theme.colors.surface,
    text: theme.colors.text,
    border: theme.colors.border,
    primary: theme.colors.primary,
  },
};

export default function AppNavigator() {
  const { user, token } = useAuth();
  const loggedIn = Boolean(user && token);

  return (
    <NavigationContainer theme={navigationTheme}>
      <Stack.Navigator
        key={loggedIn ? 'auth' : 'guest'}
        initialRouteName={loggedIn ? 'Dashboard' : 'Inicial'}
        screenOptions={{
          headerShown: false,
          contentStyle: {
            backgroundColor: theme.colors.background,
          },
        }}
      >
        {loggedIn ? (
          <>
            <Stack.Screen name="Dashboard" component={Dashboard} />
            <Stack.Screen name="Estufas" component={Estufas} />
            <Stack.Screen name="Plantacoes" component={Plantacoes} />
            <Stack.Screen name="Alertas" component={Alertas} />
            <Stack.Screen name="Perfil" component={Perfil} />
            <Stack.Screen name="Inicial" component={Inicial} />
            <Stack.Screen name="Login" component={Login} />
            <Stack.Screen name="Cadastro" component={Cadastro} />
          </>
        ) : (
          <>
            <Stack.Screen name="Inicial" component={Inicial} />
            <Stack.Screen name="Login" component={Login} />
            <Stack.Screen name="Cadastro" component={Cadastro} />
            <Stack.Screen name="Dashboard" component={Dashboard} />
            <Stack.Screen name="Estufas" component={Estufas} />
            <Stack.Screen name="Plantacoes" component={Plantacoes} />
            <Stack.Screen name="Alertas" component={Alertas} />
            <Stack.Screen name="Perfil" component={Perfil} />
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}
