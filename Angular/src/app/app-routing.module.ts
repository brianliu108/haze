import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { PaymentPageComponent } from './payment-page/payment-page.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';
import { StoreComponent } from './store/store.component';

const routes: Routes = [
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: '',
    component: LoginComponent
  },
  {
    path: 'store',
    component: StoreComponent
  },
  {
    path: 'profile',
    component: ProfileComponent
  },
  {
    path: 'payment',
    component: PaymentPageComponent
  },
  {
    path: 'preferences',
    component: PreferencesComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
