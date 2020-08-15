import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { ApiAuthService } from '../services/api-auth.service';

@Injectable({ providedIn: 'root'})
export class AuthGuard implements CanActivate {
    
    constructor(private _router: Router,
                private _apiAuthService: ApiAuthService) {

    }

    canActivate(route: ActivatedRouteSnapshot) {
        const user = this._apiAuthService.userData;
        if (user) {
            return true;
        }
        else
        {
            this._router.navigate(['/login']);
            return false;
        }
    }
}