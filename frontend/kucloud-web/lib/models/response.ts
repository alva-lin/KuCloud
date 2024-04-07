export interface BaseResponse<T> {
  data: T;
  // success: boolean;
  // message: string;
}

export interface PaginatedList<T> extends BaseResponse<T[]> {
  page: number;
  pageSize: number;
  totalPage: number;
  totalCount: number;

  hasPreviousPage: boolean;
  hasNextPage: boolean;

  isFirstPage: boolean;
  isLastPage: boolean;
}

export enum ErrorSeverity {
  Error = 'Error',
  Warning = 'Warning',
  Info = 'Info',
}

export interface Error {
  // 错误字段
  name: string;
  // 错误原因
  reason: string;
  // 错误码
  code: string;
  // 严重性
  severity: ErrorSeverity;
}

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  instance: string;
  traceId: string;
  errors: Error[];
}
