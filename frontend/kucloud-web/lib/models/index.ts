export { ErrorSeverity } from './response';
export type { PaginatedList, Error, ProblemDetails } from './response';

export { StorageTypeList } from './storage';
export type { StorageType, StorageNodeDto, AncestorInfo, FolderDto } from './storage';

export interface AuditRecord {
  creationTime: Date;
  modifiedTime?: Date;
  deletionTime?: Date;
  isDelete: boolean;
}

export interface BasicDto {
  auditRecord: AuditRecord;
}
