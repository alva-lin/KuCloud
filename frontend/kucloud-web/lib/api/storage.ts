import { myAxios } from '@/lib/api/index';
import { FolderDto, PaginatedList, StorageNodeDto } from '@/lib/models';

const baseUrl = '/storage';

export async function getFolder(params: { id: number }): Promise<FolderDto> {
  const resp = await myAxios<FolderDto>(`${baseUrl}/${params.id}`, {
    method: 'GET',
  });
  return resp.data;
}

export async function addFolder(params: { name: string; parentId?: number }): Promise<number> {
  const resp = await myAxios<number>(`${baseUrl}/create-folder`, {
    method: 'POST',
    data: params,
  });
  return resp.data;
}

export async function moveToTrash(params: { ids: number[] }): Promise<void> {
  await myAxios(`${baseUrl}`, {
    method: 'POST',
    data: params,
  });
}

export async function trash(params: {
  page: number;
  pageSize: number;
  keyword?: string;
}): Promise<PaginatedList<StorageNodeDto>> {
  const resp = await myAxios<PaginatedList<StorageNodeDto>>(`${baseUrl}/deleted`, {
    method: 'GET',
    params,
  });
  return resp.data;
}

export async function downloadFile(params: { id: number }): Promise<void> {
  await myAxios(`${baseUrl}/download/${params.id}`, {
    method: 'GET',
    responseType: 'blob',
  });
}

export async function move(params: { ids: number[]; parentId: number }): Promise<void> {
  await myAxios(`${baseUrl}/move`, {
    method: 'POST',
    data: params,
  });
}

export async function rename(params: { id: number; name: string }): Promise<void> {
  await myAxios(`${baseUrl}/rename`, {
    method: 'POST',
    data: params,
  });
}

export async function restore(params: { ids: number[] }): Promise<void> {
  await myAxios(`${baseUrl}/restore`, {
    method: 'POST',
    data: params,
  });
}

export async function uploadFile(params: { parentId: number; file: File }): Promise<number> {
  const formData = new FormData();
  formData.append('file', params.file);
  const resp = await myAxios<string>(`${baseUrl}/upload`, {
    method: 'POST',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  const path = resp.data;

  const resp2 = await myAxios<number>(`${baseUrl}/add-file`, {
    method: 'POST',
    data: { parentId: params.parentId, path, name: params.file.name },
  });
  return resp2.data;
}
