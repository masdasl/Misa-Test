export interface Column {
  title?: string;
  dataIndex: string;
  type: string;
}
export interface Row {
  key: number | string;
  [key: string]: any;
}

export interface PaginationData {
  pageNo: number;
  pageSize: number;
  sortType: "ASC" | "DESC";
  sortKey: string;
  filters?: Record<string, string>;
  searchValue?: Record<string, string>;
}
