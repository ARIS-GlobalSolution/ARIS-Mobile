import React, { useEffect, useState } from 'react';
import {
  ActivityIndicator,
  Alert,
  Image,
  ImageBackground,
  KeyboardAvoidingView,
  Platform,
  Pressable,
  RefreshControl,
  SafeAreaView,
  ScrollView,
  Text,
  TextInput,
  View,
} from 'react-native';
import BottomNavigation from '../components/BottomNavigation';
import { useAuth } from '../context/AuthContext';
import { criarCultura, listarCulturas, removerCultura, atualizarCultura } from '../services/culturasService';
import { listarEstufas } from '../services/estufasService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

const emptyForm = {
  nome: '',
  estufaId: '',
  tempMin: '',
  tempMax: '',
  umidadeMin: '',
  umidadeMax: '',
};

function normalizeCultura(item) {
  return {
    id: item?.id ?? item?.ID_CULTURA ?? item?.Id ?? null,
    nome: item?.nome ?? item?.NOME ?? '',
    estufaId: item?.estufaId ?? item?.ID_ESTUFA ?? item?.idEstufa ?? '',
    tempMin: item?.tempMin ?? item?.temperaturaMinima ?? item?.TemperaturaMinima ?? '',
    tempMax: item?.tempMax ?? item?.temperaturaMaxima ?? item?.TemperaturaMaxima ?? '',
    umidadeMin: item?.umidadeMin ?? item?.umidadeMinima ?? item?.UmidadeMinima ?? '',
    umidadeMax: item?.umidadeMax ?? item?.umidadeMaxima ?? item?.UmidadeMaxima ?? '',
  };
}

function formatValue(value, suffix = '') {
  if (value === null || value === undefined || value === '') {
    return 'N/D';
  }

  return `${value}${suffix}`;
}

function normalizeEstufa(item) {
  return {
    id: item?.id ?? item?.ID_ESTUFA ?? item?.Id ?? null,
    nome: item?.nome ?? item?.NOME ?? '',
    usuarioId: item?.usuarioId ?? item?.ID_USUARIO ?? item?.idUsuario ?? '',
  };
}

function getCurrentUserId(user) {
  return user?.id ?? user?.idUsuario ?? user?.usuarioId ?? user?.ID_USUARIO ?? null;
}

