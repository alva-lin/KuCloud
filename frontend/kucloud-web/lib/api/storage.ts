import { myAxios } from '@/lib/api/index';
import { FolderDto } from '@/lib/models';

const baseUrl = '/storage';

export async function getFolder(params: { id: number }): Promise<FolderDto> {
  const resp = await myAxios<FolderDto>(`${baseUrl}/${params.id}`, {
    method: 'GET',
  });
  return resp.data;
}
