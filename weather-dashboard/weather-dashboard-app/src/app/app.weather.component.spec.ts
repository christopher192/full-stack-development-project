import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';  // ✅ Standalone component
import { WeatherService } from './weather.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AgGridModule } from 'ag-grid-angular';
import { HighchartsChartModule } from 'highcharts-angular';
import { of } from 'rxjs';

class MockWeatherService {
  getWeatherData() {
    return of({
      items: [
        {
          timestamp: 1617552000000,
          readings: [
            { value: 25 }, // temperature
            { value: 60 }  // humidity
          ]
        }
      ]
    });
  }
}

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let weatherService: WeatherService;

beforeEach(async () => {
  await TestBed.configureTestingModule({
    imports: [
      AppComponent,
      HttpClientTestingModule,
      CommonModule,
      FormsModule,
      HighchartsChartModule,
      AgGridModule
    ]
  })
  .overrideComponent(AppComponent, {
    set: {
      providers: [
        DatePipe,
        { provide: WeatherService, useClass: MockWeatherService }
      ]
    }
  }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    weatherService = TestBed.inject(WeatherService);
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should call loadData on initialization', () => {
    const spy = spyOn(component, 'loadData').and.callThrough();
    component.ngOnInit();
    expect(spy).toHaveBeenCalled();
  });

  it('should process data correctly', () => {
    const spy = spyOn(component, 'processData').and.callThrough();
    component.loadData();  // Uses mock service
    expect(spy).toHaveBeenCalled();
    expect(component.rowData.length).toBe(1);
    expect(component.rowData[0].temperature).toBe(25);
    expect(component.rowData[0].humidity).toBe(60);
  });

  it('should configure chart options properly', () => {
    const sampleData = [
      { date: '01/04/2021 12:00:00', temperature: 25, humidity: 60 }
    ];
    component.setupChart(sampleData);
    expect(component.chartOptions.title?.text).toBe('Temperature & Humidity Trend');
    expect(component.chartOptions.series?.length).toBe(2);
  });

  it('should configure grid options properly', () => {
    component.setupGrid();
    expect(component.gridOptions.columnDefs?.length).toBe(3);
    expect(component.gridOptions.paginationPageSize).toBe(20);
  });
});
