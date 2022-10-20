import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-web-banner',
  templateUrl: './web-banner.component.html',
  styleUrls: ['./web-banner.component.scss']
})
export class WebBannerComponent implements OnInit {

  constructor(
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
  }

  navigateProfile(){
    this.appComponent.navigate("profile");
  }

  navigateStore(){
    this.appComponent.navigate("store");
  }

  navigateLogin(){
    localStorage.clear();
    this.appComponent.navigate("");
  }

  navigatePreferences(){
    this.appComponent.navigate("preferences");
  }

}
