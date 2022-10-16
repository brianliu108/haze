import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import axios, { AxiosError } from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  siteKey: string;
  captchaComplete: boolean = false;
  errors:Array<any> = [];
  constructor(
    private appComponent: AppComponent
  ) {
    this.siteKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
  }

  ngOnInit(): void {
  }
  usernameCtrl: FormControl = new FormControl(null, Validators.required);
  emailCtrl: FormControl = new FormControl(null, [Validators.required, Validators.email]);
  passwdCtrl: FormControl = new FormControl(null, [Validators.pattern(/(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}/), Validators.required]);
  registerGroup: FormGroup = new FormGroup({ email: this.emailCtrl, passwd: this.passwdCtrl });
  
  async attemptCreate() {
    if (this.validateForm()) {
      try {
        let registerInfo = {
          "email": this.emailCtrl.value,
          "username": this.usernameCtrl.value,
          "password": this.passwdCtrl.value
        }
        const response = await axios.post('https://localhost:7105/Register', registerInfo);
        console.log(response.data)
        
        if (response.status = 200) {
          this.appComponent.navigate("");
        }
        this.appComponent.navigate("")
      } catch (error:any) {
        if (error.response.data.errors) {
          this.errors = error.response.data.errors;
          console.log(this.errors);
        }  
      } 
    }
  }

  resolved(captchaResponse: string) {
    console.log(`Resolved captcha with response: ${captchaResponse}`);
    this.captchaComplete = true;
  }

  validateForm() {
    if (this.captchaComplete && this.registerGroup.valid)
      return true;
    return false;
  }

  routeToLogin() {
    this.appComponent.navigate("");
  }

}
