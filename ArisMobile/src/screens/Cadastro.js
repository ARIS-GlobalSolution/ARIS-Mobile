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
import { criarUsuario, loginUsuario } from '../services/usuariosService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

function getFriendlyError(error) {
  const raw = String(error?.message || error || '').toLowerCase();

  if (raw.includes('network request failed') || raw.includes('failed to fetch')) {
    return 'Não foi possível acessar o servidor. Verifique a API e a rede.';
  }

  if (raw.includes('400')) {
    return 'Confira se nome, e-mail e senha estão preenchidos corretamente.';
  }

  if (raw.includes('409') || raw.includes('duplicate') || raw.includes('já existe')) {
    return 'Já existe uma conta com esse e-mail.';
  }

  if (raw.includes('500') || raw.includes('ora-')) {
    return 'O servidor encontrou um problema ao criar a conta. Tente novamente em instantes.';
  }

  return error?.message || 'Não foi possível concluir o cadastro.';
}

export default function Cadastro({ navigation }) {
  const { signIn } = useAuth();
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [salvando, setSalvando] = useState(false);

  function voltarInicio() {
    navigation.reset({
      index: 0,
      routes: [{ name: 'Inicial' }],
    });
  }

  async function cadastrar() {
    const nomeTratado = nome.trim();
    const emailTratado = email.trim().toLowerCase();
    const senhaTratada = senha.trim();

    if (!nomeTratado || !emailTratado || !senhaTratada) {
      Alert.alert('Cadastro incompleto', 'Preencha nome, email e senha.');
      return;
    }

    try {
      setSalvando(true);

      await criarUsuario({
        nome: nomeTratado,
        email: emailTratado,
        senha: senhaTratada,
      });

      Alert.alert('Sucesso', 'Conta criada com sucesso. Entrando na base...');

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
      Alert.alert('Erro de cadastro', getFriendlyError(error));
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
                <Text style={{ color: theme.colors.text, fontSize: 28, fontWeight: '900' }}>Cadastro</Text>
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
                  Crie sua conta ARIS para usar o app e acompanhar suas estufas.
                </Text>
              </View>

              <View style={[globalStyles.cardStrong, { borderRadius: 18, paddingVertical: 22, gap: 14 }]}>
                <View>
                  <Text style={globalStyles.label}>Nome</Text>
                  <TextInput
                    value={nome}
                    onChangeText={setNome}
                    placeholder="Digite seu nome"
                    placeholderTextColor={theme.colors.muted}
                    style={globalStyles.input}
                  />
                </View>

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
                  onPress={cadastrar}
                  disabled={salvando}
                >
                  {salvando ? (
                    <ActivityIndicator color={theme.colors.background} />
                  ) : (
                    <Text style={globalStyles.buttonText}>Cadastrar</Text>
                  )}
                </Pressable>
              </View>
            </ScrollView>
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
