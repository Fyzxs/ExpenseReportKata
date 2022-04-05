using System;
using System.Collections.Generic;

//Smell - file and namespace names
namespace expensereport_csharp
{
    //DS - Enum
    //DS - Naming Convention
    public enum ExpenseType
    {
        //DS - undeclared backing value
        DINNER, BREAKFAST, CAR_RENTAL
    }
    
    public class Expense
    {
        //DS - DTO
        //DS - Exposed fields
        //DS - money as int
        public ExpenseType type;
        public int amount;
    }

    //DS - Could be static
    public class ExpenseReport
    {
        private readonly IReportScribe _reportScribe;

        public ExpenseReport():this(new ConsoleReportScribe()) {}

        private ExpenseReport(IReportScribe reportScribe) => _reportScribe = reportScribe;

        //DS - Could be static
        public void PrintReport(List<Expense> expenses)
        {
            //DS - money as int
            int total = 0;
            int mealExpenses = 0;

            //DS - Hard coupling to specific report output
            //DS - Direct use of DateTime
            _reportScribe.WriteLine("Expenses " + DateTime.Now);

            //DS - type behavior spread over 3 sections
            foreach (Expense expense in expenses)
            {
                
                if (expense.type == ExpenseType.DINNER || expense.type == ExpenseType.BREAKFAST)
                {
                    mealExpenses += expense.amount;
                }

                String expenseName = "";
                //DS - switch
                //DS - Hard coding
                switch (expense.type)
                {
                    case ExpenseType.DINNER:
                        expenseName = "Dinner";
                        break;
                    case ExpenseType.BREAKFAST:
                        expenseName = "Breakfast";
                        break;
                    case ExpenseType.CAR_RENTAL:
                        expenseName = "Car Rental";
                        break;
                }

                //DS - Hard Coded ... lots
                String mealOverExpensesMarker =
                    expense.type == ExpenseType.DINNER && expense.amount > 5000 ||
                    expense.type == ExpenseType.BREAKFAST && expense.amount > 1000
                        ? "X"
                        : " ";

                //DS - coupled writer
                _reportScribe.WriteLine(expenseName + "\t" + expense.amount + "\t" + mealOverExpensesMarker);

                total += expense.amount;
            }

            _reportScribe.WriteLine("Meal expenses: " + mealExpenses);
            _reportScribe.WriteLine("Total expenses: " + total);
        }
    }

    internal sealed class ConsoleReportScribe : IReportScribe
    {
        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    internal interface IReportScribe
    {
        void WriteLine(string msg);
    }
}