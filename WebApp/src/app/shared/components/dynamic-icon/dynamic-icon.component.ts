import { Component, Input } from "@angular/core";
import { LucideArchive, LucideFingerprintPattern, LucideHouse, LucideIdCard, LucideKeyRound, LucideShieldPlus } from "@lucide/angular";

@Component({
  selector: 'app-dynamic-icon',
  imports: [LucideHouse, LucideIdCard, LucideFingerprintPattern, LucideKeyRound, LucideArchive, LucideShieldPlus],
  template: `
    @switch (name) {
      @case ('IdCard') { <svg [style.width]="width" [style.height]="height" lucideIdCard></svg> }
      @case ('House') { <svg [style.width]="width" [style.height]="height" lucideHouse></svg> }
      @case ('Fingerprint') { <svg [style.width]="width" [style.height]="height" lucideFingerprintPattern></svg> }
      @case ('KeyRound') { <svg [style.width]="width" [style.height]="height" lucideKeyRound></svg> }
      @case ('Archive') { <svg [style.width]="width" [style.height]="height" lucideArchive></svg> }
      @case ('ShieldPlus') { <svg [style.width]="width" [style.height]="height" lucideShieldPlus></svg> }
    }
  `
})
export class DynamicIconComponent {
  @Input({ required: true }) name = '';
  @Input() width: string = '100%';
  @Input() height: string = '100%';

  constructor() {
    console.log(this.name);
  }
}