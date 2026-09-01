export interface FileNode {
    name: string;
    type: string;
    sizeKB?: number | null;
    url?: string | null;
    children: FileNode[];
}
