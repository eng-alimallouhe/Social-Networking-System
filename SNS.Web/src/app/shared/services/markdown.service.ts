import { Injectable } from '@angular/core';
import { marked } from 'marked';
import DOMPurify from 'dompurify';

@Injectable({
  providedIn: 'root'
})
export class MarkdownService {
  constructor() {
    // Configure marked to ensure consistent rendering
    marked.setOptions({
      breaks: true,
      gfm: true,
    });
  }

  /**
   * Parses Markdown into sanitized HTML.
   */
  parse(markdown: string): string {
    if (!markdown) {
      return '';
    }
    const rawHtml = marked.parse(markdown) as string;
    return DOMPurify.sanitize(rawHtml);
  }

  /**
   * Parses Markdown and returns a truncated version of the sanitized HTML 
   * that preserves HTML tags while capping the visible text length.
   */
  parseAndTruncate(markdown: string, maxLength: number): { html: string, isTruncated: boolean } {
    if (!markdown) {
      return { html: '', isTruncated: false };
    }

    const fullHtml = this.parse(markdown);
    
    // Use the browser's DOM parser to walk the HTML and truncate text nodes
    const div = document.createElement('div');
    div.innerHTML = fullHtml;
    
    let currentLength = 0;
    let isTruncated = false;

    function traverse(node: Node): boolean {
      if (currentLength >= maxLength) {
        node.parentNode?.removeChild(node);
        return false;
      }

      if (node.nodeType === Node.TEXT_NODE) {
        const text = node.textContent || '';
        if (currentLength + text.length > maxLength) {
          // Find the nearest space to avoid cutting mid-word if possible
          let cutIndex = maxLength - currentLength;
          const searchArea = text.substring(0, cutIndex + 15);
          const lastSpace = searchArea.lastIndexOf(' ', cutIndex + 10);
          
          if (lastSpace > cutIndex - 20 && lastSpace <= text.length) {
              cutIndex = lastSpace;
          }
          
          node.textContent = text.substring(0, cutIndex) + '...';
          currentLength = maxLength; // max reached
          isTruncated = true;
        } else {
          currentLength += text.length;
        }
      } else if (node.nodeType === Node.ELEMENT_NODE) {
        const childNodes = Array.from(node.childNodes);
        for (const child of childNodes) {
          if (!traverse(child)) {
            // Remove remaining siblings as the limit was reached
            let next = child.nextSibling;
            while (next) {
              const toRemove = next;
              next = next.nextSibling;
              toRemove.parentNode?.removeChild(toRemove);
            }
            break;
          }
        }
      }
      return currentLength < maxLength;
    }

    Array.from(div.childNodes).forEach(child => traverse(child));

    // Determine if the original text was actually longer than the max length
    const plainText = div.textContent || '';
    // If we didn't hit the truncation limit in traverse but the plain text is short, it's not truncated
    // Actually, isTruncated is set in traverse. Wait, if the text is exactly maxLength, it doesn't add '...'
    // But we need to know if we should show the "Show More" button.
    
    // A more reliable way: just get the full plain text length first
    const fullDiv = document.createElement('div');
    fullDiv.innerHTML = fullHtml;
    const fullTextLength = (fullDiv.textContent || '').length;
    
    if (fullTextLength <= maxLength) {
        return { html: fullHtml, isTruncated: false };
    }

    return { html: div.innerHTML, isTruncated: true };
  }
}
