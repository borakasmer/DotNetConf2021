import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductService } from './service/productService';
import { AgGridModule } from "ag-grid-angular";
import { ButtonRendererComponent } from './button-renderer.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    AgGridModule.withComponents([
      ButtonRendererComponent
    ]),

  ],
  providers: [ProductService],
  bootstrap: [AppComponent]
})
export class AppModule { }
