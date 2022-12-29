import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { UserProfileService } from 'src/app/services/user-profile.service';

@Injectable({
  providedIn: 'root'
})
export class UserAnnouncmentsResolverService implements Resolve<any> {

  constructor(private compSvc: UserProfileService, private usrContext: UserContextServiceService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.compSvc.GetUserAnnouncements(this.usrContext.UserId).pipe(
      catchError(error => {
        return of('No data');
      })
    );
  }

}
