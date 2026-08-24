import { Component, Input, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-circle-loader',
  imports: [],
  templateUrl: './circle-loader.html',
  styleUrl: './circle-loader.css',
})
export class CircleLoader implements OnInit, OnDestroy {
  @Input() translationKey?: string;
  @Input({ required: false }) isChangeLanguage: boolean = true;

  private translate = inject(TranslateService);
  private sub?: Subscription;

  translatedText = signal<string | null>(null);

  ngOnInit() {
    if (this.translationKey) {
      const currentLanguage = this.translate.getCurrentLang();

      let languageCode = "";

      if (this.isChangeLanguage) {
        languageCode = "_EN";
        if (currentLanguage && languageCode.toLocaleLowerCase().startsWith("en")) {
          languageCode = "_AR";
        }
      }

      this.translate.get(`${this.translationKey}${languageCode}`).subscribe(text => {
        this.translatedText.set(text);
      });
    }
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }
}
