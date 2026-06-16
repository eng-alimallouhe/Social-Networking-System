import { inject, Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Pipe({
  name: 'dateConverter',
})
export class DateConverterPipe implements PipeTransform {

  private translateService = inject(TranslateService);

  transform(value: Date, ...args: unknown[]): unknown {
    //Convert to local Time:
    const date = new Date(value).toLocaleString('en-US', {
      timeZone: 'Asia/Riyadh',
    });

    console.log('local date' , date);

    const now = new Date();
    const diff = now.getTime() - new Date(date).getTime();
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const weeks = Math.floor(days / 7);
    const months = Math.floor(days / 30);
    const years = Math.floor(days / 365);

    if (seconds < 60) {
      return this.translateService.instant('Time.Just_Now');
    } else if (minutes < 60) {
      return `${minutes} ${this.translateService.instant('Time.Minute_Ago')}`;
    } else if (hours < 24) {
      return `${hours} ${this.translateService.instant('Time.Hour_Ago')}`;
    } else if (days < 7) {
      return `${days} ${this.translateService.instant('Time.Day_Ago')}`;
    } else if (weeks < 4) {
      return `${weeks} ${this.translateService.instant('Time.Week_Ago')}`;
    } else if (months < 12) {
      return `${months} ${this.translateService.instant('Time.Month_Ago')}`;
    } else {
      return `${years} ${this.translateService.instant('Time.Year_Ago')}`;
    }
  }
}
