import React from 'react';

import { ColorSchemeScript } from '@mantine/core';
import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';

import { MyMantineProvider, MyQueryClientProvider } from '@/lib/providers';

import './globals.css';

export const metadata = {
  title: 'KuCloud - 私人云盘',
  description:
    'KuCloud is a cloud storage service that allows you to store and share files online.',
};

export default function RootLayout({ children }: { children: any }) {
  return (
    <html lang="en">
      <head>
        <ColorSchemeScript />
        <link rel="shortcut icon" href="/favicon.svg" />
        <meta
          name="viewport"
          content="minimum-scale=1, initial-scale=1, width=device-width, user-scalable=no"
        />
      </head>
      <body>
        <MyMantineProvider>
          <MyQueryClientProvider>{children}</MyQueryClientProvider>
        </MyMantineProvider>
      </body>
    </html>
  );
}
