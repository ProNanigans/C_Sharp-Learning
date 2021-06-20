using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace src_neil
{
    /*
Class List:
Staff
Manager : Staff
Admin : Staff
FileReader
PaySlip
Program
*/

//Staff Class
class Staff {
  //Fields
  private float hourlyRate;
  private int hWorked;
  //Properties
  public float TotalPay { get; protected set; }
  public float BasicPay { get; private set; }
  public string NameOfStaff { get; private set; }
  public int HoursWorked {
    get {
      return hWorked;
    }
    set {
      if (value > 0) {
        hWorked = value;
      } else {
        hWorked = 0;
      }
    }
  }
  //Constructors
  public Staff(string name, float rate) {
    NameOfStaff = name;
    hourlyRate = rate;
  }
  //Methods
  public virtual void CalculatePay() {
    Console.WriteLine("Calculating Pay...");
    BasicPay = hWorked * hourlyRate;
    TotalPay = BasicPay;
  }
  public override string ToString() {
    return "Hourly Rate = " + hourlyRate +
    "\nHours Worked = " + HoursWorked +
    "\nTotal Pay = " + TotalPay +
    "\nBasic Pay = " + BasicPay +
    "\nName Of Staff = " + NameOfStaff;
  }
}

//Manager Class
class Manager : Staff {
  //Fields
  private const float managerHourlyRate = 50;
  //Properties
  public int Allowance { get; set; }
  //Constructors
  public Manager(string name) : base(name, managerHourlyRate) { /* nothing */ }
  //Methods
  public override void CalculatePay() {
    base.CalculatePay();
    Allowance = 1000;
    if (HoursWorked > 160) {
      TotalPay += Allowance;
    }
  }
  public override string ToString() {
    string super = base.ToString();
    super += "\nAllowance = " + Allowance;
    return super;
  }
}

//Admin Class
class Admin : Staff {
  //Fields
  private const float overtimeRate = 15.5F;
  private const float adminHourlyRate = 30;
  //Properties
  public float Overtime { get; private set; }
  //Constructors
  public Admin(string name) : base(name, adminHourlyRate) { /* nothing */ }
  //Methods
  public override void CalculatePay() {
    base.CalculatePay();
    if (HoursWorked > 160) {
      Overtime = overtimeRate * (HoursWorked - 160);
      TotalPay += Overtime;
    }
  }
  public override string ToString() {
    string super = base.ToString();
    super += "\nOvertime Rate = " + overtimeRate +
    "\nOvertime = " + Overtime;
    return super;
  }
}

//FileReader class
class FileReader {
  //Methods
  public List<Staff> ReadFile() {
    List<Staff> staff = new List<Staff>();
    if (File.Exists("staff.txt")) {
      using (StreamReader sr = new StreamReader("staff.txt")) {
        while (!sr.EndOfStream) {
          string line = sr.ReadLine();
          string[] filter = { "," };
          string[] split = line.Split(filter, StringSplitOptions.RemoveEmptyEntries);
          if (split[1].Equals("Manager")) {
            staff.Add(new Manager(split[0]));
          } else {
            staff.Add(new Admin(split[0]));
          }
        }
        sr.Close();
      }
    } else {
      Console.WriteLine("FileNotFoundException: File staff.txt Could Not Be Located In Directory");
    }
    return staff;
  }
}

//PaySlip Class
class PaySlip {
  //Fields
  private int month;
  private int year;
  //Enums
  enum MonthsOfYear {
    JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC
  }
  //Constructors
  public PaySlip(int payMonth, int payYear) {
    month = payMonth;
    year = payYear;
  }
  //Methods
  public void GeneratePaySlip(List<Staff> staff) {
    string path;
    foreach(Staff s in staff) {
      path = s.NameOfStaff + ".txt";
      StreamWriter sw = new StreamWriter(path);
      sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
      sw.WriteLine("==========================");
      sw.WriteLine("Name of Staff: {0}", s.NameOfStaff);
      sw.WriteLine("Hours Worked: {0}", s.HoursWorked);
      sw.WriteLine("");
      sw.WriteLine("Basic Pay: {0:C}", s.BasicPay);
      if (s.GetType() == typeof(Manager)) {
        sw.WriteLine("Allowance: {0:C}", ((Manager)s).Allowance);
      } else if (s.GetType() == typeof(Admin)) {
        sw.WriteLine("Overtime Pay: {0:C}", ((Admin)s).Overtime);
      }
      sw.WriteLine("");
      sw.WriteLine("==========================");
      sw.WriteLine("Total Pay: {0:C}", s.TotalPay);
      sw.WriteLine("==========================");
      sw.Close();
    }
  }
  public void GenerateSummary(List<Staff> staff) {
    var result =
      from s in staff
      where s.HoursWorked < 10
      select s;
    string path = "summary.txt";
    StreamWriter sw = new StreamWriter(path);
    sw.WriteLine("Staff with less than 10 working hours");
    sw.WriteLine("");
    foreach(Staff s in result) {
      sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", s.NameOfStaff, s.HoursWorked);
    }
    sw.Close();
  }
}

//Program Class
public class Program {
     ///main
    public static void Main(string[] args) {
    List<Staff> staff = new List<Staff>();
    FileReader fr = new FileReader();
    int month = 0, year = 0;
    while (year == 0) {
      Console.Write("\nPlease enter the year: ");
      try {
        string line = Console.ReadLine();
        year = Convert.ToInt32(line);
      } catch (FormatException) {
        Console.WriteLine("FormatException: user input cannot be successfully casted to Int32");
        year = 0;
      }
    }
    while (month == 0) {
      Console.Write("\nPlease enter the month: ");
      try {
        string line = Console.ReadLine();
        month = Convert.ToInt32(line);
      } catch (FormatException) {
        Console.WriteLine("FormatException: user input cannot be successfully casted to Int32");
        month = 0;
      }
      if (month < 1 || month > 12) {
        Console.WriteLine("Invalid Input: month must be a value in between 1 and 12 (inclusive)");
        month = 0;
      }
    }
    staff = fr.ReadFile();
    for (int i = 0; i < staff.Count; i++) {
      Staff s = staff[i];
      try {
        Console.Write("\nEnter hours worked for {0}: ", s.NameOfStaff);
        string input = Console.ReadLine();
         s.HoursWorked = Convert.ToInt32(input);
         s.CalculatePay();
         Console.WriteLine(s.ToString());
      } catch (Exception e) {
        Console.WriteLine(e.Message);
        i--;
      }
    }
    PaySlip ps = new PaySlip(month, year);
    ps.GeneratePaySlip(staff);
    ps.GenerateSummary(staff);
    Console.WriteLine("\nPress ENTER to Exit");
    Console.Read();
  }
}

}
