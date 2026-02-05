import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
})
export class Layout {
  menuItems = [
    { path: '/contracts', icon: 'description', label: 'Contracts' },
    { path: '/payment-plans', icon: 'payment', label: 'Payment Plans' },
    { path: '/change-requests', icon: 'swap_horiz', label: 'Change Requests' }
  ];
}
