import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Root component. It only renders the page that matches the current URL
 * (<router-outlet> works like <Outlet /> in React Router).
 */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
})
export class App {}
