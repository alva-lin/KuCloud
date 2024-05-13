'use client';

import { ActionIcon, useMantineColorScheme } from '@mantine/core';

export function ColorSchemeToggle() {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();

  return (
    <ActionIcon variant="default" onClick={() => toggleColorScheme()}>
      {colorScheme === 'dark' ? (
        <span className="i-fluent-weather-moon-24-regular" role="img" />
      ) : (
        <span className="i-fluent-weather-sunny-24-regular" role="img" />
      )}
    </ActionIcon>
  );
}
