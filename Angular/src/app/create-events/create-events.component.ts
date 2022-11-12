import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-create-events',
  templateUrl: './create-events.component.html',
  styleUrls: ['./create-events.component.scss']
})
export class CreateEventsComponent implements OnInit {

  private token: any;
  private requestInfo: any;

  success: boolean = false;
  errors: Array<any> = [];
  gamesList: Array<any> = [];

  allAdded: boolean = false;

  startDateCtrl: FormControl = new FormControl(null, Validators.required);
  endDateCtrl: FormControl = new FormControl(null, Validators.required);
  eventNameCtrl: FormControl = new FormControl(null, Validators.required);
  includedGames: Array<any> = [];
  eventGroup: FormGroup = new FormGroup({

  });

  constructor(
    private appComponent: AppComponent
  ) { }

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
  }

  async getGames() {
    try {
      let getGamesCall = await axios.get(this.appComponent.apiHost + "/Products", this.requestInfo);
      console.log(getGamesCall);

      if (getGamesCall.status == 200) {
        this.gamesList = getGamesCall.data;
      }
    }
    catch (err: any) {
      this.errors.push(err);
      console.error(err);
    }
  }

  addAll() {
    if (this.allAdded == false)
      this.allAdded = true;
    else
      this.allAdded = false;
  }

  addAllGames() {
    this.includedGames = this.gamesList;
    console.log(this.includedGames);

  }

  addGame(item: any) {
    if (this.allAdded == false && !this.includedGames.includes) {
      this.includedGames.push(item);

    }
    console.log(this.includedGames);
  }

  routeToStore() {
    this.appComponent.navigate("/store");
  }
}
