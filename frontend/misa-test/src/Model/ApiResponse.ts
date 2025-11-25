export interface ApiResponse<T> {
  data: T | null;
  meta?: { page: number; pageSize: number; total: number };
  error?: string;
}
