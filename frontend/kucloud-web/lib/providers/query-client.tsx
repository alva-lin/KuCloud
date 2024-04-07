'use client';

import { useState } from 'react';

import { notifications } from '@mantine/notifications';
import {
  Mutation,
  MutationCache,
  MutationMeta,
  Query,
  QueryCache,
  QueryClient,
  QueryClientProvider,
  QueryMeta,
} from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { AxiosError } from 'axios';

import { ProblemDetails } from '@/lib/models';

declare module '@tanstack/react-query' {
  interface Register {
    defaultError: AxiosError<ProblemDetails>;
  }
}

const onErrorHandler = (
  error: AxiosError<ProblemDetails>,
  context: { get meta(): QueryMeta | MutationMeta | undefined }
) => {
  if (error.response) {
    const problemDetail = error.response.data;

    const title = context.meta?.errorTitle
      ? (context.meta.errorTitle as string)
      : problemDetail.status >= 500
        ? 'Server Error'
        : problemDetail.title;

    const message = context.meta?.errorMessage
      ? (context.meta.errorMessage as string)
      : problemDetail.status >= 500
        ? 'oops, there are some errors in the server, please try again later'
        : 'there are some errors in your request, please check your input';

    notifications.show({
      title,
      message,
      color: 'red',
    });
  }
};

const onMutationSuccessHandler = (context: { get meta(): MutationMeta | undefined }) => {
  if (context.meta?.successTitle) {
    const title = context.meta.successTitle as string;
    const message = context.meta.successMessage as string;
    notifications.show({
      title,
      message,
      color: 'green',
    });
  }
};

export const globalQueryOnErrorHandler = (
  error: AxiosError<ProblemDetails>,
  query: Query<unknown, unknown, unknown>
) => {
  onErrorHandler(error, query);
};

export const globalMutationOnErrorHandler = (
  error: AxiosError<ProblemDetails>,
  variables: unknown,
  context: unknown,
  mutation: Mutation<unknown, unknown, unknown>
): Promise<unknown> | unknown => {
  onErrorHandler(error, mutation);

  return Promise.reject(error);
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
            // onError: (error, variables, context) => {
            // },
          },
        },
        queryCache: new QueryCache({
          onError: (error, query) => {
            onErrorHandler(error, query);
          },
        }),
        mutationCache: new MutationCache({
          // global handler, will always be called
          onError: (error, variables, context, mutation) => {
            onErrorHandler(error, mutation);
            return error;
          },
          onSuccess: (data, variables, context, mutation) => {
            onMutationSuccessHandler(mutation);
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
