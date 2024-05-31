'use client';

import { useState } from 'react';

import { notifications } from '@mantine/notifications';
import { QueryCache, QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { AxiosError } from 'axios';

import { ProblemDetails } from '@/lib/models';

declare module '@tanstack/react-query' {
  interface Register {
    defaultError: AxiosError<ProblemDetails>;
  }
}

const onErrorHandler = (error: AxiosError<ProblemDetails>) => {
  if (error.response) {
    const problemDetail = error.response.data;

    const title = problemDetail.status >= 500 ? 'Server Error' : problemDetail.title;

    const message =
      problemDetail.status >= 500
        ? 'oops, there are some errors in the server, please try again later'
        : 'there are some errors in your request, please check your input';

    notifications.show({
      title,
      message,
      color: 'red',
    });
  }
};

export function MyQueryClientProvider({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // With SSR, we usually want to set some default staleTime above 0
            // to avoid reFetching immediately on the client
            staleTime: 60 * 1000,
          },
          mutations: {
            // can be overridden on each mutation
            onError: (error) => {
              onErrorHandler(error);
              return error;
            },
          },
        },
        queryCache: new QueryCache({
          onError: (error) => {
            onErrorHandler(error);
          },
        }),
      })
  );

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" />
    </QueryClientProvider>
  );
}
