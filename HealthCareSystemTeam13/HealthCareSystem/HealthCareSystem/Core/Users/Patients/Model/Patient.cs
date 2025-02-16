﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareSystem.Core.Users.Patients.Model
{
    class Patient
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsBlocked = false;
        public int UserId { get; set; }

        public string FullName { get; set; }
        
        public Patient()
        {

        }
        public Patient(string firstName, string lastName, int userId, bool isBlocked)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserId = userId;
            this.IsBlocked = isBlocked;
            this.FullName = FirstName + " " + LastName;
        }

        public Patient(int id, string firstName, string lastName, int userId, bool isBlocked)
        {
            this.ID = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserId = userId;
            this.IsBlocked = isBlocked;
            this.FullName = FirstName + " " + LastName;
        }

    }
}
