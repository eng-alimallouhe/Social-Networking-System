export interface Paged<T> {
    totalCount: number;
    pageSize: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
    currentPage: number;
    items: T[];
}