import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import axios from 'axios';
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

  makeCall() {

  }

  async logonCall(logonInfo: object) {
    try {
      const response = await axios.post('https://localhost:7105/Login', logonInfo);
      return (response);
    } catch (error) {
      console.error(error);
      return (error);
    }
  }

  routeToRegister() {
    this.appComponent.navigate("register");
  }

  async attemptLogin() {
    if (this.loginGroup.valid) {
      let logonInfo: object = {
        "Username": this.usernameCtrl.value,
        "Password": this.passwdCtrl.value,
        headers: {
          "Access-Control-Allow-Origin": "*"
        }
      }
      let response: any = await this.logonCall(logonInfo);
      let responseData: any = response.data;
      console.log(responseData);

      if (response.status == 200) {
        let currentUser = {
          username: this.usernameCtrl.value,
          password: this.passwdCtrl.value,
          token: responseData

        }

        localStorage.setItem("currentUser", JSON.stringify(currentUser));
        this.appComponent.navigate("store");
      }
    }
  }

  routeToPasswordRecovery() {
    this.appComponent.navigate("password-recovery");
  }


}
