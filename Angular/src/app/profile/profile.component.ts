import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  checked: boolean = false;

  constructor(
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
  }

  submitProfileChanges(){

  }

  navigatePayment(){
    this.appComponent.navigate("payment");
  }
}
