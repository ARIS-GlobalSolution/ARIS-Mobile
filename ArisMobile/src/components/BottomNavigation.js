import React from 'react';
import { Pressable, Text, View } from 'react-native';
import { theme } from '../styles/tema';

const items = [
  { route: 'Dashboard', label: 'Painel', icon: 'Home' },
  { route: 'Plantacoes', label: 'Culturas', icon: '+' },
  { route: 'Alertas', label: 'Alertas', icon: '!' },
  { route: 'Perfil', label: 'Perfil', icon: 'ID' },
];

export default function BottomNavigation({ navigation, active }) {
  return (
    <View
      style={{
        position: 'absolute',
        left: 18,
        right: 18,
        bottom: 20,
        minHeight: 68,
        borderRadius: 18,
        backgroundColor: 'rgba(20, 10, 8, 0.98)',
        borderWidth: 1,
        borderColor: theme.colors.border,
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-around',
        paddingHorizontal: 8,
      }}
    >
      {items.map((item) => {
        const selected = active === item.route;

        return (
          <Pressable
            key={item.route}
            accessibilityRole="button"
            accessibilityLabel={`Ir para ${item.label}`}
            onPress={() => navigation.navigate(item.route)}
            style={{
              alignItems: 'center',
              justifyContent: 'center',
              width: 70,
              minHeight: 52,
              borderRadius: 12,
              backgroundColor: selected ? 'rgba(191, 90, 58, 0.22)' : 'transparent',
            }}
          >
            <Text style={{ color: selected ? theme.colors.accent : theme.colors.muted, fontWeight: '900' }}>
              {item.icon}
            </Text>
            <Text style={{ color: selected ? theme.colors.text : theme.colors.muted, fontSize: 11, marginTop: 4 }}>
              {item.label}
            </Text>
          </Pressable>
        );
      })}
    </View>
  );
}
