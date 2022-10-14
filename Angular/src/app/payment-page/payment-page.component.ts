import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-payment-page',
  templateUrl: './payment-page.component.html',
  styleUrls: ['./payment-page.component.scss']
})
export class PaymentPageComponent implements OnInit {

  constructor(
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
  }

  navigateProfile(){
    this.appComponent.navigate('profile');
  }

  submitPaymentInfo(){
    
  }

}
