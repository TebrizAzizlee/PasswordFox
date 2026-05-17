export interface PagedResponse<T> {

  items: T[];

  totalCount: number;

  page: number;

  pageSize: number;

  totalPages: number;

  hasNext: boolean;

  hasPrevious: boolean;
}
