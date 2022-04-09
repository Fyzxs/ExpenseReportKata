using System;
using System.Collections.Generic;
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
            FakeReportScribe fakeReportScribe = new();
            ExpenseReport subject = new(fakeReportScribe, new VisitorTotal());

            // ACT
            subject.PrintReport(new List<ISmartExpense>());

            //ASSERT
            string[] lines = fakeReportScribe.Lines.ToArray();
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Meal expenses: 0");
            lines[2].Should().Be("Total expenses: 0");
        }
        [TestMethod]
        public void SingleDinnerUnder()
        {
            // ARRANGE
            FakeReportScribe fakeReportScribe = new();
            ExpenseReport subject = new(fakeReportScribe, new VisitorTotal());
            DinnerExpense dinnerExpense = new(5000);

            // ACT
            subject.PrintReport(new List<ISmartExpense> { dinnerExpense });

            //ASSERT
            string[] lines = fakeReportScribe.Lines.ToArray();
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5000\t ");
            lines[2].Should().Be("Meal expenses: 5000");
            lines[3].Should().Be("Total expenses: 5000");
        }
        [TestMethod]
        public void SingleDinnerOver()
        {
            // ARRANGE
            FakeReportScribe fakeReportScribe = new();
            ExpenseReport subject = new(fakeReportScribe, new VisitorTotal());
            DinnerExpense dinnerExpense = new(5001);

            // ACT
            subject.PrintReport(new List<ISmartExpense> { dinnerExpense });

            //ASSERT
            string[] lines = fakeReportScribe.Lines.ToArray();
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5001\tX");
            lines[2].Should().Be("Meal expenses: 5001");
            lines[3].Should().Be("Total expenses: 5001");
        }
        [TestMethod]
        public void AllWithBreakfastOver()
        {
            // ARRANGE
            FakeReportScribe fakeReportScribe = new();
            ExpenseReport subject = new(fakeReportScribe, new VisitorTotal());
            BreakfastExpense breakfastExpense = new(1001);
            DinnerExpense dinnerExpense = new(5000);
            CarRentalExpense carRentalExpense = new(2000);

            // ACT
            subject.PrintReport(new List<ISmartExpense> { dinnerExpense, breakfastExpense, carRentalExpense });

            //ASSERT
            string[] lines = fakeReportScribe.Lines.ToArray();
            lines[0].Should().Contain("Expenses " + DateTime.Now);
            lines[1].Should().Be("Dinner\t5000\t ");
            lines[2].Should().Be("Breakfast\t1001\tX");
            lines[3].Should().Be("Car Rental\t2000\t ");
            lines[4].Should().Be("Meal expenses: 6001");
            lines[5].Should().Be("Total expenses: 8001");
        }

        private class FakeReportScribe : IReportScribe
        {
            public readonly List<string> Lines = new();
            public void WriteLine(string msg) => Lines.Add(msg);
        }
    }
}