import { OnInit, Component } from '@angular/core';
import { ApiAuthService } from '../services/api-auth.service';
import { Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';

@Component({ templateUrl: 'login.component.html'})
export class LoginComponent implements OnInit {

    public loginForm = this.formBuilder.group({
        username: ['', Validators.required],
        password: ['', Validators.required]
    })

    constructor(public apiAuthService: ApiAuthService, 
                private router: Router,
                private formBuilder: FormBuilder ) {
            if (this.apiAuthService.userData) {
                this.router.navigate(['/']);
            }
    }

    ngOnInit() {

    }

    login() {
        console.log(this.loginForm.value);
        this.apiAuthService.login(this.loginForm.value).subscribe(response => {
            if (response.success) {
                this.router.navigate(['/']);
            }
        })
    }
}