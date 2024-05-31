'use client';

import { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { useQuery } from '@tanstack/react-query';

import { Navigator, StorageNodeList, ToolBar } from '@/components/Folder';
import { Api } from '@/lib/api';
import { StorageNodeDto } from '@/lib/models';

export default function HomePage({ params }: { params: { folderId?: string[] } }) {
  const router = useRouter();
  const [folderId, setFolderId] = useState<number>(() =>
    params.folderId ? parseInt(params.folderId[0], 10) : 1
  );
  const [selection, setSelection] = useState<StorageNodeDto[]>([]);
  const { data: folder, refetch } = useQuery({
    queryKey: Api.Storage.getFolderQueryKey({ id: folderId }),
    queryFn: () => Api.Storage.getFolder({ id: folderId }),
  });

  useEffect(() => {
    if (folderId) {
      router.replace(`/${folderId}`);
    }
  }, [folderId, router]);

  if (!folder) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <ToolBar folder={folder} selection={selection} refresh={refetch} />
      <Navigator name={folder.name} ancestors={folder.ancestors} onClick={setFolderId} />
      <StorageNodeList
        nodes={folder.children}
        onSelectionChange={setSelection}
        onClickNextFolder={setFolderId}
      />
    </>
  );
}
