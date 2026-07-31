import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'localDate',
    standalone: true
})
export class LocalDatePipe implements PipeTransform {

    transform(value: string | Date | null | undefined): string {
        if (!value) {
            return '';
        }

        const utcDate = new Date(value);

        if (isNaN(utcDate.getTime())) {
            return '';
        }

        return `${utcDate.getFullYear()}/${utcDate.getMonth() + 1}/${utcDate.getDate()}`;
    }
}