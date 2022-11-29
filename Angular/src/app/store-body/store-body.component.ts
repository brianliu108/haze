import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
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

  shownGames: Array<any> = [];
  wishlistGames: Array<any> = [];
  events: Array<any> = [];
  eventsGames: Array<any> = [];

  allGames: Array<any> = [];

  displayGames: boolean = false;
  displayWishList: boolean = false;
  displayEvents: boolean = false;
  displayOwnedGames: boolean = true;

  searchCtrl: FormControl = new FormControl(null);


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
    try {
      let getGamesCall = await axios.get(this.appComponent.apiHost + "/Products", this.requestInfo);

      if (getGamesCall.status == 200) {
        this.shownGames = JSON.parse(JSON.stringify(getGamesCall.data));
        this.allGames = getGamesCall.data;
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  routeToDetails(item: any) {
    localStorage.setItem('selectedGame', JSON.stringify(item));
    this.appComponent.navigate('/gameDetails');
  }

  routeToEventDetails(item: any) {
    localStorage.setItem('selectedEvent', JSON.stringify(item));
    this.appComponent.navigate('/eventDetails');
  }

  setDisplayWishList() {
    this.displayWishList = true;
    this.displayGames = false;
    this.displayEvents = false;
    this.displayOwnedGames = false;
  }

  setDisplayStore() {
    this.displayWishList = false;
    this.displayGames = true;
    this.displayEvents = false;
    this.displayOwnedGames = false;
  }

  setDisplayEvents() {
    this.displayWishList = false;
    this.displayGames = false;
    this.displayEvents = true;
    this.displayOwnedGames = false;
  }

  setOwnedGames(){
    this.displayWishList = false;
    this.displayGames = false;
    this.displayEvents = false;
    this.displayOwnedGames = true;
  }

  async getWishList() {
    try {
      let getWishlistGamesCall = await axios.get(this.appComponent.apiHost + "/WishList", this.requestInfo);

      if (getWishlistGamesCall.status == 200) {
        this.wishlistGames = getWishlistGamesCall.data;
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  async getEvents() {
    try {
      let getEventsGamesCall = await axios.get(this.appComponent.apiHost + "/Events", this.requestInfo);

      let test = getEventsGamesCall.data;

      if (getEventsGamesCall.status == 200) {
        this.events = getEventsGamesCall.data;
        //this.eventsGames = test[0].products;
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  showItem(item: any) {
    console.log(item);
  }

  filterGames() {
    this.shownGames = JSON.parse(JSON.stringify(this.allGames));
    if(this.searchCtrl.value.length != 0){
      for (let i = this.shownGames.length-1; i >= 0; i--) {
        if(!this.shownGames[i].productName.toLowerCase().includes(this.searchCtrl.value.toLowerCase())){
          this.shownGames.splice(i, 1);
        }
      }
    }
  }
}

