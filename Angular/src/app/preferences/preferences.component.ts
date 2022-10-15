import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.scss']
})
export class PreferencesComponent implements OnInit {

  constructor() { }

  validSelection: boolean = true;

  
  preferencesGroup: FormGroup = new FormGroup({

  });

  ngOnInit(): void {
  }



  submitPreferenceChanges(){
    
  }

}
