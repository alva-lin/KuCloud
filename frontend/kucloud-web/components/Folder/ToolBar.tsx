import React, { useCallback } from 'react';

import { Button, Menu } from '@mantine/core';

import UploadFileButton from '@/components/Folder/UploadFileButton';
import { Api } from '@/lib/api';
import { useContextModal } from '@/lib/hooks/useContextModal';
import { FolderDto, StorageNodeDto } from '@/lib/models';

import CreateFolderForm from './CreateFolderForm';

export default function ToolBar(props: {
  folder: FolderDto;
  selection: StorageNodeDto[];
  refresh: () => void;
}) {
  const { folder, selection } = props;
  const firstSelection = selection[0];
  const singleSelection = selection.length === 1;
  const selectionIds = selection.map((item) => item.id);

  const { open: openCreateModal, close: closeCreateModal } = useContextModal({
    centered: true,
    title: '新建文件夹',
    children: (
      <CreateFolderForm
        folderId={folder.id}
        afterSubmit={(success) => {
          success && closeCreateModal();
        }}
      />
    ),
  });

  const download = useCallback(() => {
    if (firstSelection) {
      Api.Storage.downloadFile({ id: firstSelection.id });
    }
  }, [firstSelection]);

  const move = useCallback(() => {
    Api.Storage.move({ ids: selectionIds, parentId: folder.id });
  }, [selectionIds, folder]);

  const moveToTrash = useCallback(() => {
    Api.Storage.moveToTrash({ ids: selectionIds });
  }, [selectionIds]);

  const rename = useCallback(() => {
    if (singleSelection) {
      // eslint-disable-next-line no-alert
      alert(`rename ${firstSelection.name}`);
    }
  }, [singleSelection, firstSelection]);

  return (
    <>
      <div className="flex justify-between items-center">
        <div className="flex gap-4 items-center">
          <Menu position="bottom-start" withArrow arrowPosition="center">
            <Menu.Target>
              <Button>新增</Button>
            </Menu.Target>

            <Menu.Dropdown>
              <Menu.Item
                leftSection={
                  <span
                    className="i-fluent-folder-24-regular w-6 h-6"
                    role="img"
                    aria-hidden="true"
                  />
                }
                onClick={openCreateModal}
              >
                新建文件夹
              </Menu.Item>
              <Menu.Divider />

              <Menu.Item
                leftSection={
                  <span
                    className="i-fluent-document-24-regular w-6 h-6"
                    role="img"
                    aria-hidden="true"
                  />
                }
              >
                <UploadFileButton parentId={folder.id}>{() => '上传文件'}</UploadFileButton>
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
          {singleSelection && firstSelection?.type === 'File' && (
            <Button onClick={download}>Download</Button>
          )}
          <Button onClick={move}>Move</Button>
          <Button onClick={moveToTrash}>Delete</Button>
          {singleSelection && <Button onClick={rename}>Rename</Button>}
        </div>
        <div className="flex gap-4 items-center">
          {selection.length > 0 && <div>{selection.length} items selected</div>}
          <Button>排序</Button>
          <Button>视图</Button>
        </div>
      </div>
    </>
  );
}
