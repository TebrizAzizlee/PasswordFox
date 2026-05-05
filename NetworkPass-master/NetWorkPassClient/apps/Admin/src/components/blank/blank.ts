import { DatePipe, Location, NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input, ViewEncapsulation } from '@angular/core';
import { EntityModel } from '../../models/entity-model';

@Component({
  selector:"app-blank",
  imports: [NgClass,DatePipe],
  templateUrl: './blank.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Blank {
  readonly pageIcon=input.required<string>();
readonly pageTitle=input.required<string>();
readonly pageDescription=input<string>();
readonly showStatus=input<boolean>(false);
readonly status=input<boolean>(true);
readonly showBackbtn=input<boolean>(true);
readonly #location=inject(Location);
readonly showEditbtn=input<boolean>(false);
readonly showAudit=input<boolean>(false);
readonly audit=input<EntityModel>();

goBack()
{
  this.#location.back();
}
}


