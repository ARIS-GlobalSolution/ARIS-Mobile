import React from 'react';
import { Image, Pressable, Text, View } from 'react-native';
import { theme } from '../styles/tema';

export default function Header({ eyebrow = '', title, subtitle, onBack, rightSlot }) {
  return (
    <View style={{ paddingTop: 18, paddingBottom: 18, gap: 12 }}>
      <View style={{ flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' }}>
        <View style={{ flexDirection: 'row', alignItems: 'center', gap: 10, flex: 1 }}>
          {onBack ? (
            <Pressable
              accessibilityRole="button"
              accessibilityLabel="Voltar"
              onPress={onBack}
              style={{
                width: 42,
                height: 42,
                borderRadius: 14,
                borderWidth: 1,
                borderColor: theme.colors.border,
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: 'rgba(255,255,255,0.03)',
              }}
            >
              <Text style={{ color: theme.colors.text, fontSize: 18, fontWeight: '900' }}>{'<'}</Text>
            </Pressable>
          ) : (
            <Image source={require('../assets/logo.png')} style={{ width: 42, height: 42, borderRadius: 12 }} />
          )}
          {eyebrow ? (
            <Text style={{ color: theme.colors.accent, fontSize: 12, fontWeight: '800', letterSpacing: 0 }}>
              {eyebrow}
            </Text>
          ) : null}
        </View>
        {rightSlot ? <View>{rightSlot}</View> : null}
      </View>
      <Text style={{ color: theme.colors.text, fontSize: 30, fontWeight: '900', marginTop: 2, letterSpacing: -0.4 }}>
        {title}
      </Text>
      {subtitle ? (
        <Text style={{ color: theme.colors.muted, fontSize: 14, lineHeight: 21, marginTop: 8 }}>
          {subtitle}
        </Text>
      ) : null}
    </View>
  );
}
