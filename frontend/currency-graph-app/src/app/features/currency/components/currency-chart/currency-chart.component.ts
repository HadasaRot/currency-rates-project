import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { CurrencyService } from "../../services/currency.service";
import { CurrencyRate } from "../../models/cur.model";
import { Chart } from "chart.js/auto";

@Component({
  selector: "app-currency-chart",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./currency-chart.component.html",
  styleUrl: "./currency-chart.component.css",
})
export class CurrencyChartComponent {
  data: CurrencyRate[] = [];
  chart: any;
  selectedRange: string = "week";
  summary: any[] = [];

  constructor(private currencyService: CurrencyService) {}

  ngOnInit() {
    this.loadWeek();
  }

  loadWeek() {
    this.selectedRange = "week";
    this.currencyService.getWeek().subscribe((res) => {
      this.data = res;
      this.calculateSummary();
      this.renderChart();
    });
  }

  loadMonth() {
    this.selectedRange = "month";
    this.currencyService.getMonth().subscribe((res) => {
      this.data = res;
      this.calculateSummary();
      this.renderChart();
    });
  }

  loadHalfYear() {
    this.selectedRange = "half";
    this.currencyService.getHalfYear().subscribe((res) => {
      this.data = res;
      this.calculateSummary();
      this.renderChart();
    });
  }

  loadYear() {
    this.selectedRange = "year";
    this.currencyService.getYear().subscribe((res) => {
      this.data = res;
      this.calculateSummary();
      this.renderChart();
    });
  }

  renderChart() {
    if (this.chart) {
      this.chart.destroy();
    }

    const labels = [
      ...new Set(this.data.map((x) => x.rateDate.substring(0, 10))),
    ].sort();

    const getValues = (code: string) =>
      labels.map((date) => {
        const record = this.data.find((x) =>
          x.currencyCode === code && x.rateDate.startsWith(date)
        );
        return record ? record.rate : null;
      });

    this.chart = new Chart("currencyChart", {
      type: "line",
      data: {
        labels: labels,
        datasets: [
          {
            label: "USD",
            data: getValues("USD"),
            borderColor: "blue",
            fill: false,
          },
          {
            label: "GBP",
            data: getValues("GBP"),
            borderColor: "green",
            fill: false,
          },
          {
            label: "CHF",
            data: getValues("CHF"),
            borderColor: "red",
            fill: false,
          },
          {
            label: "SEK",
            data: getValues("SEK"),
            borderColor: "orange",
            fill: false,
          },
        ],
      },
    });
  }

  calculateSummary() {
    const currencies = ["USD", "GBP", "CHF", "SEK"];

    this.summary = currencies
      .map((code) => {
        const items = this.data
          .filter((x) => x.currencyCode === code)
          .sort((a, b) => a.rateDate.localeCompare(b.rateDate));

        if (items.length === 0) return null;

        const first = items[0].rate;
        const last = items[items.length - 1].rate;

        const change = last - first;
        const percent = (change / first) * 100;

        return {
          code,
          last,
          change,
          percent,
        };
      })
      .filter((x) => x !== null);
  }

  getColor(code: string): string {
    const colors: any = {
      "USD": "blue",
      "GBP": "green",
      "CHF": "red",
      "SEK": "orange",
    };
    return colors[code] || "#ccc";
  }
}
