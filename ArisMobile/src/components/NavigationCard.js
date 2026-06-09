import React from 'react';
import { Pressable, Text, View } from 'react-native';
import { theme } from '../styles/tema';

export default function NavigationCard({ title, value, detail, color = theme.colors.accent, onPress }) {
  return (
    <Pressable
      onPress={onPress}
      style={{
        width: '48%',
        minHeight: 126,
        borderRadius: theme.radius.lg,
        borderWidth: 1,
        borderColor: theme.colors.border,
        backgroundColor: theme.colors.card,
        padding: theme.spacing.md,
        justifyContent: 'space-between',
      }}
    >
      <View style={{ width: 34, height: 4, borderRadius: 3, backgroundColor: color }} />
      <View>
        <Text style={{ color: theme.colors.text, fontSize: 24, fontWeight: '900' }}>{value}</Text>
        <Text style={{ color: theme.colors.text, fontSize: 14, fontWeight: '800', marginTop: 2 }}>{title}</Text>
        <Text style={{ color: theme.colors.muted, fontSize: 12, marginTop: 6 }}>{detail}</Text>
      </View>
    </Pressable>
  );
}
