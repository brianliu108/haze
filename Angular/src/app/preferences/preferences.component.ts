import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import axios from 'axios';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.scss']
})
export class PreferencesComponent implements OnInit {
  preferences:any = {
    categoryIds: [],
    platformIds: []
  }; 
  cat1: FormControl = new FormControl();
  cat2: FormControl = new FormControl();
  cat3: FormControl = new FormControl();
  cat4: FormControl = new FormControl();
  cat5: FormControl = new FormControl();
  cat6: FormControl = new FormControl();
  cat7: FormControl = new FormControl();
  plat1: FormControl = new FormControl();
  plat2: FormControl = new FormControl();
  plat3: FormControl = new FormControl();
  plat4: FormControl = new FormControl();
  plat5: FormControl = new FormControl();
  categories:Array<FormControl> = [this.cat1, this.cat2, this.cat3, this.cat4, this.cat5, this.cat6, this.cat7]
  platforms:Array<FormControl> = [this.plat1, this.plat2, this.plat3, this.plat4, this.plat5]
  errors = []
  constructor() { }

  validSelection: boolean = true;
  
  preferencesGroup: FormGroup = new FormGroup({

  });

  ngOnInit(): void {
    let token:any = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    let requestInfo: object = {
      headers: {
        Authorization: "Bearer " + token
      }
    };

    axios.get('https://localhost:7105/UserPreferences', requestInfo).then((res) => {
      this.preferences = res.data;
      console.log(this.preferences);
      
    }).catch((error) => {

    });
      
    // (async () => {
      
    // });
  }

  submitPreferenceChanges(){
    
  }

}
