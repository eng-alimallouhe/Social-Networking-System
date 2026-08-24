import { Pipe, PipeTransform, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'highlight',
  standalone: true
})
export class HighlightPipe implements PipeTransform {
  private sanitizer = inject(DomSanitizer);

  transform(text: string, search: string): SafeHtml {
    if (!search || !text) {
      return text;
    }

    // Escape regex characters from search string
    const escapedSearch = search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const regex = new RegExp(`(${escapedSearch})`, 'gi');
    
    const highlighted = text.replace(regex, `<mark class="search-highlight">$1</mark>`);
    return this.sanitizer.bypassSecurityTrustHtml(highlighted);
  }
}
