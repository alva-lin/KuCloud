'use client';

import React from 'react';

import { AppShell } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';

export default function Layout({ children }: { children: any }) {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [opened, { toggle }] = useDisclosure();

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 200,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding="md"
    >
      <AppShell.Header>{/*<Header opened={opened} toggle={toggle} />*/}</AppShell.Header>

      <AppShell.Navbar px="md">{/*<NavMenu />*/}</AppShell.Navbar>

      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  );
}
