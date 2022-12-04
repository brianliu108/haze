import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-event-details',
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent implements OnInit {
  private token: any;
  private requestInfo: any;

  selectedEventInfo: any;
  userInfo: any;
  isNotEditing: boolean = true;

  success: boolean = false;
  errors: Array<any> = [];
  gamesList: Array<any> = [];

  allAdded: boolean = false;

  startDateCtrl: FormControl = new FormControl(null, Validators.required);
  endDateCtrl: FormControl = new FormControl(null, Validators.required);
  eventNameCtrl: FormControl = new FormControl(null, Validators.required);
  includedGames: Array<any> = [];
  eventGroup: FormGroup = new FormGroup({
    startDate: this.startDateCtrl,
    endDate: this.endDateCtrl,
    eventName: this.eventNameCtrl
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
    this.selectedEventInfo = JSON.parse(localStorage.getItem('selectedEvent')!);
    this.setInputs();
    this.userInfo = JSON.parse(localStorage.getItem('currentUser')!);
  }

  async getGames() {
    try {
      let getGamesCall = await axios.get(this.appComponent.apiHost + "/Products", this.requestInfo);

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
    if (this.allAdded == false) {
      this.allAdded = true;
      this.addAllGames();
    }
    else {
      this.allAdded = false;
      this.includedGames = [];
    }
  }

  addAllGames() {
    this.includedGames = this.gamesList.slice(0);
    console.log(this.includedGames);
  }

  addGame(item: any) {
    if (!this.includedGames.includes(item)) {
      this.includedGames.push(item);
    }
    else if (this.includedGames.includes(item)) {
      let spliceIndex: number = 0;
      for (let i: number = 0; i < this.includedGames.length; i++) {
        if (this.includedGames[i] == item) {
          spliceIndex = i;
        }
      }
      this.includedGames.splice(spliceIndex, 1);
    }
    console.log(this.includedGames);
  }

  routeToStore() {
    this.appComponent.navigate("/store");
  }

  async createEvent() {
    if (this.eventGroup.valid && this.includedGames.length != 0) {
      let includedGamesCallList: Array<any> = [];
      for (let item of this.includedGames) {
        includedGamesCallList.push(item.id);
      }

      console.log(this.startDateCtrl.value);

      let eventInfo = {
        startDate: this.startDateCtrl.value,
        endDate: this.endDateCtrl.value,
        eventName: this.eventNameCtrl.value,
        productIds: includedGamesCallList
      };

      try {
        const createResponse = await axios.post('https://localhost:7105/Event', eventInfo, this.requestInfo);

        if (createResponse.status = 200) {
          console.log(createResponse);
          this.showSuccess();
          setTimeout(() => {
            this.routeToStore();
          }, 3000);
        }
      }
      catch (error: any) {
        this.errors.push(error);
        console.error(error);
      }
    }
    else {
      this.errors.push("Must select games for event!");
    }
  }

  showSuccess() {
    this.success = true;
  }

  setInputs() {
    this.eventNameCtrl.setValue(this.selectedEventInfo.eventName);
    this.startDateCtrl.setValue(this.selectedEventInfo.startDate);
    this.endDateCtrl.setValue(this.selectedEventInfo.endDate);
  }

  async registerForEvent() {
    this.errors = [];
    try {
      console.log(this.requestInfo);

      const registerResponse = await axios.post('https://localhost:7105/RegisterForEvent/' + this.selectedEventInfo.id, null, this.requestInfo);

      if (registerResponse.status = 200) {
        console.log(registerResponse);
        this.showSuccess();
        setTimeout(() => {
          this.routeToStore();
        }, 3000);
      }
    }
    catch (error: any) {
      this.errors.push(error);
      console.error(error);
    }
  }
}
