using System;
using System.Collections.Generic;
using System.Linq;
//Smell - file and namespace names
namespace expensereport_csharp
{
    public class ExpenseReport
    {
        private readonly IReportScribe _reportScribe;
        private readonly VisitorTotal _visitorTotal;

        public ExpenseReport() : this(new ConsoleReportScribe(), new VisitorTotal())
        { }

        public ExpenseReport(IReportScribe reportScribe, VisitorTotal visitorTotal)
        {
            _reportScribe = reportScribe;
            _visitorTotal = visitorTotal;
        }

        public void PrintReport(IEnumerable<ISmartExpense> expenses)
        {
            //DS - Direct use of DateTime
            _reportScribe.WriteLine("Expenses " + DateTime.Now);
            
            foreach (ISmartExpense expense in expenses)
            {
                expense.AddToTotal(_visitorTotal);
                expense.ReportTo(_reportScribe);
            }

            _visitorTotal.ReportTo(_reportScribe);
        }
    }

    public interface ITotal
    {
        void Add(TotalType type, int amount);
        void ReportTo(IReportScribe reportScribe);
    }

    internal abstract class Total : ITotal
    {
        private readonly string _tag;
        private int _amount;

        protected Total(string tag) : this(tag, 0)
        { }

        private Total(string tag, int amount)
        {
            _tag = tag;
            _amount = amount;
        }
        public virtual void Add(TotalType type, int amount)
        {
            _amount += amount;
        }

        public void ReportTo(IReportScribe reportScribe)
        {
            reportScribe.WriteLine($"{_tag} expenses: {_amount}");
        }
    }

    internal sealed class MealTotal : Total
    {
        public MealTotal() : base("Meal")
        { }

        public override void Add(TotalType type, int amount)
        {
            if (!TotalType.Meal.Equals(type)) return;
            base.Add(type, amount);
        }
    }

    internal sealed class TotalTotal : Total
    {
        public TotalTotal() : base("Total")
        { }
    }

    public sealed class VisitorTotal : ITotal
    {
        private readonly List<ITotal> _totals;

        public VisitorTotal() : this(new List<ITotal> { new MealTotal(), new TotalTotal() })
        { }

        private VisitorTotal(List<ITotal> totals) => _totals = totals;

        public void Add(TotalType type, int amount)
        {
            _totals.ForEach(x => x.Add(type, amount));
        }

        public void ReportTo(IReportScribe reportScribe)
        {
            _totals.ForEach(x => x.ReportTo(reportScribe));
        }
    }

    public enum TotalType
    {
        Total = 0,
        Meal = 1,
        Travel = 2
    }


    public abstract class SmartExpense:ISmartExpense
    {
        private readonly TotalType _totalType;
        private readonly int _amount;
        private readonly string _name;
        private readonly int _maxAllowed;

        protected SmartExpense(TotalType totalType, int amount, string name, int maxAllowed)
        {
            _totalType = totalType;
            _amount = amount;
            _name = name;
            _maxAllowed = maxAllowed;
        }
        public void AddToTotal(ITotal visitor) => visitor.Add(_totalType, _amount);

        public void ReportTo(IReportScribe reportScribe) => reportScribe.WriteLine(_name + "\t" + _amount + "\t" + OverLimitMarker());
        private bool IsOverLimit() => _maxAllowed < _amount;
        private string OverLimitMarker() => IsOverLimit() ? "X" : " ";
    }

    public interface ISmartExpense
    {
        void AddToTotal(ITotal total);
        void ReportTo(IReportScribe reportScribe);
    }

    public abstract class MealExpense : SmartExpense
    {
        protected MealExpense(int amount, string name, int maxAllowed) : base(TotalType.Meal, amount, name, maxAllowed)
        {
        }
    }
    internal abstract class TravelExpense : SmartExpense
    {
        protected TravelExpense(int amount, string name) : base(TotalType.Travel, amount, name, int.MaxValue)
        {
        }
    }

    internal sealed class DinnerExpense : MealExpense
    {
        public DinnerExpense(int amount):base(amount, "Dinner", 5000)
        { }
    }

    internal sealed class BreakfastExpense : MealExpense
    {
        public BreakfastExpense(int amount) : base(amount, "Breakfast", 1000)
        { }
    }

    internal sealed class CarRentalExpense : TravelExpense
    {
        public CarRentalExpense(int amount) : base(amount, "Car Rental")
        { }
    }

    public class LunchExpense : MealExpense
    {
        public LunchExpense(int amount) : base(amount, "Lunch", 2000)
        {
        }
    }

    internal sealed class ConsoleReportScribe : IReportScribe
    {
        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public interface IReportScribe
    {
        void WriteLine(string msg);
    }
}