import { useCallback } from 'react';

import { Button } from '@mantine/core';

import { Api } from '@/lib/api';
import { FolderDto, StorageNodeDto } from '@/lib/models';

export default function ToolBar(props: { folder: FolderDto; selection: StorageNodeDto[] }) {
  const { folder, selection } = props;
  const firstSelection = selection[0];
  const singleSelection = selection.length === 1;
  const selectionIds = selection.map((item) => item.id);

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
    <div className="flex justify-between items-center">
      <div className="flex gap-4 items-center">
        <Button>Add Folder</Button>
        <Button>Upload File</Button>
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
  );
}
