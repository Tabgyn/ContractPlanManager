import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeRequestList } from './change-request-list';

describe('ChangeRequestList', () => {
  let component: ChangeRequestList;
  let fixture: ComponentFixture<ChangeRequestList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeRequestList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeRequestList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
