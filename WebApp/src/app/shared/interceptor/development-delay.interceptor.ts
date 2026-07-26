import { HttpInterceptorFn } from '@angular/common/http';
import { delay } from 'rxjs/operators';

export const developmentDelayInterceptor: HttpInterceptorFn = (req, next) => {

    console.log("HTTP:", req.url);

    return next(req).pipe(
        delay(1500)
    );

};