export default function Plantacoes({ navigation }) {
  const { user, token } = useAuth();
  const currentUserId = getCurrentUserId(user);
  const [culturas, setCulturas] = useState([]);
  const [estufas, setEstufas] = useState([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    navigation.setOptions({ title: 'Culturas' });
    carregarDados();
  }, [navigation, token]);

  async function carregarDados() {
    try {
      setError('');
      const [culturasData, estufasData] = await Promise.all([
        listarCulturas(token),
        listarEstufas(token),
      ]);

      const culturasNormalizadas = Array.isArray(culturasData) ? culturasData.map(normalizeCultura) : [];
      const estufasNormalizadas = Array.isArray(estufasData) ? estufasData.map(normalizeEstufa) : [];

      setCulturas(culturasNormalizadas);
      setEstufas(
        currentUserId == null
          ? estufasNormalizadas
          : estufasNormalizadas.filter((item) => String(item.usuarioId) === String(currentUserId)),
      );
    } catch (err) {
      setError(err.message || 'Nao foi possivel carregar as culturas.');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }

  function limparFormulario() {
    setForm(emptyForm);
    setEditingId(null);
  }

  function preencherEdicao(item) {
    setEditingId(item.id);
    setForm({
      nome: String(item.nome ?? ''),
      estufaId: String(item.estufaId ?? ''),
      tempMin: String(item.tempMin ?? ''),
      tempMax: String(item.tempMax ?? ''),
      umidadeMin: String(item.umidadeMin ?? ''),
      umidadeMax: String(item.umidadeMax ?? ''),
    });
  }

  function selecionarEstufa(estufaId) {
    setForm((current) => ({ ...current, estufaId: String(estufaId) }));
  }

  function validarFormulario() {
    if (!form.nome.trim()) return 'Informe o nome da cultura.';
    if (!form.estufaId.trim()) return 'Informe a estufa.';
    if (!form.tempMin.trim() || !form.tempMax.trim()) return 'Informe os limites de temperatura.';
    if (!form.umidadeMin.trim() || !form.umidadeMax.trim()) return 'Informe os limites de umidade.';
    return '';
  }

  async function salvar() {
    const validationMessage = validarFormulario();
    if (validationMessage) {
      Alert.alert('Atencao', validationMessage);
      return;
    }

    if (estufas.length === 0) {
      Alert.alert('Atencao', 'Cadastre uma estufa primeiro para criar uma cultura.');
      return;
    }

    try {
      setSaving(true);
      const payload = {
        nome: form.nome.trim(),
        estufaId: Number(form.estufaId),
        tempMin: Number(form.tempMin),
        tempMax: Number(form.tempMax),
        umidadeMin: Number(form.umidadeMin),
        umidadeMax: Number(form.umidadeMax),
      };

      if (editingId) {
        await atualizarCultura(editingId, payload, token);
        Alert.alert('Sucesso', 'Cultura atualizada com sucesso.');
      } else {
        await criarCultura(payload, token);
        Alert.alert('Sucesso', 'Cultura cadastrada com sucesso.');
      }

      limparFormulario();
      await carregarDados();
    } catch (err) {
      Alert.alert('Erro', err.message || 'Nao foi possivel salvar a cultura.');
    } finally {
      setSaving(false);
    }
  }

  function confirmarExclusao(item) {
    Alert.alert('Excluir cultura', `Deseja excluir "${item.nome}"?`, [
      { text: 'Cancelar', style: 'cancel' },
      {
        text: 'Excluir',
        style: 'destructive',
        onPress: async () => {
          try {
            setSaving(true);
            await removerCultura(item.id, token);
            if (editingId === item.id) {
              limparFormulario();
            }
            await carregarDados();
            Alert.alert('Sucesso', 'Cultura removida com sucesso.');
          } catch (err) {
            Alert.alert('Erro', err.message || 'Nao foi possivel excluir a cultura.');
          } finally {
            setSaving(false);
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
            <KeyboardAvoidingView
              style={{ flex: 1 }}
              behavior={Platform.OS === 'ios' ? 'padding' : undefined}
            >
              <ScrollView
                keyboardShouldPersistTaps="handled"
                refreshControl={
                  <RefreshControl
                    refreshing={refreshing}
                    onRefresh={() => {
                      setRefreshing(true);
                      carregarDados();
                    }}
                    tintColor={theme.colors.primarySoft}
                  />
                }
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
                        ARIS
                      </Text>
                      <Text style={{ color: theme.colors.muted, lineHeight: 21, textAlign: 'left', marginTop: 6 }}>
                        Cadastre e acompanhe seus cultivos com facilidade.
                      </Text>
                    </View>
                  </View>
                  <View style={{ alignItems: 'flex-start' }}>
                    <View style={globalStyles.chip}>
                      <Text style={globalStyles.chipText}>Gestao da conta</Text>
                    </View>
                  </View>
                </View>

                <View style={[globalStyles.cardStrong, { gap: 12 }]}>
                  <View style={globalStyles.spaceBetween}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                      {editingId ? `Editando #${editingId}` : 'Nova cultura'}
                    </Text>
                    <Text style={{ color: theme.colors.accent, fontWeight: '900' }}>ARIS</Text>
                  </View>

                  <View style={{ gap: 10 }}>
                    <View style={{ gap: 8 }}>
                      <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Nome</Text>
                      <TextInput
                        style={globalStyles.input}
                        placeholder="Ex.: Alface"
                        placeholderTextColor={theme.colors.muted}
                        value={form.nome}
                        onChangeText={(value) => setForm((current) => ({ ...current, nome: value }))}
                      />
                    </View>

                    <View style={{ gap: 8 }}>
                      <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Estufa</Text>
                      <TextInput
                        style={globalStyles.input}
                        placeholder="Ex.: 1"
                        placeholderTextColor={theme.colors.muted}
                        keyboardType="numeric"
                        value={form.estufaId}
                        onChangeText={(value) => setForm((current) => ({ ...current, estufaId: value }))}
                      />
                    </View>

                    <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                      Toque em uma estufa da lista abaixo para preencher o ID automaticamente.
                    </Text>

                    <View style={{ flexDirection: 'row', flexWrap: 'wrap', gap: 8 }}>
                      {estufas.length === 0 ? (
                        <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                          Você ainda não tem estufas cadastradas.
                        </Text>
                      ) : (
                        estufas.map((estufa) => (
                          <Pressable
                            key={estufa.id}
                            style={globalStyles.chip}
                            onPress={() => selecionarEstufa(estufa.id)}
                          >
                            <Text style={globalStyles.chipText}>
                              #{estufa.id} {estufa.nome}
                            </Text>
                          </Pressable>
                        ))
                      )}
                    </View>

                    {form.estufaId ? (
                      <Text style={{ color: theme.colors.accent, lineHeight: 21, fontWeight: '800' }}>
                        Estufa selecionada: #{form.estufaId}
                      </Text>
                    ) : null}

                    <View style={{ flexDirection: 'row', gap: 10 }}>
                      <View style={{ flex: 1, gap: 8 }}>
                        <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Temp. min.</Text>
                        <TextInput
                          style={globalStyles.input}
                          placeholder="10"
                          placeholderTextColor={theme.colors.muted}
                          keyboardType="numeric"
                          value={form.tempMin}
                          onChangeText={(value) => setForm((current) => ({ ...current, tempMin: value }))}
                        />
                      </View>
                      <View style={{ flex: 1, gap: 8 }}>
                        <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Temp. max.</Text>
                        <TextInput
                          style={globalStyles.input}
                          placeholder="25"
                          placeholderTextColor={theme.colors.muted}
                          keyboardType="numeric"
                          value={form.tempMax}
                          onChangeText={(value) => setForm((current) => ({ ...current, tempMax: value }))}
                        />
                      </View>
                    </View>

                    <View style={{ flexDirection: 'row', gap: 10 }}>
                      <View style={{ flex: 1, gap: 8 }}>
                        <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Umid. min.</Text>
                        <TextInput
                          style={globalStyles.input}
                          placeholder="60"
                          placeholderTextColor={theme.colors.muted}
                          keyboardType="numeric"
                          value={form.umidadeMin}
                          onChangeText={(value) => setForm((current) => ({ ...current, umidadeMin: value }))}
                        />
                      </View>
                      <View style={{ flex: 1, gap: 8 }}>
                        <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Umid. max.</Text>
                        <TextInput
                          style={globalStyles.input}
                          placeholder="80"
                          placeholderTextColor={theme.colors.muted}
                          keyboardType="numeric"
                          value={form.umidadeMax}
                          onChangeText={(value) => setForm((current) => ({ ...current, umidadeMax: value }))}
                        />
                      </View>
                    </View>
                  </View>

                  <View style={{ flexDirection: 'row', gap: 10 }}>
                    <Pressable
                      style={[globalStyles.button, { flex: 1, opacity: estufas.length === 0 ? 0.6 : 1 }]}
                      onPress={salvar}
                      disabled={saving || estufas.length === 0}
                    >
                      {saving ? (
                        <ActivityIndicator color={theme.colors.background} />
                      ) : (
                        <Text style={globalStyles.buttonText}>
                          {editingId ? 'Atualizar' : 'Salvar'}
                        </Text>
                      )}
                    </Pressable>

                    <Pressable
                      style={[globalStyles.buttonSecondary, { flex: 1 }]}
                      onPress={limparFormulario}
                      disabled={saving}
                    >
                      <Text style={globalStyles.buttonSecondaryText}>Limpar</Text>
                    </Pressable>
                  </View>
                </View>

                <View style={[globalStyles.cardStrong, { marginTop: 18, gap: 12 }]}>
                  <View style={globalStyles.spaceBetween}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                      Suas culturas
                    </Text>
                    {loading ? <ActivityIndicator color={theme.colors.primarySoft} /> : null}
                  </View>
                  <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                    Aqui aparecem so os cultivos da sua conta.
                  </Text>
                  {error ? <Text style={{ color: theme.colors.danger, lineHeight: 21 }}>{error}</Text> : null}
                </View>

                {!loading && culturas.length === 0 ? (
                  <View style={[globalStyles.cardStrong, { marginTop: 12 }]}>
                    <Text style={{ color: theme.colors.text, fontSize: 16, fontWeight: '900' }}>
                      Nenhuma cultura cadastrada
                    </Text>
                    <Text style={{ color: theme.colors.muted, marginTop: 8, lineHeight: 21 }}>
                      Use o formulario acima para criar o primeiro cultivo.
                    </Text>
                  </View>
                ) : null}

                {culturas.map((item) => (
                  <View key={item.id} style={[globalStyles.cardStrong, { marginTop: 12, gap: 12 }]}>
                    <View style={globalStyles.spaceBetween}>
                      <View style={{ flex: 1, paddingRight: 10 }}>
                        <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                          {item.nome}
                        </Text>
                        <Text style={{ color: theme.colors.muted, marginTop: 4 }}>
                          Estufa #{item.estufaId}
                        </Text>
                      </View>
                      <View style={globalStyles.chip}>
                        <Text style={globalStyles.chipText}>ID {item.id}</Text>
                      </View>
                    </View>

                    <Text style={{ color: theme.colors.text, lineHeight: 21 }}>
                      Temperatura: {formatValue(item.tempMin)} C a {formatValue(item.tempMax)} C
                    </Text>
                    <Text style={{ color: theme.colors.text, lineHeight: 21 }}>
                      Umidade: {formatValue(item.umidadeMin, '%')} a {formatValue(item.umidadeMax, '%')}
                    </Text>

                    <View style={{ flexDirection: 'row', gap: 10 }}>
                      <Pressable
                        style={[globalStyles.buttonSecondary, { flex: 1 }]}
                        onPress={() => preencherEdicao(item)}
                        disabled={saving}
                      >
                        <Text style={globalStyles.buttonSecondaryText}>Editar</Text>
                      </Pressable>
                      <Pressable
                        style={[globalStyles.button, { flex: 1, backgroundColor: theme.colors.danger }]}
                        onPress={() => confirmarExclusao(item)}
                        disabled={saving}
                      >
                        <Text style={globalStyles.buttonText}>Excluir</Text>
                      </Pressable>
                    </View>
                  </View>
                ))}
              </ScrollView>
            </KeyboardAvoidingView>
            <BottomNavigation navigation={navigation} active="Plantacoes" />
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
