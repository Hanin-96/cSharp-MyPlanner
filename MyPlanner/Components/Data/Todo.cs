﻿namespace MyPlanner.Data
{
    public class Todo
    {
        //Definierar struktur på todo data
        private int id;
        private string title = string.Empty;
        private string description = string.Empty;
        private string category = string.Empty;
        private int priority;
        private bool status;
        private DateTime dueDate;

        //Definierar get och set på alla variabler 
        public int Id { get { return id; } set { id = value; } }
        public string? Title { get { return title; } set { title = value; } }
        public string? Description { get { return description; } set { description = value; } }
        public string? Category { get { return category; } set { category = value; } }
        public int Priority { get { return priority; } set { priority = value; } }
        public bool Status { get { return status; } set { status = value; } }
        public DateTime DueDate { get { return dueDate; } set { dueDate = value; } }

        //Konstruktor
        public Todo()
        {
            //Avrundar tiden till närmaste minut
            DateTime timeNow = DateTime.Now;
            DueDate = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, 0);
            //Status ska alltid vara satt till false(=0), dvs inte avklarad när ny todo läggs till
            Status = false;
            Category = "";
        }

    }
}
