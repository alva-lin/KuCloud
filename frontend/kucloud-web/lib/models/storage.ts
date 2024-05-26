import { BasicDto } from '@/lib/models/index';

export type StorageType = 'Folder' | 'File';

export const StorageTypeList: StorageType[] = ['Folder', 'File'];

interface InnerStorageNodeDto extends BasicDto {
  id: number;
  name: string;
  size: number;
}

export interface AncestorInfo {
  id: number;
  name: string;
  level: number;
}

export type FolderDto = InnerStorageNodeDto & {
  type: 'Folder';
  isRoot: number;
  ancestors: AncestorInfo[];
  children: StorageNodeDto[];
};

export type FileDto = InnerStorageNodeDto & {
  type: 'File';
};

export type StorageNodeDto = FolderDto | FileDto;
