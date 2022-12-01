import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApproveUserReviewsComponent } from './approve-user-reviews.component';

describe('ApproveUserReviewsComponent', () => {
  let component: ApproveUserReviewsComponent;
  let fixture: ComponentFixture<ApproveUserReviewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApproveUserReviewsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApproveUserReviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
