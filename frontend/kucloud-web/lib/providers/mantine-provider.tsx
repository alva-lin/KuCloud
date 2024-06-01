import { MantineProvider, createTheme } from '@mantine/core';
import { ModalsProvider } from '@mantine/modals';
import { Notifications } from '@mantine/notifications';

const siteTheme = createTheme({
  primaryColor: 'dark',
});

export function MyMantineProvider({ children }: { children: React.ReactNode }) {
  return (
    <MantineProvider theme={siteTheme}>
      <Notifications autoClose={5000} position="top-right" />
      <ModalsProvider>{children}</ModalsProvider>
    </MantineProvider>
  );
}
