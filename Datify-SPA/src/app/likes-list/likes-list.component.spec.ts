/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LikesListComponent } from './likes-list.component';

describe('LikesListComponent', () => {
  let component: LikesListComponent;
  let fixture: ComponentFixture<LikesListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LikesListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LikesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
