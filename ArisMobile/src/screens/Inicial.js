import React from 'react';
import { Image, ImageBackground, Pressable, SafeAreaView, Text, View } from 'react-native';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

export default function Inicial({ navigation }) {
  return (
    <View style={globalStyles.screen}>
      <ImageBackground source={require('../assets/fundo.png')} resizeMode="cover" style={globalStyles.backgroundImage}>
        <View style={globalStyles.overlay}>
          <SafeAreaView style={globalStyles.safe}>
            <View style={[globalStyles.content, { flex: 1, justifyContent: 'space-between', paddingTop: 8 }]}>
              <View style={{ flex: 1, justifyContent: 'center', gap: 16 }}>
                <Image
                  source={require('../assets/logo.png')}
                  resizeMode="contain"
                  style={{ width: '100%', height: 120 }}
                />
                <View style={[globalStyles.cardStrong, { gap: 10, alignItems: 'center' }]}>
                  <Text
                    style={{
                      color: theme.colors.text,
                      fontSize: 16,
                      lineHeight: 24,
                      textAlign: 'center',
                      fontWeight: '700',
                    }}
                  >
                   "Se conseguimos cultivar em Marte, podemos transformar a agricultura na Terra."
                  </Text>
                </View>
              </View>

              <View style={{ gap: 12, paddingBottom: 12 }}>
                <Pressable style={globalStyles.button} onPress={() => navigation.navigate('Login')}>
                  <Text style={globalStyles.buttonText}>Entrar</Text>
                </Pressable>
                <Pressable style={globalStyles.buttonSecondary} onPress={() => navigation.navigate('Cadastro')}>
                  <Text style={globalStyles.buttonSecondaryText}>Criar conta</Text>
                </Pressable>
              </View>
            </View>
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
