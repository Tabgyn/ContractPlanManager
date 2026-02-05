import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeRequestDialog } from './change-request-dialog';

describe('ChangeRequestDialog', () => {
  let component: ChangeRequestDialog;
  let fixture: ComponentFixture<ChangeRequestDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeRequestDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeRequestDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
