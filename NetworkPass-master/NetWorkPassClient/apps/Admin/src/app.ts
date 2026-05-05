import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from './AuthServices/authservice';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone:true,
  imports:[RouterOutlet],
  template: '<router-outlet></router-outlet>'
})
export class App implements OnInit {

  protected title = 'Admin';

  private auth = inject(AuthService);

  ngOnInit(): void {
    this.auth.fetchMe().subscribe({
      error: () => {
        // 🔥 401 normaldır → ignore et
      }
    });
  }
}