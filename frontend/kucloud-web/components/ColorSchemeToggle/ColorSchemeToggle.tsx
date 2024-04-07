'use client';

import { useCallback } from 'react';

import { Button, Group, useMantineColorScheme } from '@mantine/core';
import { notifications } from '@mantine/notifications';

export function ColorSchemeToggle() {
  const { setColorScheme } = useMantineColorScheme();

  const changeColorScheme = useCallback(
    (scheme: 'light' | 'dark' | 'auto') => {
      notifications.show({
        title: 'Color scheme changed',
        message: `Switched to ${scheme} color scheme`,
      });
      setColorScheme(scheme);
    },
    [setColorScheme]
  );

  return (
    <Group justify="center" mt="xl">
      <Button onClick={() => changeColorScheme('light')}>Light</Button>
      <Button onClick={() => changeColorScheme('dark')}>Dark</Button>
      <Button onClick={() => changeColorScheme('auto')}>Auto</Button>
    </Group>
  );
}
