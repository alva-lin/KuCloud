import { useCallback } from 'react';

import { Button, TextInput } from '@mantine/core';
import { useField } from '@mantine/form';
import { notifications } from '@mantine/notifications';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { Api } from '@/lib/api';
import { ProblemDetails } from '@/lib/models';

export default function CreateFolderForm(props: {
  folderId: number;
  afterSubmit?: (success: boolean) => void;
}) {
  const { folderId, afterSubmit } = props;

  const nameField = useField<string>({
    initialValue: '',
    validateOnChange: true,
    validateOnBlur: true,
    validate: (value: string) => {
      if (value.trim().length < 3) {
        return '名称长度不能小于 3';
      }
      return null;
    },
  });

  const queryClient = useQueryClient();
  const mutation = useMutation({
    mutationFn: Api.Storage.addFolder,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: Api.Storage.getFolderQueryKey({ id: folderId }) });
      notifications.show({
        message: '创建成功',
        color: 'green',
      });

      afterSubmit && afterSubmit(true);
    },
    onError: (error: AxiosError<ProblemDetails>) => {
      const problemDetails = error.response?.data;
      if (problemDetails) {
        const { errors } = problemDetails;
        const otherError: string[] = [];
        errors.forEach((err) => {
          if (err.name === 'name') {
            nameField.setError(err.reason);
          } else {
            otherError.push(err.reason);
          }
        });

        notifications.show({
          title: '创建失败',
          message: otherError.join(', '),
          color: 'red',
        });
      }

      afterSubmit && afterSubmit(false);
    },
  });

  const submit = useCallback(
    (name: string) => {
      mutation.mutate({
        parentId: folderId,
        name: name.trim(),
      });
    },
    [afterSubmit, mutation, folderId]
  );

  return (
    <div className="flex flex-col gap-4">
      <TextInput {...nameField.getInputProps()} placeholder="输入你的文件夹名称"></TextInput>
      <div className="flex justify-end items-center">
        <Button
          onClick={() => {
            submit(nameField.getValue());
          }}
          disabled={!!nameField.error}
        >
          创建
        </Button>
      </div>
    </div>
  );
}
