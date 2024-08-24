import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, delayWhen, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  
  const busy = inject(BusyService);

  busy.busy();
  return next(req).pipe(
    delay(1000),
    finalize(()=>{
      busy.idol();
    })
  );
};
