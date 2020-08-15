import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Response } from '../models/response'
import { User } from '../models/user'
import { map } from 'rxjs/operators';
import { Login } from '../models/login';

const httpOption = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json'
    })
}

@Injectable({
    providedIn: 'root'
})
export class ApiAuthService {
    url: string = 'https://localhost:44395/api/auth/login';

    private _userSubject: BehaviorSubject<User>;

    public get userData(): User {
        return this._userSubject.value;
    }

    constructor( private _http: HttpClient ){
        this._userSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('user')));
    }

    login(login: Login): Observable<Response> {
        return this._http.post<Response>(this.url, login, httpOption)
            .pipe(map(res => {
                if (res.success) {
                    const user: User = res.data;
                    localStorage.setItem('user', JSON.stringify(user));
                    this._userSubject.next(user);
                }
                return res;
            }));
    }

    logout() {
        localStorage.removeItem('user');
        this._userSubject.next(null);
    }
}