import { Component, OnInit } from '@angular/core';
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

  attemptCreate(){
    if (this.validateForm())
      this.appComponent.navigate("");
  }

  resolved(captchaResponse: string) {
    console.log(`Resolved captcha with response: ${captchaResponse}`);
    this.captchaComplete = true;
  }

  validateForm() {
    if (this.captchaComplete)
      return true;
    return false;
  }
}
