import React, { useCallback, useRef } from 'react';

import { FileButton } from '@mantine/core';
import { useMutation, useQueryClient } from '@tanstack/react-query';

import { Api } from '@/lib/api';

export default function UploadFileButton(props: {
  parentId: number;
  children: (props: { onClick: () => void }) => React.ReactNode;
}) {
  const { parentId, children } = props;

  const queryClient = useQueryClient();
  const mutation = useMutation({
    mutationKey: ['folder', parentId, 'uploadFile'],
    mutationFn: Api.Storage.uploadFile,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: Api.Storage.getFolderQueryKey({ id: parentId }),
      });
    },
  });

  const resetRef = useRef<() => void>(null);
  const uploadFile = useCallback(
    (file: File | null) => {
      if (!file) {
        return;
      }

      resetRef.current?.();
      mutation.mutate({ parentId, file });
    },
    [parentId, mutation, resetRef]
  );

  return (
    <FileButton resetRef={resetRef} onChange={uploadFile}>
      {children}
    </FileButton>
  );
}
