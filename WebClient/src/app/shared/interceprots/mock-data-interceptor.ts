import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { of } from 'rxjs';

export const mockDataInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('/assets/data/')) {
    const mockReq = req.clone({
      url: `${req.url}.json`
    });
    return next(mockReq);
  }
  return next(req);
};
