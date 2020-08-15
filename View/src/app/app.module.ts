import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSidenavModule } from '@angular/material/sidenav';
import { HomeComponent } from './home/home.component';
import { ClientComponent } from './client/client.component';
import { ClientDialogComponent } from './client/clientDialog/clientDialog.component';
import { DialogDeleteComponent } from './common/delete/dialogdelete.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { MainNavComponent } from './main-nav/main-nav.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { LoginComponent } from './login/login.component';
import { JwtInterceptor } from './security/jwt-interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ClientComponent, 
    ClientDialogComponent, 
    DialogDeleteComponent, 
    MainNavComponent, 
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule, 
    MatSidenavModule, 
    HttpClientModule, 
    MatTableModule, 
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule, 
    FormsModule, 
    LayoutModule, 
    MatToolbarModule, 
    MatIconModule, 
    MatListModule, 
    MatGridListModule, 
    MatCardModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
