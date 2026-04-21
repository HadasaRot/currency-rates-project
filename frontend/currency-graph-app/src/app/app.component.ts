import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CurrencyChartComponent } from './features/currency/components/currency-chart/currency-chart.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,CurrencyChartComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'currency-graph-app';
}
