using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{


    public  struct EventType
    {
        public string Title { get; private set; }
        private EventType(string title) 
        {
            Title = title;
        }

        public override string ToString()
        {
            return this.Title;
        }
        public static EventType Like = new EventType("Created");

    }
    public class CPMTIntegrationEvent
    {
        public int ID { get; set; }
        public string EventType { get; set; }
        public decimal ProjId { get; set; }
        public DateTime EventDate { get; set; }
    }


    public class ProjectDetails
    {
        public decimal ProjId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectTitle { get; set; }
        public string Currency { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int P_Elmt { get; set; }
        public DateTime obj_Date { get; set; }


    }






}
