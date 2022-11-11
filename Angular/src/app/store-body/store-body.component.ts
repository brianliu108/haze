import { Component, OnInit } from '@angular/core';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-store-body',
  templateUrl: './store-body.component.html',
  styleUrls: ['./store-body.component.scss']
})
export class StoreBodyComponent implements OnInit {

  private token: any;
  private requestInfo: any;

  games: Array<any> = [];

  constructor(
    private appComponent: AppComponent
  ) { }

  userData: any;

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    this.getGames();
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);
  }

  async getGames() {
    try{
      let getGamesCall = await axios.get(this.appComponent.apiHost + "/Products",  this.requestInfo);
      console.log(getGamesCall);

      if(getGamesCall.status == 200){
        this.games = getGamesCall.data;
      }
    }
    catch(err: any){
      console.error(err);
    }
  }

  routeToDetails(item: any){
    localStorage.setItem('selectedGame', JSON.stringify(item));
    this.appComponent.navigate('/gameDetails');
  }
}
