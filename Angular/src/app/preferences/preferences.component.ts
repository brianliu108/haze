import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.scss']
})
export class PreferencesComponent implements OnInit {
  userPreferences: any = {
    categoryIds: [],
    platformIds: []
  };
  categories: Array<any> = [];
  platforms: Array<any> = [];
  errors = [false, false, false]

  private token:any;
  private requestInfo:any;
  constructor(private appComponent: AppComponent) { }

  validSelection: boolean = true;

  preferencesGroup: FormGroup = new FormGroup({

  });

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    try {
      let categoriesResponse = await axios.get('https://localhost:7105/Categories', this.requestInfo);
      let platformsResponse = await axios.get('https://localhost:7105/Platforms', this.requestInfo);
      let userPreferencesResponse = await axios.get('https://localhost:7105/UserPreferences', this.requestInfo);

      this.categories = categoriesResponse.data;
      this.platforms = platformsResponse.data;
      this.userPreferences = userPreferencesResponse.data;

      for (let categoryId of this.userPreferences.categoryIds) {
        let category: any = this.categories.find(x => x.id == categoryId);
        if (category) {
          category.checked = true;
        }
      }
      for (let platformId of this.userPreferences.platformIds) {
        let platform: any = this.platforms.find(x => x.id == platformId);
        if (platform) {
          platform.checked = true;
        }
      }
    } catch (e) {
      console.log(e);
      if(this.token == undefined)
        this.appComponent.navigate("")
    }

  }

  onCheckboxChange(id: number, isCategory: boolean) {
    let userPreference: any;
    if (isCategory) {
      userPreference = this.categories.find(x => x.id == id);
    } else {
      userPreference = this.platforms.find(x => x.id == id);
    }
    userPreference.checked = !userPreference.checked;

  }

  async submitPreferenceChanges() {
    // validate
    if (this.categories.filter(x => x.checked).length > 3) 
      this.errors[0] = true;
    else 
      this.errors[0] = false;

    if (this.platforms.filter(x => x.checked).length > 2) 
      this.errors[1] = true;
    else
      this.errors[1] = false;    

    this.errors[2] = false;
    if(!this.errors.find(x => x == true)) {
      // no errors
      let checkedCategories = this.categories.filter(x => x.checked);
      let checkedPlatforms = this.platforms.filter(x => x.checked);
      this.userPreferences.categoryIds = [];
      this.userPreferences.platformIds = [];
      for (let checkedCategory of checkedCategories) {        
        this.userPreferences.categoryIds.push(checkedCategory.id);
      }
      for (let checkedPlatform of checkedPlatforms) {        
        this.userPreferences.platformIds.push(checkedPlatform.id);
      }

      try {
        let requestPayload:any = {          
          "categoryIds": this.userPreferences.categoryIds,
          "platformIds": this.userPreferences.platformIds          
        }

        console.log(this.token);
        console.log(JSON.stringify(requestPayload));
        console.log(await axios.patch('https://localhost:7105/UserPreferences', requestPayload, this.requestInfo));
        this.appComponent.navigate("store");
      } catch (e) {
        console.log(e);
        this.errors[2] = true;     
      }
      
    }      
  }

}
