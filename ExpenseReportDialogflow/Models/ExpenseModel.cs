using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseReportDialogflow.Models
{
    public class ExpenseModel
    {
        public string Place { get; set; }
        public float Price { get; set; }
        public string ExpenseType { get; set; }
    }
}
