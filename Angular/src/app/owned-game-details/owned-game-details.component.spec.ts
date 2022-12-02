import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OwnedGameDetailsComponent } from './owned-game-details.component';

describe('OwnedGameDetailsComponent', () => {
  let component: OwnedGameDetailsComponent;
  let fixture: ComponentFixture<OwnedGameDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OwnedGameDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OwnedGameDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
