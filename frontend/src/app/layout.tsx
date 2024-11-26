import '@mantine/core/styles.css';

import React from 'react';
import { ColorSchemeScript, MantineProvider } from '@mantine/core';
import { theme } from '@/theme';
import {Shell} from "@/src/components/AppShell";

export const metadata = {
  title: 'Mantine Next.js template',
  description: 'I am using Mantine with Next.js!',
};

export default function RootLayout({ children }: { children: any }) {
  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <ColorSchemeScript />
        <link rel="shortcut icon" href="/favicon.svg" />
        <meta
          name="viewport"
          content="minimum-scale=1, initial-scale=1, width=device-width, user-scalable=no"
        />
        <title>Media</title>
      </head>
      <body>
        <MantineProvider theme={theme} defaultColorScheme="dark">
          <Shell>
            {children}
          </Shell>
        </MantineProvider>
      </body>
    </html>
  );
}
