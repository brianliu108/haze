import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { ReactiveFormsModule } from '@angular/forms';
import { matTabsAnimations, MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule, MatExpansionPanel } from '@angular/material/expansion';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import {MatCheckboxModule} from '@angular/material/checkbox'; 
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatSelectModule} from '@angular/material/select';  

import { MatMenu, MatMenuModule } from '@angular/material/menu';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { StoreComponent } from './store/store.component';
import {MatListModule} from '@angular/material/list';
import { ProfileComponent } from './profile/profile.component';
import { WebBannerComponent } from './web-banner/web-banner.component';
import { PaymentPageComponent } from './payment-page/payment-page.component';
import { RecaptchaModule, RecaptchaFormsModule } from 'ng-recaptcha';
import { PreferencesComponent } from './preferences/preferences.component';
import { PasswordRecoveryComponent } from './password-recovery/password-recovery.component';
import { AddressComponent } from './address/address.component';
import { StoreBodyComponent } from './store-body/store-body.component';
import { CreateAdminComponent } from './create-admin/create-admin.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    StoreComponent,
    ProfileComponent,
    WebBannerComponent,
    PaymentPageComponent,
    PreferencesComponent,
    PasswordRecoveryComponent,
    AddressComponent,
    StoreBodyComponent,
    CreateAdminComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatInputModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatMenuModule,
    MatIconModule,
    MatExpansionModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    RecaptchaFormsModule,
    RecaptchaModule,
    MatSidenavModule,
    MatListModule,
    MatToolbarModule,
    MatSelectModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
