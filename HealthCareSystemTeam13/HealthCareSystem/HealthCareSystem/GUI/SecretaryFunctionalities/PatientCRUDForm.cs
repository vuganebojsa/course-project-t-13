﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthCareSystem.Core.Users.Secretaries.Repository;
using HealthCareSystem.Core.GUI.SecretaryFunctionalities;
using HealthCareSystem.Core.Users.Patients.Repository;

namespace HealthCareSystem.Core.GUI
{
    public partial class PatientCRUDForm : Form
    {
        private readonly IPatientRepository _patientRepository;
        string Username;
        public PatientCRUDForm(string username)
        {
            _patientRepository = new PatientRepository();
            _patientRepository.PullPatients();
            InitializeComponent();
            FillDataGridView();
            this.Username = username;
        }

        private void FillDataGridView()
        {
            patientsDataGrid.DataSource = _patientRepository.GetPatientsTable();
            DataGridViewSettings();
        }

        private void DataGridViewSettings()
        {
            patientsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            patientsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            patientsDataGrid.MultiSelect = false;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddPatient addPatientForm = new AddPatient();
            addPatientForm.ShowDialog();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            EditPatient editPatient = new EditPatient();
            editPatient.ShowDialog();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeletePatient deletePatientForm = new DeletePatient();
            deletePatientForm.ShowDialog();
        }

        private void blockButton_Click(object sender, EventArgs e)
        {
            BlockPatient blockPatient = new BlockPatient(Username);
            blockPatient.ShowDialog();
        }

        private void urgentButton_Click(object sender, EventArgs e)
        {
            ChooseSpeciality chooseSpeciality = new ChooseSpeciality((int)patientsDataGrid.SelectedRows[0].Cells[0].Value);
            chooseSpeciality.ShowDialog();
        }
    }
}
