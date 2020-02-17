import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { ProfileComponent } from '../user/profile/profile.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<ProfileComponent> {
  canDeactivate(component: ProfileComponent) {
    if (component.editForm.dirty) {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost!');
    }
    return true;
  }
}
