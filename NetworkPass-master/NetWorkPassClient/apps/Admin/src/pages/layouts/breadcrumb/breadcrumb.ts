import { NgClass } from '@angular/common';
import { Breadcrumbservice } from './../../../services/breadcrumbservice';
import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { RouterLink } from "@angular/router";




@Component({
  selector:"app-breadcrumb",
  imports: [RouterLink, NgClass],
  templateUrl: './breadcrumb.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Breadcrumb {
readonly breadcrumb=inject(Breadcrumbservice);
}
