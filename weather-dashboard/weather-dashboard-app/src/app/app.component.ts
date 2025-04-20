import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser, DatePipe } from '@angular/common';
import { WeatherService } from './weather.service'; 
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { HighchartsChartModule } from 'highcharts-angular';
import { AgGridAngular } from 'ag-grid-angular';
import { GridOptions } from 'ag-grid-community'; 

// Define an interface for Weather data
interface WeatherData {
  date: string;
  temperature: number;
  humidity: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, CommonModule, HighchartsChartModule, AgGridAngular],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [DatePipe]
})
export class AppComponent implements OnInit {
  title = 'weather-dashboard';
  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {}; 
  rowData: any[] = [];
  startDate: string = new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0];
  isBrowser: boolean;
  isLoading = false;
  gridOptions: GridOptions = {};

  constructor(
    private weatherService: WeatherService, 
    private http: HttpClient, 
    @Inject(PLATFORM_ID) private platformId: Object,
    private datePipe: DatePipe
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit() {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;  // Show the spinner before making the request
    this.weatherService.getWeatherData(this.startDate).subscribe({
      next: (data) => {
        this.processData(data.items);
        this.isLoading = false;  // Hide the spinner after data is loaded
      },
      error: (error) => {
        console.error('Error loading weather data:', error);
        this.isLoading = false;  // Hide the spinner in case of error
      }
    });
  }

  processData(items: any[]): void {
    // Process data for chart and grid
    const processedData = items.map(item => ({
      date: this.datePipe.transform(item.timestamp, 'dd/MM/yyyy HH:mm:ss'),
      temperature: item.readings?.[0]?.value ?? 0,
      humidity: item.readings?.[1]?.value ?? 0
    }));

    this.rowData = processedData;
    this.setupChart(processedData);
    this.setupGrid();
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
        { headerName: 'Date', field: 'date', sortable: true, filter: true, flex: 1 },
        { headerName: 'Temperature (°C)', field: 'temperature', sortable: true, filter: true, flex: 1 },
        { headerName: 'Humidity (%)', field: 'humidity', sortable: true, filter: true, flex: 1 }
      ],
      rowData: this.rowData,
      pagination: true,
      paginationPageSize: 20,
      defaultColDef: {
        resizable: true
      },
      domLayout: 'autoHeight'
    };
  }

  refreshData(): void {
    this.loadData();
  }
}