/*
  文件大小转换为 B, KB, MB, GB, 最多保留以为小数
 */
export function formatFileSize(size?: number, fixed?: number): string {
  fixed = fixed ?? 2;

  if (size === 0 || size === undefined) {
    return '';
  }
  if (size < 1024) {
    return `${size} B`;
  }
  size /= 1024;
  if (size < 1024) {
    return `${size.toFixed(fixed)} KB`;
  }
  size /= 1024;
  if (size < 1024) {
    return `${size.toFixed(fixed)} MB`;
  }
  size /= 1024;
  return `${size.toFixed(fixed)} GB`;
}

/*
  格式化时间，返回 yyyy-MM-dd 格式
 */
export function formatDate(date: Date): string {
  return date.toISOString().split('T')[0];
}
