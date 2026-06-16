import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));

document.addEventListener('DOMContentLoaded', () => {
  const loader = document.getElementById('intro-loader');

  setTimeout(() => {
    loader?.classList.add('fade-out');

    setTimeout(() => {
      loader?.remove();
    }, 600);
  }, 3000);
});
