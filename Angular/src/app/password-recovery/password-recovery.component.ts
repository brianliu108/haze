import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-password-recovery',
  templateUrl: './password-recovery.component.html',
  styleUrls: ['./password-recovery.component.scss']
})
export class PasswordRecoveryComponent implements OnInit {

  constructor(
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
  }

  routeToLogin(){
    this.appComponent.navigate("");
  }

}
