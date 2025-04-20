import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { WeatherService } from './weather.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DatePipe } from '@angular/common';
import { of, throwError } from 'rxjs';
import { PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HighchartsChartModule } from 'highcharts-angular';
import { AgGridAngular } from 'ag-grid-angular';
import * as Highcharts from 'highcharts';
import { ColDef } from 'ag-grid-community';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let weatherServiceSpy: jasmine.SpyObj<WeatherService>;

  const mockWeatherData = {
    items: [
      {
        timestamp: '2023-05-01T12:00:00',
        readings: [
          { value: 25.5 }, // temperature
          { value: 65 }    // humidity
        ]
      },
      {
        timestamp: '2023-05-02T12:00:00',
        readings: [
          { value: 27.0 },
          { value: 70 }
        ]
      }
    ]
  };

  beforeEach(async () => {
    weatherServiceSpy = jasmine.createSpyObj('WeatherService', ['getWeatherData']);
    
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        FormsModule,
        HighchartsChartModule,
        AgGridAngular
      ],
      providers: [
        { provide: WeatherService, useValue: weatherServiceSpy },
        DatePipe,
        { provide: PLATFORM_ID, useValue: 'browser' }
      ]
    }).compileComponents();
    
    weatherServiceSpy.getWeatherData.and.returnValue(of(mockWeatherData));
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should have correct title', () => {
    expect(component.title).toEqual('weather-dashboard');
  });

  it('should set startDate to 30 days ago', () => {
    const today = new Date();
    const thirtyDaysAgo = new Date(today.setDate(today.getDate() - 30)).toISOString().split('T')[0];
    expect(component.startDate).toEqual(thirtyDaysAgo);
  });

  it('should initialize Highcharts', () => {
    expect(component.Highcharts).toEqual(Highcharts);
  });

  it('should load weather data on initialization', fakeAsync(() => {
    component.ngOnInit();
    tick();
    
    expect(weatherServiceSpy.getWeatherData).toHaveBeenCalledWith(component.startDate);
    expect(component.rowData.length).toBe(2);
    expect(component.isLoading).toBeFalse();
  }));

  it('should process weather data correctly', () => {
    const datePipeSpy = spyOn(component['datePipe'], 'transform').and.returnValues(
      '01/05/2023 12:00:00',
      '02/05/2023 12:00:00'
    );
    
    component.processData(mockWeatherData.items);
    
    expect(component.rowData.length).toBe(2);
    expect(component.rowData[0].temperature).toBe(25.5);
    expect(component.rowData[0].humidity).toBe(65);
    expect(component.rowData[1].temperature).toBe(27.0);
    expect(component.rowData[1].humidity).toBe(70);
  });

  it('should handle data with missing readings', () => {
    const incompleteData = [
      {
        timestamp: '2023-05-03T12:00:00',
        readings: null
      },
      {
        timestamp: '2023-05-04T12:00:00',
        readings: []
      }
    ];
    
    component.processData(incompleteData);
    
    expect(component.rowData.length).toBe(2);
    expect(component.rowData[0].temperature).toBe(0);
    expect(component.rowData[0].humidity).toBe(0);
  });

  it('should setup chart options correctly', () => {
    const mockData = [
      { date: '01/05/2023', temperature: 25, humidity: 65 },
      { date: '02/05/2023', temperature: 27, humidity: 70 }
    ];
    
    component.setupChart(mockData);
    
    expect(component.chartOptions.title?.text).toBe('Temperature & Humidity Trend');
    
    // Safe type checking for xAxis categories
    const xAxis = component.chartOptions.xAxis as Highcharts.XAxisOptions;
    expect(xAxis?.categories).toEqual(['01/05/2023', '02/05/2023']);
    
    // Safe type checking for series data
    const series = component.chartOptions.series as any[];
    expect(series[0].data).toEqual([25, 27]);
    expect(series[1].data).toEqual([65, 70]);
  });

  it('should setup grid options correctly', () => {
    component.rowData = [
      { date: '01/05/2023', temperature: 25, humidity: 65 }
    ];
    
    component.setupGrid();
    
    const columnDefs = component.gridOptions.columnDefs as ColDef[];
    expect(columnDefs?.length).toBe(3);
    
    // Safe type checking for columnDefs field property
    expect((columnDefs[0] as any).field).toBe('date');
    expect((columnDefs[1] as any).field).toBe('temperature');
    expect((columnDefs[2] as any).field).toBe('humidity');
    
    expect(component.gridOptions.pagination).toBeTrue();
    expect(component.gridOptions.paginationPageSize).toBe(20);
  });

  it('should refresh data when refreshData is called', () => {
    const loadDataSpy = spyOn(component, 'loadData');
    component.refreshData();
    expect(loadDataSpy).toHaveBeenCalled();
  });

  it('should handle errors when loading weather data', fakeAsync(() => {
    weatherServiceSpy.getWeatherData.and.returnValue(throwError(() => new Error('Test error')));
    const consoleSpy = spyOn(console, 'error');
    
    component.loadData();
    tick();
    
    expect(consoleSpy).toHaveBeenCalledWith('Error loading weather data:', jasmine.any(Error));
    expect(component.isLoading).toBeFalse();
  }));
});