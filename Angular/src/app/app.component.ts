import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Haze';
  readonly apiHost = "https://localhost:7105"
  constructor(
    public router: Router
  ){ }

  public navigate(name: string){
    this.router.navigate(["/" + name]);
  }
  
}
