import { Component, EventEmitter, inject, input, Input, OnInit, Output, output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router, RouteReuseStrategy } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    JsonPipe,
    NgIf,
    TextInputComponent,
    DatePickerComponent,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private router = inject(Router);
  private accountService = inject(AccountService);


  cancelRegister = output<boolean>();
  
  registerForm: FormGroup = new FormGroup({});

  maxDate = new Date();

  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeForm();

    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      gender: new FormControl('male'),
      username: new FormControl('', Validators.required),
      knownAs: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),

      password: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(8),
      ]),
      confirmPassword: new FormControl('', [
        Validators.required,
        this.matchValues('password'),
      ]),
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value == control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }

  register() {
    const dob = this.getdate(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth:dob});
    this.accountService.register(this.registerForm.value).subscribe({
      next: (_) => {
        this.router.navigateByUrl('/members');
      },
      error: (err) => (this.validationErrors = err),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getdate(dob: string | undefined) {
    if (!dob) {
      return;
    }

    return new Date(dob).toISOString().slice(0,10);
  }
}
