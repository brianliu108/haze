import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  siteKey: string;
  captchaComplete: boolean = false;
  constructor(
    private appComponent: AppComponent
  ) {
    this.siteKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
  }

  ngOnInit(): void {
  }
  usernameCtrl: FormControl = new FormControl(null, Validators.required);
  emailCtrl: FormControl = new FormControl(null, Validators.required);
  passwdCtrl: FormControl = new FormControl(null, [Validators.pattern(/(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}/), Validators.required]);
  registerGroup: FormGroup = new FormGroup({ email: this.emailCtrl, passwd: this.passwdCtrl });
  
  attemptCreate() {
    if (this.validateForm())
      this.appComponent.navigate("");
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
}
