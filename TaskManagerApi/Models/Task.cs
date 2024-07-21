using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagerApi.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}