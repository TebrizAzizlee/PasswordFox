import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../AuthServices/authservice';


@Component({
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.html'
})
export class LoginComponent {
 
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  isLoading = false;
  requiresTfa = false;

  // 🔥 TFA code
  tfaCodeArray: string[] = ['', '', '', '', '', ''];

  form = this.fb.group({
    loginIdentifier: ['', Validators.required],
    password: ['', Validators.required]
  });

  submit() {
    if (this.form.invalid) return;

    this.isLoading = true;

    const { loginIdentifier, password } = this.form.value;

    // 🔥 TFA mərhələsi
    if (this.requiresTfa) {
      const tfaCode = this.tfaCodeArray.join('');

      this.auth.loginWithTfa({
        userName: loginIdentifier!,
        tfaCode: tfaCode
      }).subscribe({
        next: () => this.afterLogin(),
        error: () => this.isLoading = false
      });

      return;
    }

    // 🔥 normal login
    this.auth.login({
      loginIdentifier: loginIdentifier!,
      password: password!
    }).subscribe({
      next: (res: any) => {
        if (res.requiresTfa) {
          this.requiresTfa = true;
          this.isLoading = false;
        } else {
          this.afterLogin();
        }
      },
      error: () => this.isLoading = false
    });
  }

  private afterLogin() {
    
    this.auth.fetchMe().subscribe(() => {
      this.router.navigate(['/dashboard'])
    });
  }

  // 🔥 TFA input handling
  onTfaInput(event: any, index: number) {
    const value = event.target.value;

    if (!value) return;

    this.tfaCodeArray[index] = value;

    const next = event.target.nextElementSibling;
    if (next) next.focus();
  }
}