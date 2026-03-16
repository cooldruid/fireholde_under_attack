import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { SnackbarService } from '../../snackbar/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const reason =
        err.error?.reason ??
        err.error?.message ??
        err.error?.title ??
        'An unexpected error occurred';

      snackbar.show(`Error ${err.status}: ${reason}`);
      return throwError(() => err);
    })
  );
};
