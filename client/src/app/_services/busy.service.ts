import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount=0;

  private service = inject(NgxSpinnerService);

  busy(){
    this.busyRequestCount++;
    this.service.show(undefined,{
      type:'ball-atom',
      bdColor:'rgba(255,255,255,0)',
      color:'#333333'
    });
  }

  idol(){
    this.busyRequestCount--;
    if (this.busyRequestCount<=0) {

      this.busyRequestCount=0;
      this.service.hide();
    }
  }
}
