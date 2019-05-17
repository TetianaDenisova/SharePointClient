using System;

namespace SharePointClient
{
    public class Task
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public double PercentComplete { get; set; }

        public Task(string Title,
                    string Status = "Not started",
                    string Priority = "Low",
                    double PercentComplete = 0,
                    DateTime? DueDate = null)
        {
            this.Title = Title;
            this.Status = Status;
            this.Priority = Priority;
            this.PercentComplete = PercentComplete;
            this.DueDate = DueDate;

        }
    }
}