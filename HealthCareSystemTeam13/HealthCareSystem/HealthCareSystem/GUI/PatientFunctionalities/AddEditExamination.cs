﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthCareSystem.Core;
using HealthCareSystem.Core.Examinations.Model;
using HealthCareSystem.Core.Examinations.Repository;
using HealthCareSystem.Core.Rooms.Repository;
using HealthCareSystem.Core.Users.Doctors;
using HealthCareSystem.Core.Users.Doctors.Model;
using HealthCareSystem.Core.Users.Doctors.Repository;
using HealthCareSystem.Core.Users.Doctors.Service;
using HealthCareSystem.Core.Users.Patients.Model;
using HealthCareSystem.Core.Users.Patients.Repository;

namespace HealthCareSystem.Core.GUI.PatientFunctionalities
{
    public partial class AddEditExamination : Form
    {
        public int ExaminationId { get; set; }
        public DateTime ExaminationDate { get; set; }
        private Doctor _selectedDoctor { get; set; }
        private BindingList<Doctor> _doctors;
        private int _RoomId { get; set; }
        private int _Duration { get; set; }
        public bool IsAddChoosen { get; set; }
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IExaminationRepository _examinationRepository;
        private readonly string _patientUsername;
        private readonly int _validDate;
        private readonly IExaminationEditRequestRepository _examinationEditRequestRepository;
        private IDoctorService _doctorService;

        public AddEditExamination(int examinationId, bool isAddChoosen, string patientUsername, int validDate, int doctorId = 0)
        {
            ExaminationId = examinationId;
            IsAddChoosen = isAddChoosen;
            _patientUsername = patientUsername;
            _validDate = validDate;
            _patientRepository = new PatientRepository(patientUsername);
            _doctorRepository = new DoctorRepository();
            _roomRepository = new RoomRepository();
            _examinationRepository = new ExaminationRepository();
            _examinationEditRequestRepository = new ExaminationEditRequestRepository();
            _doctorService = new DoctorService();

            InitializeComponent();

            FillDoctorsComboBox();
            if (!IsAddChoosen)
            {
                LoadEditData();

            }
            else
            {
                tbDuration.Text = "15";
            }
            SetSelectedDoctorById(doctorId);

        }

        private void SetSelectedDoctorById(int doctorId)
        {
            if (doctorId != 0)
            {
                foreach (Doctor doctor in _doctors)
                {
                    if (doctorId == doctor.ID)
                    {
                        cbDoctors.SelectedItem = doctor;
                        break;
                    }
                }
            }
        }

        private void LoadEditData()
        {
            Dictionary<string, string> data = _examinationRepository.GetExamination(ExaminationId);
            tbExaminationId.Text = data["id"];

            tbDuration.Text = "15";
            tbRoomId.Text = data["room_id"];
            DateTime dateParsed = DateTime.Parse(data["dateOf"]);
            
            dtDate.Value = dateParsed;
            tbTime.Text = dateParsed.Hour.ToString() + ":" + dateParsed.Minute.ToString();
            cbDoctors.SelectedIndex = cbDoctors.FindStringExact(GetSelectedDoctor().FullName);

        }

        private void FillDoctorsComboBox()
        {
            // Using BindingList so that i can display the name of the doctor, and have his id in the back
            _doctors = _doctorRepository.GetDoctors();

            cbDoctors.ValueMember = null;
            cbDoctors.DropDownStyle = ComboBoxStyle.DropDownList;
            
            cbDoctors.DisplayMember = "FullName";
            cbDoctors.DataSource = _doctors;
            
        }

        private Doctor GetSelectedDoctor()
        {
            Doctor doctor;
            string doctorQuery = "select * from Doctors inner join Examination on Doctors.id = Examination.id_doctor where Examination.id = " + ExaminationId + "";
            doctor = _doctorRepository.GetSelectedDoctor(doctorQuery);

            return doctor;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            SetSelectedValues();
            if (tbRoomId.Text == "") tbRoomId.Text = "0";

            bool isValid = CheckSelectedValues();
            
            if (isValid)
            {
                string time = tbTime.Text;
                DateTime mergedTime = TimeDateHelpers.GetMergedDateTime(ExaminationDate, time);

                if (IsAddChoosen)
                    InsertExamination(mergedTime);
                else
                {
                    if (_validDate == 1)
                        UpdateContent(mergedTime);
                    else
                        SendExaminationEditRequest(mergedTime);
                }

                _patientRepository.BlockSpamPatients(_patientUsername);
                this.Close();
            } 
        }

        private void InsertExamination(DateTime mergedTime)
        {
            _examinationRepository.InsertExamination(_patientRepository.GetPatientId(), _selectedDoctor.ID, mergedTime,
                _Duration, _RoomId);
            MessageBox.Show("Successfully added examination!");
        }

        private void SendExaminationEditRequest(DateTime mergedTime)
        {
            _examinationEditRequestRepository.SendExaminationEditRequest(ExaminationId, DateTime.Now, true, _selectedDoctor.ID, mergedTime,
                _RoomId);

            _examinationRepository.InsertExaminationChanges(TypeOfChange.Edit, _patientRepository.GetPatientId());

            MessageBox.Show("Wait for a secretary to aproove this request.");
        }

        private void UpdateContent(DateTime mergedTime)
        {
            string updateQuery = "Update Examination set id_doctor = " + _selectedDoctor.ID + ", isEdited=" + true + ", dateOf = '" + mergedTime + "', id_room = " + _RoomId + " where id = " + ExaminationId + "";
            _patientRepository.UpdatePatientContent(updateQuery, _patientRepository.GetPatientId());
            MessageBox.Show("Successfully edited examination!");

        }

        private bool CheckSelectedValues()
        {
            var regex = @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$";
            string time = tbTime.Text;

            List<Examination> otherExaminations = _examinationRepository.GetAllOtherExaminations(ExaminationId);
            DateTime mergedExaminationTime = TimeDateHelpers.GetMergedDateTime(ExaminationDate, time);
            var match = System.Text.RegularExpressions.Regex.Match(tbTime.Text, regex);

            if (ExaminationDate <= DateTime.Now)
            {

                MessageBox.Show("Examination date must be after current time.");
                return false;

            }
            else if (!_doctorService.IsDoctorAvailable(_selectedDoctor.ID, mergedExaminationTime, otherExaminations))
            {

                MessageBox.Show("Doctor is not available at that time.");
                return false;

            }
            else if (!match.Success)
            {

                MessageBox.Show("Invalid time format. Please enter like: HH:MM ");
                return false;

            }
            else if (!_roomRepository.IsRoomAvailable(Convert.ToInt32(tbRoomId.Text), mergedExaminationTime, otherExaminations))
                if (!SetAvailableRoomId(mergedExaminationTime, otherExaminations)) return false;

            return true;
        }

        private bool SetAvailableRoomId(DateTime mergedExaminationTime, List<Examination> otherExaminations)
        {
            int availableRoomId = _roomRepository.GetAvailableRoomId(mergedExaminationTime, otherExaminations);
            if (availableRoomId == 0)
            {
                MessageBox.Show("No available rooms at this date/time.");
                return false;
            }

            _RoomId = availableRoomId;
            return true;
        }

        private void SetSelectedValues()
        {
            _selectedDoctor = (Doctor)cbDoctors.SelectedValue;
            if (tbRoomId.Text != "") { _RoomId = Convert.ToInt32(tbRoomId.Text); }
            else { _RoomId = 0; }
            _Duration = Convert.ToInt32(tbDuration.Text);
            ExaminationDate = dtDate.Value;
        }
       
    }
}
