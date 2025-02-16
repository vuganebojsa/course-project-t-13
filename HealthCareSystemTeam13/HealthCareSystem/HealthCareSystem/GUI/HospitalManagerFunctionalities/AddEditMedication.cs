﻿using HealthCareSystem.Core.Ingredients.Model;
using HealthCareSystem.Core.Medications.Model;
using HealthCareSystem.Core.Medications.Repository;
using HealthCareSystem.Core.Rooms.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthCareSystem.Core.GUI.HospitalManagerFunctionalities
{
    public partial class AddEditMedication : Form
    {
        public int MedicationId { get; set; }
        private readonly IIngredientRepository _ingredientRepository;
        private IMedicationRepository _medicationRepository;
        private string MedicationName;

        private bool IsAddChosen { get; set; }

        public AddEditMedication(int medicationId, bool isAddChosen)
        {
            MedicationId = medicationId;
            IsAddChosen = isAddChosen;
            _medicationRepository = new MedicationRepository();
            _ingredientRepository = new IngredientsRepository();
            InitializeComponent();
            FillCheckBoxList();

            if (!isAddChosen)
            {
                LoadEditData();
            }
        }

        private void FillCheckBoxList()
        {
            List<Ingredient> ingredients = _ingredientRepository.GetIngredients("select * from ingredients");
          
            foreach(Ingredient ingredient in ingredients)
            {
                clbIngredients.Items.Add(ingredient);
            }
        }

        private void LoadEditData()
        {
            string query = "select * from medications where id=" + MedicationId;
            Medication medication = _medicationRepository.GetSelectedMedication(query);


            List<Ingredient> allIngredients = _ingredientRepository.GetIngredients("select * from ingredients");
            List<Ingredient> ingredientsInMedication = _ingredientRepository.GetIngredients("select * from ingredients" +
                " where id in (select id_ingredient from MedicationContainsIngredient where id_medication=" + MedicationId + ")");
            

            foreach (Ingredient ingredient in allIngredients)
            {
                foreach(Ingredient containedIngredient in ingredientsInMedication)
                {
                    if(containedIngredient.Name == ingredient.Name)
                    {
                        clbIngredients.Items.Add(ingredient, true);
                    }
                    else
                    {
                        clbIngredients.Items.Add(ingredient, false);
                    }
                }
                
            }

            tbMedication.Text = medication.Name;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            MedicationName = tbMedication.Text;
            if (IsAddChosen)
            {
                bool isInserted = InsertMedication();
                if (!isInserted) return;

            }
            else
                UpdateDeniedMedication();

            this.Close();
        }

        private void UpdateDeniedMedication()
        {
            string updateNameAndStatusQuery = "Update medications set nameOfMedication = '" + MedicationName + "'" +
                                " ,status = '" + MedicationStatus.InProgress.ToString() + "' where id = " + MedicationId;
            DatabaseCommander.ExecuteNonQueries(updateNameAndStatusQuery, DatabaseConnection.GetConnection());

            string deleteRejectedQuery = "Delete from rejectedmedications where id_medication=" + MedicationId;
            DatabaseCommander.ExecuteNonQueries(deleteRejectedQuery, DatabaseConnection.GetConnection());

            string deleteMedicationIngredientsQuery = "Delete from MedicationContainsIngredient where id_medication=" + MedicationId;
            DatabaseCommander.ExecuteNonQueries(deleteMedicationIngredientsQuery, DatabaseConnection.GetConnection());

            foreach (Ingredient ingredient in clbIngredients.CheckedItems)
            {
                _medicationRepository.InsertMedicationContainsIngredient(MedicationId, ingredient.Id);
            }
            MessageBox.Show("Successfully edited a medication!");
        }

        private bool InsertMedication()
        {
            if (!_medicationRepository.DoesMedicationExists(MedicationName))
            {
                MessageBox.Show("There already exists medication with this name!");
                this.Close();
                return false;
            }


            _medicationRepository.InsertMedication(MedicationName);

            string medicationQuery = "select * from medications where nameOfMedication ='" + MedicationName + "'";
            Medication newMedication = _medicationRepository.GetSelectedMedication(medicationQuery);

            foreach (Ingredient ingredient in clbIngredients.CheckedItems)
            {
                _medicationRepository.InsertMedicationContainsIngredient(newMedication.Id, ingredient.Id);
            }
            MessageBox.Show("Successfully added a medication!");
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
