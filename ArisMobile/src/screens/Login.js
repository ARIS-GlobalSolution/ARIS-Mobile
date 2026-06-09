import React, { useState } from 'react';
import {
  Alert,
  ActivityIndicator,
  Image,
  ImageBackground,
  Pressable,
  SafeAreaView,
  ScrollView,
  Text,
  TextInput,
  View,
} from 'react-native';
import { useAuth } from '../context/AuthContext';
import { loginUsuario } from '../services/usuariosService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

function getFriendlyError(error) {
  const raw = String(error?.message || error || '').toLowerCase();

  if (raw.includes('network request failed') || raw.includes('failed to fetch')) {
    return 'Não foi possível acessar o servidor. Verifique a API e a rede.';
  }

  if (raw.includes('401') || raw.includes('unauthorized') || raw.includes('credencial')) {
    return 'E-mail ou senha inválidos.';
  }

  if (raw.includes('404')) {
    return 'O login não foi encontrado na API.';
  }

  if (raw.includes('500') || raw.includes('ora-')) {
    return 'O servidor encontrou um problema ao entrar. Tente novamente em instantes.';
  }

  return error?.message || 'Não foi possível entrar.';
}

export default function Login({ navigation }) {
  const { signIn } = useAuth();
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [salvando, setSalvando] = useState(false);

  function voltarInicio() {
    navigation.reset({
      index: 0,
      routes: [{ name: 'Inicial' }],
    });
  }

  async function entrar() {
    const emailTratado = email.trim().toLowerCase();
    const senhaTratada = senha.trim();

    if (!emailTratado || !senhaTratada) {
      Alert.alert('Login incompleto', 'Preencha email e senha.');
      return;
    }

    try {
      setSalvando(true);

      const loginResult = await loginUsuario(emailTratado, senhaTratada);
      if (!loginResult?.token) {
        throw new Error('A API não devolveu um token.');
      }

      signIn({
        user: loginResult.user,
        token: loginResult.token,
      });

      navigation.reset({
        index: 0,
        routes: [{ name: 'Dashboard' }],
      });
    } catch (error) {
      Alert.alert('Erro de login', getFriendlyError(error));
    } finally {
      setSalvando(false);
    }
  }

  return (
    <View style={globalStyles.screen}>
      <ImageBackground source={require('../assets/fundo.png')} resizeMode="cover" style={globalStyles.backgroundImage}>
        <View style={globalStyles.overlay}>
          <SafeAreaView style={globalStyles.safe}>
            <ScrollView
              keyboardShouldPersistTaps="handled"
              contentContainerStyle={[
                globalStyles.content,
                {
                  flexGrow: 1,
                  justifyContent: 'center',
                  paddingTop: 8,
                  paddingBottom: 36,
                },
              ]}
            >
              <Pressable
                accessibilityRole="button"
                accessibilityLabel="Voltar"
                onPress={voltarInicio}
                style={{
                  width: 42,
                  height: 42,
                  borderRadius: 14,
                  borderWidth: 1,
                  borderColor: theme.colors.border,
                  alignItems: 'center',
                  justifyContent: 'center',
                  backgroundColor: 'rgba(255,255,255,0.03)',
                  marginBottom: 18,
                }}
              >
                <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>{'←'}</Text>
              </Pressable>

              <Image
                source={require('../assets/logo.png')}
                resizeMode="contain"
                style={{ width: 170, height: 70, alignSelf: 'center', marginBottom: 18 }}
              />

              <View style={{ alignItems: 'center', marginBottom: 18 }}>
                <Text style={{ color: theme.colors.text, fontSize: 28, fontWeight: '900' }}>Login</Text>
                <Text
                  style={{
                    color: theme.colors.muted,
                    fontSize: 14,
                    lineHeight: 21,
                    textAlign: 'center',
                    marginTop: 6,
                    maxWidth: 280,
                  }}
                >
                  Entre em sua conta ARIS para usar o app e acompanhar suas estufas.
                </Text>
              </View>

              <View style={[globalStyles.cardStrong, { borderRadius: 18, paddingVertical: 22, gap: 14 }]}>
                <View>
                  <Text style={globalStyles.label}>Email</Text>
                  <TextInput
                    value={email}
                    onChangeText={setEmail}
                    keyboardType="email-address"
                    autoCapitalize="none"
                    placeholder="Digite seu email"
                    placeholderTextColor={theme.colors.muted}
                    style={globalStyles.input}
                  />
                </View>

                <View>
                  <Text style={globalStyles.label}>Senha</Text>
                  <TextInput
                    value={senha}
                    onChangeText={setSenha}
                    secureTextEntry
                    placeholder="Digite sua senha"
                    placeholderTextColor={theme.colors.muted}
                    style={globalStyles.input}
                  />
                </View>

                <Pressable
                  style={[globalStyles.button, { marginTop: 6, marginHorizontal: 20 }]}
                  onPress={entrar}
                  disabled={salvando}
                >
                  {salvando ? (
                    <ActivityIndicator color={theme.colors.background} />
                  ) : (
                    <Text style={globalStyles.buttonText}>Entrar</Text>
                  )}
                </Pressable>

                <Pressable
                  onPress={() => navigation.navigate('Cadastro')}
                  style={[globalStyles.buttonSecondary, { marginHorizontal: 20 }]}
                  disabled={salvando}
                >
                  <Text style={globalStyles.buttonSecondaryText}>Criar conta</Text>
                </Pressable>
              </View>
            </ScrollView>
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
