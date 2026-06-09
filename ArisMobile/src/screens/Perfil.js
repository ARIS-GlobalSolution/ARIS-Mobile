import React from 'react';
import { Alert, Image, ImageBackground, Pressable, SafeAreaView, Text, View } from 'react-native';
import BottomNavigation from '../components/BottomNavigation';
import { useAuth } from '../context/AuthContext';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

export default function Perfil({ navigation }) {
  const { user, signOut, deleteAccount } = useAuth();

  function voltarInicio() {
    navigation.reset({
      index: 0,
      routes: [{ name: 'Inicial' }],
    });
  }

  function sair() {
    signOut();
    voltarInicio();
  }

  function excluirConta() {
    Alert.alert('Excluir conta', 'Essa acao vai remover o usuario do backend e voltar para a tela inicial.', [
      { text: 'Cancelar', style: 'cancel' },
      {
        text: 'Excluir',
        style: 'destructive',
        onPress: async () => {
          try {
            await deleteAccount();
            voltarInicio();
          } catch (error) {
            Alert.alert('Erro', error.message || 'Nao foi possivel excluir a conta.');
          }
        },
      },
    ]);
  }

  return (
    <View style={globalStyles.screen}>
      <ImageBackground source={require('../assets/fundo.png')} resizeMode="cover" style={globalStyles.backgroundImage}>
        <View style={globalStyles.overlay}>
          <SafeAreaView style={globalStyles.safe}>
            <View style={[globalStyles.content, { flex: 1, justifyContent: 'center', paddingTop: 8, paddingBottom: 120 }]}>
              <View style={{ alignItems: 'center', marginBottom: 18 }}>
                <Image source={require('../assets/logo.png')} resizeMode="contain" style={{ width: 140, height: 52 }} />
              </View>

              <View style={[globalStyles.cardStrong, { gap: 14 }]}>
                <View style={{ alignItems: 'center', marginBottom: 2 }}>
                  <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>Sua conta</Text>
                </View>
                <View>
                  <Text style={globalStyles.label}>Nome</Text>
                  <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900', marginTop: 6, textAlign: 'left' }}>
                    {user?.name || 'Visitante'}
                  </Text>
                </View>
                <View>
                  <Text style={globalStyles.label}>E-mail</Text>
                  <Text style={{ color: theme.colors.text, fontSize: 16, fontWeight: '700', marginTop: 6, textAlign: 'left' }}>
                    {user?.email || 'Nao informado'}
                  </Text>
                </View>
                <Pressable style={globalStyles.button} onPress={() => navigation.navigate('Dashboard')}>
                  <Text style={globalStyles.buttonText}>Abrir painel</Text>
                </Pressable>
                <Pressable style={globalStyles.buttonSecondary} onPress={sair}>
                  <Text style={globalStyles.buttonSecondaryText}>Sair</Text>
                </Pressable>
                <Pressable
                  style={[
                    globalStyles.buttonSecondary,
                    { borderColor: 'rgba(216, 91, 73, 0.35)', backgroundColor: 'rgba(216, 91, 73, 0.08)' },
                  ]}
                  onPress={excluirConta}
                >
                  <Text style={{ color: theme.colors.danger, fontWeight: '800', fontSize: 15, textAlign: 'center' }}>
                    Excluir conta
                  </Text>
                </Pressable>
              </View>
            </View>
            <BottomNavigation navigation={navigation} active="Perfil" />
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
