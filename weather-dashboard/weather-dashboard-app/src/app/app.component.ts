import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { WeatherService } from './weather.service'; 
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { GridOptions } from 'ag-grid-community';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [FormsModule, CommonModule, HighchartsChartModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'weather-dashboard';
  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {}; 
  gridOptions: GridOptions = {};
  rowData: any[] = [];
  startDate: string = new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0];

  constructor(private weatherService: WeatherService, private http: HttpClient) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(): void {
    this.weatherService.getWeatherData(this.startDate).subscribe(data => {
      this.processData(data.items);
    });
  }

  processData(items: any[]): void {
    // Process data for chart and grid
    const processedData = items.map(item => ({
      date: item.timestamp,
      temperature: item.readings[0].value,
      humidity: item.readings[1].value
    }));

    this.rowData = processedData;
    this.setupChart([
      { date: '2025-04-16', temperature: 30.2, humidity: 60 },
      { date: '2025-04-17', temperature: 31.1, humidity: 58 },
      { date: '2025-04-18', temperature: 29.5, humidity: 62 }
    ]);
    // this.setupGrid();
  }

  setupChart(data: any[]): void {
    this.chartOptions = {
      title: { text: 'Temperature & Humidity Trend' },
      xAxis: { categories: data.map(d => d.date) },
      series: [
        { name: 'Temperature', type: 'line', data: data.map(d => d.temperature) },
        { name: 'Humidity', type: 'line', data: data.map(d => d.humidity), yAxis: 1 }
      ],
      yAxis: [
        { title: { text: 'Temperature (°C)' } },
        { title: { text: 'Humidity (%)' }, opposite: true }
      ]
    };
  }

  setupGrid(): void {
    this.gridOptions = {
      columnDefs: [
        { headerName: 'Date', field: 'date', sortable: true },
        { headerName: 'Temperature (°C)', field: 'temperature', sortable: true },
        { headerName: 'Humidity (%)', field: 'humidity', sortable: true }
      ],
      rowData: this.rowData
    };
  }

  refreshData(): void {
    this.loadData();
  }
}
