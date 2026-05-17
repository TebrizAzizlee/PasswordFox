import {
  ChangeDetectionStrategy,
  Component,
  inject,
  ViewEncapsulation,
} from '@angular/core';
import Blank from '../../../components/blank/blank';
import { CreateBranchRequest } from '../../../Branch/data-access/models/CreateBranchRequest';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BranchService } from '../../../Branch/data-access/branch.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

@Component({
  imports: [Blank, ReactiveFormsModule],
  standalone: true,
  templateUrl: './create.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Create {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(BranchService);
  private readonly router = inject(Router);
  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    address: this.fb.group({
      city: ['', Validators.required],
      district: ['', Validators.required],
      fullAddress: ['', Validators.required],
    }),
    contactInfo: this.fb.group({
      phoneNumber1: ['', Validators.required],
      phoneNumber2: [''],
      email: ['', [Validators.required, Validators.email]],
    }),
    networkInfo: this.fb.group({
      wanIp: ['', Validators.required],
      subnet: ['', Validators.required],
      gateway: ['', Validators.required],
      dnsServer: ['', Validators.required],
    }),
  });
  loading = false;
  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    const request: CreateBranchRequest = {
      name: raw.name ?? '',

        city: raw.address?.city ?? '',
        district: raw.address?.district ?? '',
        fullAddress: raw.address?.fullAddress ?? '',
        phoneNumber1: raw.contactInfo?.phoneNumber1 ?? '',
        phoneNumber2: raw.contactInfo?.phoneNumber2 ?? '',
        email: raw.contactInfo?.email ?? '',


        wanIp: raw.networkInfo?.wanIp ?? '',
        subnet: raw.networkInfo?.subnet ?? '',
        gateway: raw.networkInfo?.gateway ?? '',
        dnsServer: raw.networkInfo?.dnsServer ?? '',

    };
    this.loading = true;
    this.service
      .createBranch(request)
      .pipe(
        finalize(() => {
          this.loading = false;
        }),
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/branches']);
        },
        error: (error) => {
          console.error('Branch create failed', error);

        },

      });
  }
}
