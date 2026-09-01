import { Template } from '../../enums/template.enum';

export interface ResumeSnapshotDto {
    id: string;
    ownerId: string;
    title: string;
    template: Template;
    personalPictureUrl?: string | null;
}
