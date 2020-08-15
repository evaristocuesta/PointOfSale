import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ApiClientService } from 'src/app/services/api-client.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Client } from 'src/app/models/client';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
    templateUrl: 'clientDialog.component.html'
})
export class ClientDialogComponent {

    public clientForm = this.formBuilder.group({
        name: ['', Validators.required],
    })

    constructor(
        private formBuilder: FormBuilder,
        public dialogRef: MatDialogRef<ClientDialogComponent>, 
        public apiClient: ApiClientService, 
        public snackBar: MatSnackBar, 
        @Inject(MAT_DIALOG_DATA) public client: Client
    ) {
        if (this.client != null) {
            this.clientForm.setValue({ name: client.name});
        }
    }

    close() {
        this.dialogRef.close();
    }

    addClient() {
        const client: Client = {id: 0, name: this.clientForm.value.name};
        this.apiClient.addClient(client).subscribe(response => {
            if (response.success) {
                this.dialogRef.close();
                this.snackBar.open(`Client ${client.name} added`, '', {
                    duration: 2000
                });
            }
        })
    }
    
    editClient() {
        const client: Client = {id: this.client.id, name: this.clientForm.value.name};
        this.apiClient.editClient(client).subscribe(response => {
            if (response.success) {
                this.dialogRef.close();
                this.snackBar.open(`Client ${client.name} edited`, '', {
                    duration: 2000
                });
            }
        })
    }
}