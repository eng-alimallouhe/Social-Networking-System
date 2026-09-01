import { MediaType } from '../../../shared/design-system/components/media-player/media-player';

export interface PostMediaDto {
    url: string;
    order: number;
    type: MediaType;
}
