using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis
{
    public static class GlobalData
    {
        public static String Year
        {
            get
            {
                return "Year" as String;
            }
            set
            {
                Year = value;
            }
        }
        public static String BudgetID
        {
            get
            {
                return "BUDGET_ID" as String;
            }
            set
            {
                BudgetID = value;
            }
        }

        public static String ors_allotment
        {
            get
            {
                return "ors_allotment" as String;
            }
            set
            {
                ors_allotment = value;
            }
        }
        public static String allotment
        {
            get
            {
                return "allotment" as String;
            }
            set
            {
                allotment = value;
            }
        }
    }
}
