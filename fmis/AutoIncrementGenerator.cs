using System;

namespace fmis
{
    public class AutoIncrementGenerator
    {
        private int counter;
        private readonly string supValue;
        private readonly string inValue;
        private readonly int currentMonth;

        public AutoIncrementGenerator()
        {
            // Retrieve the initial counter value from the database or any other persistent storage
            // In this example, we'll start with 1
            counter = 30;

            supValue = "S";
            inValue = "T";
            currentMonth = DateTime.Now.Month % 100;
        }

        public string GenerateIndividual()
        {
            string number = $"{inValue}{currentMonth:D2}-{counter:D4}";

            // Increment the counter for the next number
            counter++;

            return number;
        }

        public string GenerateSupplier()
        {
            string number = $"{supValue}{currentMonth:D2}-{counter:D4}";

            // Increment the counter for the next number
            counter++;

            return number;
        }
    }
}
