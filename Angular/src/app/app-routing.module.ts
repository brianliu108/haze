import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddressComponent } from './address/address.component';
import { CartCheckoutComponent } from './cart-checkout/cart-checkout.component';
import { CreateAdminComponent } from './create-admin/create-admin.component';
import { CreateEventsComponent } from './create-events/create-events.component';
import { CreateGamesComponent } from './create-games/create-games.component';
import { EventDetailsComponent } from './event-details/event-details.component';
import { GameDetailsComponent } from './game-details/game-details.component';
import { LoginComponent } from './login/login.component';
import { PasswordRecoveryComponent } from './password-recovery/password-recovery.component';
import { PaymentPageComponent } from './payment-page/payment-page.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';
import { ReportsComponent } from './reports/reports.component';
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
  },
  {
    path: 'password-recovery',
    component: PasswordRecoveryComponent
  },
  {
    path: 'address',
    component: AddressComponent
  },
  {
    path: 'createAdmin',
    component: CreateAdminComponent
  },
  {
    path: 'createGame',
    component: CreateGamesComponent
  },
  {
    path: 'gameDetails',
    component: GameDetailsComponent
  },
  {
    path: 'createEvent',
    component: CreateEventsComponent
  },
  {
    path: 'reports',
    component: ReportsComponent
  },
  {
    path: 'eventDetails',
    component: EventDetailsComponent
  },
  {
    path: 'checkout',
    component: CartCheckoutComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
