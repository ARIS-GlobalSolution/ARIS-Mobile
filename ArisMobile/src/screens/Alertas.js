import React, { useEffect, useState } from 'react';
import { Image, ImageBackground, SafeAreaView, ScrollView, Text, View } from 'react-native';
import BottomNavigation from '../components/BottomNavigation';
import { getLatestTelemetry } from '../services/telemetriaService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

export default function Alertas({ navigation }) {
  const [telemetria, setTelemetria] = useState(null);
  const [carregando, setCarregando] = useState(true);

  useEffect(() => {
    let ativo = true;

    const carregar = async () => {
      try {
        const data = await getLatestTelemetry();
        if (ativo) {
          setTelemetria(data);
        }
      } catch {
        if (ativo) {
          setTelemetria(null);
        }
      } finally {
        if (ativo) {
          setCarregando(false);
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

  const alertas = [];

  if (telemetria?.temperature != null && (telemetria.temperature < 20 || telemetria.temperature > 30)) {
    alertas.push({
      title: 'Pulso térmico',
      detail: `Temperatura em ${telemetria.temperature.toFixed(1)} C fora da faixa.`,
      color: theme.colors.danger,
    });
  }

  if (telemetria?.humidity != null && (telemetria.humidity < 40 || telemetria.humidity > 80)) {
    alertas.push({
      title: 'Umidade crítica',
      detail: `Umidade em ${Math.round(telemetria.humidity)}% pede atenção.`,
      color: theme.colors.warning,
    });
  }

  if (telemetria?.waterLevel != null && telemetria.waterLevel > 20) {
    alertas.push({
      title: 'Água alta',
      detail: `Nível de água em ${Math.round(telemetria.waterLevel)} cm acima do limite.`,
      color: theme.colors.primarySoft,
    });
  }

  const semAlertas = !carregando && alertas.length === 0;

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
                      Avisos
                    </Text>
                    <Text style={{ color: theme.colors.muted, lineHeight: 21, textAlign: 'left', marginTop: 6 }}>
                      Veja o que precisa da sua atenção.
                    </Text>
                  </View>
                </View>
                <View style={{ alignItems: 'flex-start' }}>
                  <View style={globalStyles.chip}>
                    <Text style={globalStyles.chipText}>{telemetria?.status ?? 'Aguardando leitura'}</Text>
                  </View>
                </View>
              </View>

              {semAlertas ? (
                <View style={globalStyles.cardStrong}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                    Nenhum aviso importante
                    </Text>
                    <Text style={{ color: theme.colors.muted, marginTop: 8, lineHeight: 21 }}>
                    Tudo parece normal no momento.
                    </Text>
                </View>
              ) : null}

              {alertas.map((item) => (
                <View key={item.title} style={[globalStyles.cardStrong, { borderColor: item.color, marginBottom: 12 }]}>
                  <Text style={{ color: item.color, fontSize: 18, fontWeight: '900' }}>{item.title}</Text>
                  <Text style={{ color: theme.colors.text, marginTop: 8, lineHeight: 21 }}>{item.detail}</Text>
                </View>
              ))}

              {telemetria ? (
                <View style={[globalStyles.cardStrong, { gap: 12, marginTop: 6 }]}>
                  <View style={globalStyles.spaceBetween}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>Última atualização</Text>
                    <Text style={{ color: theme.colors.accent, fontWeight: '900' }}>
                      {telemetria.capturedAt ? new Date(telemetria.capturedAt).toLocaleTimeString('pt-BR') : '--'}
                    </Text>
                  </View>
                  <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                    Temperatura {telemetria.temperature != null ? `${telemetria.temperature.toFixed(1)} C` : '--'} •
                    Umidade {telemetria.humidity != null ? `${Math.round(telemetria.humidity)}%` : '--'} •
                    Água {telemetria.waterLevel != null ? `${Math.round(telemetria.waterLevel)} cm` : '--'}
                  </Text>
                </View>
              ) : null}
            </ScrollView>
            <BottomNavigation navigation={navigation} active="Alertas" />
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
