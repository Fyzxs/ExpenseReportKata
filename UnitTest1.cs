using System;
using System.Collections.Generic;
using System.IO;
using expensereport_csharp;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseReportKata
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NoExpenses()
        {
            // ARRANGE
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            ExpenseReport subject = new();

            // ACT
            subject.PrintReport(new List<Expense>());

            //ASSERT
            string[] lines = stringWriter.ToString().Split(Environment.NewLine);
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Meal expenses: 0");
            lines[2].Should().Be("Total expenses: 0");
        }
        [TestMethod]
        public void SingleDinnerUnder()
        {
            // ARRANGE
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            ExpenseReport subject = new();
            Expense dinner = new Expense() { amount = 5000, type = ExpenseType.DINNER };

            // ACT
            subject.PrintReport(new List<Expense> { dinner });

            //ASSERT
            string[] lines = stringWriter.ToString().Split(Environment.NewLine);
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5000\t ");
            lines[2].Should().Be("Meal expenses: 5000");
            lines[3].Should().Be("Total expenses: 5000");
        }
        [TestMethod]
        public void SingleDinnerOver()
        {
            // ARRANGE
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            ExpenseReport subject = new();
            Expense dinner = new Expense() { amount = 5001, type = ExpenseType.DINNER };

            // ACT
            subject.PrintReport(new List<Expense> { dinner });

            //ASSERT
            string[] lines = stringWriter.ToString().Split(Environment.NewLine);
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5001\tX");
            lines[2].Should().Be("Meal expenses: 5001");
            lines[3].Should().Be("Total expenses: 5001");
        }
        [TestMethod]
        public void AllWithBreakfastOver()
        {
            // ARRANGE
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            ExpenseReport subject = new();
            Expense bfast = new Expense() { amount = 1001, type = ExpenseType.BREAKFAST };
            Expense dinner = new Expense() { amount = 5000, type = ExpenseType.DINNER };
            Expense car = new Expense() { amount = 2000, type = ExpenseType.CAR_RENTAL };

            // ACT
            subject.PrintReport(new List<Expense> { dinner, bfast, car });

            //ASSERT
            string[] lines = stringWriter.ToString().Split(Environment.NewLine);
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5000\t ");
            lines[2].Should().Be("Breakfast\t1001\tX");
            lines[3].Should().Be("Car Rental\t2000\t ");
            lines[4].Should().Be("Meal expenses: 6001");
            lines[5].Should().Be("Total expenses: 8001");
        }
    }
}