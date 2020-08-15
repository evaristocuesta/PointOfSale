import { OnInit, Component } from '@angular/core';
import {ApiAuthService } from '../services/api-auth.service';
import { Router } from '@angular/router';

@Component({ templateUrl: 'login.component.html'})
export class LoginComponent implements OnInit {

    public username: string;
    public password: string;

    constructor(public apiAuthService: ApiAuthService, 
                private router: Router
        ) {
            if (this.apiAuthService.userData) {
                this.router.navigate(['/']);
            }
    }

    ngOnInit() {

    }

    login() {
        this.apiAuthService.login(this.username, this.password).subscribe(response => {
            if (response.success) {
                this.router.navigate(['/']);
            }
        })
    }
}