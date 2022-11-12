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
  wishlistGames: Array<any> = [];
  events: Array<any> = [];

  displayGames: boolean = true;
  displayWishList: boolean = false;  
  displayEvents: boolean = false;

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
    this.getWishList();
    this.getEvents();
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

  routeToEventDetails(item: any){
    localStorage.setItem('selectedEvent', JSON.stringify(item));
    this.appComponent.navigate('/gameDetails');
  }

  setDisplayWishList(){
    this.displayWishList = true;
    this.displayGames = false;    
    this.displayEvents = false;
  }

  setDisplayStore(){
    this.displayWishList = false;
    this.displayGames = true;
    this.displayEvents = false;
  }

  setDisplayEvents(){
    this.displayWishList = false;
    this.displayGames = false;
    this.displayEvents = true;
  }

  async getWishList(){
    try{
      let getWishlistGamesCall = await axios.get(this.appComponent.apiHost + "/WishList",  this.requestInfo);

      if(getWishlistGamesCall.status == 200){
        this.wishlistGames = getWishlistGamesCall.data;
      }
    }
    catch(err: any){
      console.error(err);
    }
  }

  async getEvents(){
    try{
      let getEventsGamesCall = await axios.get(this.appComponent.apiHost + "/Events",  this.requestInfo);

      if(getEventsGamesCall.status == 200){
        this.events = getEventsGamesCall.data;
      }
    }
    catch(err: any){
      console.error(err);
    }
  }

  showItem(item: any){
    console.log(item);
  }
}
