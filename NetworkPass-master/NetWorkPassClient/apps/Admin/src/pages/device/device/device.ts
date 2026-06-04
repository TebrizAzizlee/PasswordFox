import { ChangeDetectionStrategy, Component, computed, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { DeviceService } from '../../../Device/data-access/device.service';
import { DeviceStore } from '../../../Device/data-access/device.store';
import { CommonModule, DatePipe } from '@angular/common';
import Blank from '../../../components/blank/blank';
import { FormsModule } from '@angular/forms';

@Component({
  imports: [Blank,DatePipe,CommonModule,FormsModule],
  templateUrl: './device.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Device implements OnInit {

  private readonly service =
    inject(DeviceService);

  readonly devices =
    inject(DeviceStore).devices;

  readonly totalCount =
    inject(DeviceStore).totalCount;
    getDeviceType(type: number): string
    { switch (type)
      { case 1: return 'Router';
        case 2: return 'Switch';
         case 3: return 'Access Point';
         case 4: return 'Firewall';
         case 5: return 'Server';
         case 6: return 'Printer';
         case 7: return 'Camera';
         case 8: return 'FingerPrint';
         default: return 'Unknown'; }
        }


         getStatusText(status: number): string
         {
          switch (status)
          { case 2: return 'Online';
            case 3: return 'Offline';
             case 5: return 'Degraded';
             default: return 'Unknown'; }

            }
readonly totalDevices = computed(
  () => this.devices().length
);

readonly onlineDevices = computed(
  () =>
    this.devices()
      .filter(x => x.status )
      .length
);

readonly offlineDevices = computed(
  () =>
    this.devices()
      .filter(x => x.status)
      .length
);

readonly degradedDevices = computed(
  () =>
    this.devices()
      .filter(x => x.status)
      .length
);
 searchText = '';


  ngOnInit(): void {

    this.service.loadDevices();
  }
}
