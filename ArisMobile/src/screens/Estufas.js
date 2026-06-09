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
import { criarEstufa, listarEstufas, removerEstufa, atualizarEstufa } from '../services/estufasService';
import { globalStyles } from '../styles/globalStyles';
import { theme } from '../styles/tema';

const emptyForm = {
  nome: '',
};

function normalizeEstufa(item) {
  return {
    id: item?.id ?? item?.ID_ESTUFA ?? item?.Id ?? null,
    nome: item?.nome ?? item?.NOME ?? '',
    localizacao: item?.localizacao ?? item?.LOCALIZACAO ?? '',
    usuarioId: item?.usuarioId ?? item?.ID_USUARIO ?? item?.idUsuario ?? '',
  };
}

function getCurrentUserId(user) {
  return user?.id ?? user?.idUsuario ?? user?.usuarioId ?? user?.ID_USUARIO ?? null;
}

export default function Estufas({ navigation }) {
  const { user, token } = useAuth();
  const currentUserId = getCurrentUserId(user);
  const [estufas, setEstufas] = useState([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    navigation.setOptions({ title: 'Estufas' });
    carregarEstufas();
  }, [navigation, token]);

  async function carregarEstufas() {
    try {
      setError('');
      const data = await listarEstufas(token);
      const normalized = Array.isArray(data) ? data.map(normalizeEstufa) : [];
      setEstufas(
        currentUserId == null
          ? normalized
          : normalized.filter((item) => String(item.usuarioId) === String(currentUserId)),
      );
    } catch (err) {
      setError(err.message || 'Nao foi possivel carregar as estufas.');
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
    });
  }

  function validarFormulario() {
    if (!form.nome.trim()) return 'Informe o nome da estufa.';
    if (!currentUserId) return 'Sua sessao expirou. Entre novamente para salvar.';
    return '';
  }

  async function salvar() {
    const validationMessage = validarFormulario();
    if (validationMessage) {
      Alert.alert('Atencao', validationMessage);
      return;
    }

    try {
      setSaving(true);
      const payload = {
        nome: form.nome.trim(),
        localizacao: null,
        usuarioId: Number(currentUserId),
      };

      if (editingId) {
        await atualizarEstufa(editingId, payload, token);
        Alert.alert('Sucesso', 'Estufa atualizada com sucesso.');
      } else {
        await criarEstufa(payload, token);
        Alert.alert('Sucesso', 'Estufa cadastrada com sucesso.');
      }

      limparFormulario();
      await carregarEstufas();
    } catch (err) {
      Alert.alert('Erro', err.message || 'Nao foi possivel salvar a estufa.');
    } finally {
      setSaving(false);
    }
  }

  function confirmarExclusao(item) {
    Alert.alert('Excluir estufa', `Deseja excluir "${item.nome}"?`, [
      { text: 'Cancelar', style: 'cancel' },
      {
        text: 'Excluir',
        style: 'destructive',
        onPress: async () => {
          try {
            setSaving(true);
            await removerEstufa(item.id, token);
            if (editingId === item.id) {
              limparFormulario();
            }
            await carregarEstufas();
            Alert.alert('Sucesso', 'Estufa removida com sucesso.');
          } catch (err) {
            Alert.alert('Erro', err.message || 'Nao foi possivel excluir a estufa.');
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
                      carregarEstufas();
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
                        Cadastre e acompanhe suas estufas com facilidade.
                      </Text>
                    </View>
                  </View>
                  <View style={{ alignItems: 'flex-start' }}>
                    <View style={globalStyles.chip}>
                      <Text style={globalStyles.chipText}>Nucleo da missao</Text>
                    </View>
                  </View>
                </View>

                <View style={[globalStyles.cardStrong, { gap: 12 }]}>
                  <View style={globalStyles.spaceBetween}>
                    <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                      {editingId ? `Editando #${editingId}` : 'Nova estufa'}
                    </Text>
                    <Text style={{ color: theme.colors.accent, fontWeight: '900' }}>ARIS</Text>
                  </View>

                  <View style={{ gap: 10 }}>
                    <View style={{ gap: 8 }}>
                      <Text style={{ color: theme.colors.text, fontWeight: '800' }}>Nome</Text>
                      <TextInput
                        style={globalStyles.input}
                        placeholder="Ex.: Estufa Alpha"
                        placeholderTextColor={theme.colors.muted}
                        value={form.nome}
                        onChangeText={(value) => setForm((current) => ({ ...current, nome: value }))}
                      />
                    </View>

                    <View style={globalStyles.card}>
                      <Text style={{ color: theme.colors.text, fontWeight: '800' }}>
                        ID da estufa
                      </Text>
                      <Text style={{ color: theme.colors.muted, marginTop: 6, lineHeight: 21 }}>
                        Será gerado automaticamente ao salvar e vai aparecer aqui na lista.
                      </Text>
                    </View>

                    <View style={globalStyles.card}>
                      <Text style={{ color: theme.colors.text, fontWeight: '800' }}>
                        Responsável
                      </Text>
                      <Text style={{ color: theme.colors.muted, marginTop: 6, lineHeight: 21 }}>
                        {user?.email || 'E-mail não carregado'}
                      </Text>
                    </View>
                  </View>

                  <View style={{ flexDirection: 'row', gap: 10 }}>
                    <Pressable style={[globalStyles.button, { flex: 1 }]} onPress={salvar} disabled={saving}>
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
                      Suas estufas
                    </Text>
                    {loading ? <ActivityIndicator color={theme.colors.primarySoft} /> : null}
                  </View>
                  <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                    Aqui aparecem so as estufas da sua conta.
                  </Text>
                  <Text style={{ color: theme.colors.muted, lineHeight: 21 }}>
                    O ID gerado da estufa aparece no cartão e é ele que você vai usar ao criar uma cultura.
                  </Text>
                  {error ? <Text style={{ color: theme.colors.danger, lineHeight: 21 }}>{error}</Text> : null}
                </View>

                {!loading && estufas.length === 0 ? (
                  <View style={[globalStyles.cardStrong, { marginTop: 12 }]}>
                    <Text style={{ color: theme.colors.text, fontSize: 16, fontWeight: '900' }}>
                      Nenhuma estufa cadastrada
                    </Text>
                    <Text style={{ color: theme.colors.muted, marginTop: 8, lineHeight: 21 }}>
                      Use o formulario acima para criar a primeira estufa.
                    </Text>
                  </View>
                ) : null}

                {estufas.map((item) => (
                  <View key={item.id} style={[globalStyles.cardStrong, { marginTop: 12, gap: 12 }]}>
                    <View style={globalStyles.spaceBetween}>
                      <View style={{ flex: 1, paddingRight: 10 }}>
                        <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>
                          {item.nome}
                        </Text>
                        <Text style={{ color: theme.colors.muted, marginTop: 4 }}>
                          ID da estufa: {item.id}
                        </Text>
                      </View>
                      <View style={globalStyles.chip}>
                        <Text style={globalStyles.chipText}>ID {item.id}</Text>
                      </View>
                    </View>

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
            <BottomNavigation navigation={navigation} active="Estufas" />
          </SafeAreaView>
        </View>
      </ImageBackground>
    </View>
  );
}
