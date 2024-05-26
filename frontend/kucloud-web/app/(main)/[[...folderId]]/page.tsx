'use client';

import { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { useQuery } from '@tanstack/react-query';

import { Api } from '@/lib/api';
import { StorageNodeDto } from '@/lib/models';

import Navigator from './Navigator';
import StorageNodeList from './StorageNodeList';
import ToolBar from './ToolBar';

export default function HomePage({ params }: { params: { folderId?: string[] } }) {
  const router = useRouter();
  const [folderId, setFolderId] = useState<number>(() =>
    params.folderId ? parseInt(params.folderId[0], 10) : 1
  );
  const [selection, setSelection] = useState<StorageNodeDto[]>([]);
  const { data: folder } = useQuery({
    queryKey: ['folder', folderId],
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
      <ToolBar folder={folder} selection={selection} />
      <Navigator name={folder.name} ancestors={folder.ancestors} onClick={setFolderId} />
      <StorageNodeList
        nodes={folder.children}
        onSelectionChange={setSelection}
        onClickNextFolder={setFolderId}
      />
    </>
  );
}
