import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactionIconComponent } from './reaction-icon.component';

describe('ReactionIconComponent', () => {
  let component: ReactionIconComponent;
  let fixture: ComponentFixture<ReactionIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactionIconComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ReactionIconComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
