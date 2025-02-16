﻿using HealthCareSystem.Core.Users.Doctors.Model;
using HealthCareSystem.Core.Users.Secretaries.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthCareSystem.Core.Examinations.Repository;

namespace HealthCareSystem.Core.GUI.SecretaryFunctionalities
{
    partial class UrgentExaminations : Form
    {
        int PatiendId;
        DoctorSpeciality Speciality;
        int Duration;
        IExaminationRepository _examinationRepository;

        public UrgentExaminations(int patientID, DoctorSpeciality doctorSpeciality, int duration)
        {
            PatiendId = patientID;
            Speciality = doctorSpeciality;
            Duration = duration;
            _examinationRepository = new ExaminationRepository();
            _examinationRepository.PullClosestExaminations(doctorSpeciality);
            InitializeComponent();
            FillDataGridView();
        }

        private void FillDataGridView()
        {
            examinationsDataGrid.DataSource = _examinationRepository.GetClosestExaminations();
            DataGridViewSettings();
        }

        private void DataGridViewSettings()
        {
            examinationsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            examinationsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            examinationsDataGrid.MultiSelect = false;
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            DateTime from = (DateTime)examinationsDataGrid.SelectedRows[0].Cells[6].Value;
            DateTime to = from.AddMinutes(Duration);
            int doctorId = (int)examinationsDataGrid.SelectedRows[0].Cells[1].Value;
            int roomId = (int)examinationsDataGrid.SelectedRows[0].Cells[9].Value;
            _examinationRepository.MoveDoctorsExaminations(from, to, doctorId);
        }
    }
}
