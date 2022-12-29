import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
// import { ErrorDialogServiceService } from './Service/error-dialog-service.service';
import {
    HttpInterceptor,
    HttpRequest,
    HttpResponse,
    HttpHandler,
    HttpEvent,
    HttpErrorResponse
} from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

    constructor(private router: Router) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (request.url.indexOf("api/login") === -1 && !request.url.includes("Auth/registerUser")) {
            var token: string = localStorage.getItem('token');
            if (token && token != "undefined") {
                request = request.clone({ headers: request.headers.set('Authorization', 'Basic ' + token) });
            }
        }

        // if(request.url.includes("Token/Refresh"))
        // {
        //     debugger;
        // }
        
        return next.handle(request).pipe(tap(() => {},
            (err: any) => {
                if (err instanceof HttpErrorResponse) {
                    if (err.status === 403) {
                        //this.router.navigate(['login'], { queryParams: { returnUrl: location.pathname }});
                        return;
                    }
                }
            }));
    }
}