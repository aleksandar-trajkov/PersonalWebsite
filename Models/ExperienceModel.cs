using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class ExperienceModel
    {
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Logo { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public List<string> Responsibilities { get; set; }
        public Direction Direction { get; set; }
    }

    public enum Direction
    {
        Left =1,
        Right = 2
    }
}
