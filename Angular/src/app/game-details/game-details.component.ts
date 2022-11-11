import { Component, OnInit } from '@angular/core';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-game-details',
  templateUrl: './game-details.component.html',
  styleUrls: ['./game-details.component.scss']
})
export class GameDetailsComponent implements OnInit {

  errors: Array<any> = [];
  selectedGame: any;
  userData: any;

  deleted: boolean = false;

  private token: any;
  private requestInfo: any;

  constructor(
    private appComponent: AppComponent) { }

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    this.selectedGame = JSON.parse(localStorage.getItem('selectedGame')!);
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);

  }

  routeToStore(){
    this.appComponent.navigate('/store');
  }

  async deleteGame(){
    console.log(this.selectedGame.id);

    try {
      const deleteResponse = await axios.delete('https://localhost:7105/Product' + '/' + this.selectedGame.id, {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });

      if (deleteResponse.status == 200) {
        this.deleted = true;
        setTimeout(() => {
          this.appComponent.navigate('/store');
        }, 3000);
      }
    } catch (error) {
      console.error(error);
    }  
  }

  //async addToWish
}
