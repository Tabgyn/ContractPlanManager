import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateChangeRequestDialog } from './create-change-request-dialog';

describe('CreateChangeRequestDialog', () => {
  let component: CreateChangeRequestDialog;
  let fixture: ComponentFixture<CreateChangeRequestDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateChangeRequestDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateChangeRequestDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
