using Company.Models.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.ViewModels
{
    public class CountMonthReportViewModel
    {
        public int OverallAmount { get; set; }
        public ICollection<ObjectOfExpenditure> ObjectOfExpenditures { get; set; }

    }
}
