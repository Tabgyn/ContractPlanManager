import { Component, signal } from '@angular/core';
import { Layout } from './shared/components/layout/layout';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [Layout],
  template: '<app-layout></app-layout>',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('Contract Plan Manager');
}
