﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCareSystem.Core.Rooms.Renovations.Model;

namespace HealthCareSystem.Core.Rooms.Repository
{
    class RenovationRepository: IRenovationRepository
    {
        public OleDbConnection Connection { get; set; }
        public DataTable Renovations { get; private set; }

        public RenovationRepository()
        {
            try
            {
                Connection = DatabaseConnection.GetConnection();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
        public void PullRenovations()
        {
            Renovations = new DataTable();
            string renovationsQuery = "select * from renovations";
            GUIHelpers.FillTable(Renovations, renovationsQuery, Connection);
        }

        public DataTable GetRenovationsDataTable()
        {
            return Renovations;
        }


        public void InsertRenovation(Renovation renovation)
        {
            var query = "INSERT INTO Renovations(id_room, dateOfStart, dateOfFinish, id_other_room, renovationType) VALUES(@id_room, @startingDate, @ending_date, @id_other_room, @renovationType)";

            using (var cmd = new OleDbCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@id_room", renovation.RoomId);
                cmd.Parameters.AddWithValue("@startingDate", renovation.StartingDate.ToString());
                cmd.Parameters.AddWithValue("@ending_date", renovation.EndingDate.ToString());
                if (renovation.SecondRoomId == -1)
                {
                    cmd.Parameters.Add("@id_other_room", OleDbType.Integer).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.AddWithValue("@id_other_room", renovation.SecondRoomId);
                }

                cmd.Parameters.AddWithValue("@renovationType", renovation.Type.ToString());
                cmd.ExecuteNonQuery();
            }

        }


        public List<Renovation> GetRenovations(string query)
        {
            List<Renovation> renovations = new List<Renovation>();

            try
            {

                OleDbCommand cmd = DatabaseCommander.GetCommand(query, Connection);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Enum.TryParse<Renovation.TypeOfRenovation>(reader["renovationType"].ToString(), out var typeOfRenovation);

                    try
                    {
                        renovations.Add(new Renovation(Convert.ToInt32(reader["id_room"]), Convert.ToDateTime(reader["dateOfStart"]), Convert.ToDateTime(reader["dateOfFinish"]), Convert.ToInt32(reader["id_other_room"]), typeOfRenovation));
                    }
                    catch (Exception)
                    {
                        renovations.Add(new Renovation(Convert.ToInt32(reader["id_room"]), Convert.ToDateTime(reader["dateOfStart"]), Convert.ToDateTime(reader["dateOfFinish"]), -1, typeOfRenovation));
                    }


                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return renovations;
        }


    }
}
