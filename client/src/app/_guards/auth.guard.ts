import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {

  const accservice = inject(AccountService);

  const toaster = inject(ToastrService);
  if (accservice.currentUser()) {
      return true;
  }
  else{
    toaster.error('You shall not pass');
    return false;
  }
};
