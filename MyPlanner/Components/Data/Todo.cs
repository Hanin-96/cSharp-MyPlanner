namespace MyPlanner.Data
{
    public class Todo
    {
        //Definierar struktur på todo data
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }

        //Konstruktor
        public Todo()
        {
            //Avrundar tiden till närmaste minut
            DateTime timeNow = DateTime.Now;
            DueDate = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, 0);
            //Status ska alltid vara satt till 0, dvs inte avklarad när ny todo läggs till
            Status = 0;
        }

    }
}
