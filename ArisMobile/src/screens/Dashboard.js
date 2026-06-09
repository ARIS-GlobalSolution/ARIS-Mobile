import React, { useEffect, useState } from 'react';
import { Image, ImageBackground, Pressable, SafeAreaView, ScrollView, Text, View } from 'react-native';
import BottomNavigation from '../components/BottomNavigation';
import NavigationCard from '../components/NavigationCard';
import { useAuth } from '../context/AuthContext';
import { getLatestTelemetry } from '../services/telemetriaService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

export default function Dashboard({ navigation }) {
  const { user } = useAuth();
  const [telemetria, setTelemetria] = useState(null);
  const [carregandoTelemetria, setCarregandoTelemetria] = useState(true);
  const [erroTelemetria, setErroTelemetria] = useState('');

  useEffect(() => {
    let ativo = true;

    const carregar = async () => {
      try {
        setErroTelemetria('');
        const data = await getLatestTelemetry();
        if (ativo) {
          setTelemetria(data);
        }
      } catch (error) {
        if (ativo) {
          setErroTelemetria('Leitura offline no momento.');
        }
      } finally {
        if (ativo) {
          setCarregandoTelemetria(false);
        }
      }
    };

    carregar();
    const interval = setInterval(carregar, 15000);

    return () => {
      ativo = false;
      clearInterval(interval);
    };
  }, []);

  const temperatura = telemetria?.temperature != null ? `${telemetria.temperature.toFixed(1)} C` : '--';
  const umidade = telemetria?.humidity != null ? `${Math.round(telemetria.humidity)}%` : '--';
  const agua = telemetria?.waterLevel != null ? `${Math.round(telemetria.waterLevel)} cm` : '--';
  const statusBase = telemetria?.alert ? 'Atenção' : 'Tudo certo';
  const detalheBase = telemetria?.message || 'Aguardando a primeira leitura.';

  return (
    <View style={globalStyles.screen}>
      <ImageBackground source={require('../assets/fundo.png')} resizeMode="cover" style={globalStyles.backgroundImage}>
        <View style={globalStyles.overlay}>
          <SafeAreaView style={globalStyles.safe}>
            <ScrollView
              contentContainerStyle={[
                globalStyles.content,
                {
                  flexGrow: 1,
                  justifyContent: 'center',
                  paddingTop: 8,
                  paddingBottom: 120,
                },
              ]}
            >
              <View style={[globalStyles.cardStrong, { gap: 14, marginBottom: 18 }]}>
                <View style={{ flexDirection: 'row', alignItems: 'center', gap: 14 }}>
                  <Image source={require('../assets/logo.png')} resizeMode="contain" style={{ width: 130, height: 46 }} />
                  <View style={{ flex: 1, alignItems: 'flex-start' }}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900', textAlign: 'left' }}>
                      {`Olá${user?.name ? `, ${user.name}` : ''}`}
                    </Text>
                    <Text style={{ color: theme.colors.muted, lineHeight: 21, textAlign: 'left', marginTop: 6 }}>
                      Veja seus dados e acesse tudo em um só lugar.
                    </Text>
                  </View>
                </View>
                <View style={{ alignItems: 'flex-start' }}>
                  <View style={globalStyles.chip}>
                    <Text style={globalStyles.chipText}>Seu painel</Text>
                  </View>
                </View>
              </View>

              <View style={{ flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between', gap: 12 }}>
                <NavigationCard
                  title="Temperatura"
                  value={temperatura}
                  detail={carregandoTelemetria ? 'Carregando' : 'Atualizado agora'}
                  color={theme.colors.primary}
                />
                <NavigationCard
                  title="Umidade"
                  value={umidade}
                  detail={carregandoTelemetria ? 'Carregando' : 'Atualizado agora'}
                  color={theme.colors.ember}
                />
                <NavigationCard
                  title="Água"
                  value={agua}
                  detail={carregandoTelemetria ? 'Carregando' : 'Atualizado agora'}
                  color={theme.colors.accent}
                />
                <NavigationCard
                  title="Abrir"
                  value="Cultivos"
                  detail="Ver e cuidar"
                  color={theme.colors.primarySoft}
                  onPress={() => navigation.navigate('Plantacoes')}
                />
                <NavigationCard
                  title="Abrir"
                  value="Estufas"
                  detail="Ver e ajustar"
                  color={theme.colors.warning}
                  onPress={() => navigation.navigate('Estufas')}
                />
              </View>

              <View style={[globalStyles.cardStrong, { marginTop: 18, gap: 12 }]}>
                <View style={globalStyles.spaceBetween}>
                  <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>Como está agora</Text>
                  <Text style={{ color: theme.colors.accent, fontWeight: '900' }}>{statusBase}</Text>
                </View>
                <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                  {detalheBase}
                </Text>
                {erroTelemetria ? (
                  <Text style={{ color: theme.colors.warning, lineHeight: 21 }}>
                    Não foi possível carregar os dados agora.
                  </Text>
                ) : null}
                <View style={{ flexDirection: 'row', gap: 10 }}>
                  <Pressable
                    style={[globalStyles.buttonSecondary, { flex: 1 }]}
                    onPress={() => navigation.navigate('Alertas')}
                  >
                    <Text style={globalStyles.buttonSecondaryText}>Ver avisos</Text>
                  </Pressable>
                  <Pressable
                    style={[globalStyles.button, { flex: 1 }]}
                    onPress={() => navigation.navigate('Perfil')}
                  >
                    <Text style={globalStyles.buttonText}>Minha conta</Text>
                  </Pressable>
                </View>
              </View>
            </ScrollView>
            <BottomNavigation navigation={navigation} active="Dashboard" />
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
