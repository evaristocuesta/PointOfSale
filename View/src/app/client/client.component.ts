import { Component, OnInit } from '@angular/core';
import { ApiClientService } from '../services/api-client.service';
import { Response } from '../models/response';
import { ClientDialogComponent } from './clientDialog/clientDialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Client } from '../models/client';
import { DialogDeleteComponent } from '../common/delete/dialogdelete.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-client',
  templateUrl: './client.component.html',
  styleUrls: ['./client.component.scss']
})
export class ClientComponent implements OnInit {

  public lst: any[];
  public columns: string [] = ['id', 'name', 'actions'];
  readonly width: string = '300px';

  constructor(
    private apiClient : ApiClientService, 
    public dialog: MatDialog, 
    public snackBar: MatSnackBar ) { 

  }

  ngOnInit(): void {
    this.getClients();
  }

  getClients() {
    this.apiClient.getClients().subscribe(response => {
      this.lst = response.data;
    });
  }

  showAddClient() {
    const dialogRef = this.dialog.open(ClientDialogComponent, {
      width: this.width
    });
    dialogRef.afterClosed().subscribe(result => {
      this.getClients();
    });
  }

  showEditClient(client: Client) {
    const dialogRef = this.dialog.open(ClientDialogComponent, {
      width: this.width, 
      data: client
    });
    dialogRef.afterClosed().subscribe(result => {
      this.getClients();
    });
  }

  deleteClient(client: Client) {
    const dialogRef = this.dialog.open(DialogDeleteComponent, {
      width: this.width
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiClient.deleteClient(client.id).subscribe(response => {
          if (response.success) {
            this.snackBar.open(`Client ${client.name} deleted`, '', {
              duration: 2000
            });
            this.getClients();
          }
        })
      }
    });
  }
}
