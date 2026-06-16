import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'compactNumber',
  standalone: true
})
export class CompactNumberPipe implements PipeTransform {
  transform(value: number | string | null | undefined): string {
    if (value == null || isNaN(Number(value))) return '0';

    return Intl.NumberFormat('en-US', {
      notation: 'compact',
      maximumFractionDigits: 1
    }).format(Number(value));
  }
}