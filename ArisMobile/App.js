import React from 'react';
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { StatusBar } from 'expo-status-bar';

import { AuthProvider, useAuth } from './src/context/AuthContext';
import Alertas from './src/screens/Alertas';
import Cadastro from './src/screens/Cadastro';
import Dashboard from './src/screens/Dashboard';
import Estufas from './src/screens/Estufas';
import Inicial from './src/screens/Inicial';
import Login from './src/screens/Login';
import Perfil from './src/screens/Perfil';
import Plantacoes from './src/screens/Plantacoes';
import { theme } from './src/styles/tema';

const Stack = createNativeStackNavigator();

const appTheme = {
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

function AppNavigator() {
  const { user, token } = useAuth();
  const loggedIn = Boolean(user && token);

  return (
    <NavigationContainer theme={appTheme}>
      <StatusBar style="light" backgroundColor={theme.colors.background} />
      <Stack.Navigator
        key={loggedIn ? 'auth' : 'guest'}
        initialRouteName={loggedIn ? 'Dashboard' : 'Inicial'}
        screenOptions={{
          headerShown: false,
          contentStyle: { backgroundColor: theme.colors.background },
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

export default function App() {
  return (
    <AuthProvider>
      <SafeAreaProvider>
        <AppNavigator />
      </SafeAreaProvider>
    </AuthProvider>
  );
}
