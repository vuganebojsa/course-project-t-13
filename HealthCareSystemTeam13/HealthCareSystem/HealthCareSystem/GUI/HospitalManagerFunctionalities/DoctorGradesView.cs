﻿using HealthCareSystem.Core.Surveys;
using HealthCareSystem.Core.Surveys.Repository;
using HealthCareSystem.Core.Users.Doctors.Model;
using HealthCareSystem.Core.Users.Doctors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthCareSystem.GUI.HospitalManagerFunctionalities
{
    public partial class DoctorGradesView : Form
    {
        private int DoctorId { get; set; }
        private readonly ISurveyRepository _surveyRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISurveyService _surveyService;
        public DoctorGradesView(int doctorId)
        {
            _surveyRepository = new SurveyRepository();
            _doctorRepository = new DoctorRepository();
            _surveyService = new SurveyService();
            DoctorId = doctorId;
            InitializeComponent();
            FillLabels();
        }

        private void FillLabels()
        {
            BindingList<Doctor> doctors = _doctorRepository.GetDoctors();

            string doctorFullName = "";
            foreach (Doctor doctor in doctors)
            {
                if(doctor.ID == DoctorId)
                {
                    doctorFullName = doctor.FullName; 
                }
            }


            lblDoctor.Text += DoctorId + "  " + doctorFullName;
            List<DoctorSurvey> doctorSurveys = _surveyRepository.GetDoctorSurveys();
            double[] avgGrades = _surveyService.GetAverageDoctorGrades(doctorSurveys, DoctorId);
            
            int[] numberDoctorGrades = _surveyService.GetNumberOfDoctorGrades(doctorSurveys, true, DoctorId);
            int[] numberQualityGrades = _surveyService.GetNumberOfDoctorGrades(doctorSurveys, false, DoctorId);

            Label[] gradeLabels = { lblDoctor1, lblDoctor2, lblDoctor3, lblDoctor4, lblDoctor5 };
            Label[] qualityLabels = { lblQuality1, lblQuality2, lblQuality3, lblQuality4, lblQuality5};

            lblAvgDoctorGrade.Text += avgGrades[0].ToString();
            lblAvgQuality.Text += avgGrades[1].ToString();

            for(int i = 0; i < 5; i++)
            {
                gradeLabels[i].Text += numberDoctorGrades[i].ToString();
                qualityLabels[i].Text += numberQualityGrades[i].ToString();            
            }
            

        }

        private void DoctorGradesView_Load(object sender, EventArgs e)
        {

        }
    }
}
