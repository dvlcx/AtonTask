using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtonTask.Models.InputModels
{
    public record LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}