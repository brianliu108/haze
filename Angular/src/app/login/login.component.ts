import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(
    private router: Router,
    private appComponent: AppComponent
  ) { }

  usernameCtrl: FormControl = new FormControl(null, Validators.required);
  passwdCtrl: FormControl = new FormControl(null, Validators.required);
  loginGroup: FormGroup = new FormGroup({
    username: this.usernameCtrl,
    passwd: this.passwdCtrl
  });

  ngOnInit(): void {
  }

  routeToRegister(){
    this.appComponent.navigate("register");
  }

  attemptLogin(){
    this.appComponent.navigate("store");
  }
}
