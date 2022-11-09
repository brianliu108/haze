import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreBodyComponent } from './store-body.component';

describe('StoreBodyComponent', () => {
  let component: StoreBodyComponent;
  let fixture: ComponentFixture<StoreBodyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StoreBodyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreBodyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
