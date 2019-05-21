using System;

namespace SharePointClient
{
    public class Task
    {
        public Guid Id;
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public double PercentComplete { get; set; }
        public string Description { get; set; }
        public Task(string Title = "",
                    string Status = "Not started",
                    string Priority = "Low",
                    double PercentComplete = 0,
                    string Description = "",
                    DateTime? DueDate = null)
        {
            this.Title = Title;
            this.Status = Status;
            this.Priority = Priority;
            this.Description = Description;
            this.PercentComplete = PercentComplete;
            this.DueDate = DueDate;
            Id = Guid.NewGuid();
        }
    }
}