import { BasicDto } from '@/lib/models/index';

export type StorageType = 'Folder' | 'File';

export const StorageTypeList: StorageType[] = ['Folder', 'File'];

export interface StorageNodeDto extends BasicDto {
  id: number;
  type: StorageType;
  name: string;
  size: number;
}

export interface AncestorInfo {
  id: number;
  name: string;
  level: number;
}

export interface FolderDto extends StorageNodeDto {
  type: 'Folder';
  isRoot: number;
  ancestors: AncestorInfo[];
  children: StorageNodeDto[];
}
