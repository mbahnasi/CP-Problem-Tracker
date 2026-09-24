using System ;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
// suppose all months are 30 days for now
public class TheDate
{
   public int day { set; get; }
   public int month { set; get; }
   public int year { set; get; }


   public void MinusFromDay( int minus )
   {
       if (day > minus) day -= minus;
       else
       {
           day += 30 - minus;
           MinusFromMonth(1);   
       }
   }


   public void MinusFromMonth( int minus)
   {
       if (month > minus) month -= minus;
       else
       {
           month += 12 - minus;
           year--;
       }
   }


   public bool NewerThanOrEqualChecker(TheDate another)// check if the caller is Newer than or equal the argument
   {
       if (year == another.year && month == another.month && day == another.day) return true;
       if (year > another.year) return true;
       if (year == another.year)
       {
           if (month > another.month) return true;
           if (month == another.month)
           {
               if (day > another.day) return true;
           }
       }


       return false;
   }


}
public class OperationsOnDate
{
   public TheDate WhatIsToday()
   {
       TheDate d = new TheDate();
       Console.WriteLine("Please enter Today's Date. ");
       Console.WriteLine("Day: ");
       d.day = int.Parse(Console.ReadLine());
       Console.WriteLine("Month: ");
       d.month = int.Parse(Console.ReadLine());
       Console.WriteLine("Year: ");
       d.year = int.Parse(Console.ReadLine());
       return d;
   }
  
   public int GetDifferenceBetween2Days(TheDate old, TheDate newer)
   {
       int dif = 0;
       // day difference
       if (newer.day >= old.day) dif += newer.day - old.day;
       else
       {
           newer.day += 30 ;
           dif += newer.day - old.day;
           newer.day -= old.day;
           if (newer.month > 1) newer.month--;
           else
           {
               newer.month += 12;
               newer.month--;
               newer.year--;
           }
       }


       if (newer.month >= old.month) dif += 30 * (newer.month - old.month);
       else
       {
           newer.month += 12;
           dif += 30 *(newer.month - old.month);
           newer.month -= old.month;
           newer.year--;
       }


       dif += 365 * (newer.year - old.year);
       newer.year -= old.year;
       return dif;
   }


   /*public TheDate theDay31Before(TheDate today)
   {
      
   }*/
}
public class problem
{
   private string name;
   private string link;


   public string Name
   {
       get => name;
       set => name = value;
   }


   public string Link
   {
       get => link;
       set => link = value ;
   }
}
public class DayAndProblems
{
   private int prefix = 0;
   public TheDate date { set; get; }
   public int Prefix { set ; get; }
   public List<problem> ProblemsSolvedToday { set; get;}
  
}
public class JsonFileHandler
{
   private string jsonsave ;
   private string jsonpath;
   public string JsonSave {set; get;}
   public string JsonPath { set; get;}


   public JsonFileHandler(List<DayAndProblems> allSolved)
   {
       JsonPath = "data.json";
       JsonSave = JsonSerializer.Serialize<List<DayAndProblems>>(allSolved);
       File.WriteAllText(JsonPath, JsonSave);
   }
   public void InitJsonSave(ref List<DayAndProblems> all_solved)
   {
       JsonPath = "data.json";
       JsonSave = File.ReadAllText(JsonPath);
       all_solved = JsonSerializer.Deserialize<List<DayAndProblems>>(JsonSave);
   }


   public void UpdateJson(ref List<DayAndProblems> all_solved)
   {
       JsonSave = JsonSerializer.Serialize<List<DayAndProblems>>(all_solved);
       File.WriteAllText(JsonPath, JsonSave);
   }
   public void ResetTheCounter(ref List<DayAndProblems> allSolved)
   {
       allSolved.Clear();
       UpdateJson(ref allSolved);
   }
}
public interface IReader
{
   public void Do(List<DayAndProblems> allSolved);
}
public interface IWriter
{
   public void Do(ref List<DayAndProblems> allSolved, ref JsonFileHandler json, ref Streak curStreak);
}
public class AddProblem : IWriter
{
   private bool ProblemExistsAlready(problem p, List<DayAndProblems> all_solved)
   {
       foreach (DayAndProblems i in all_solved)
       {
           foreach (problem j in i.ProblemsSolvedToday)
           {
               if (j.Link == p.Link) return true;
           }
       }


       return false;
   }
   private bool NewDay(TheDate date, List<DayAndProblems> all_solved)
       {
           if (all_solved.Count == 0) return true;
           int lastIndex = all_solved.Count - 1;
           TheDate lastDate = all_solved[lastIndex].date;
           if (date == lastDate) return false;
           return true;
       }
   public problem InputProblemData()
       {
           problem p = new problem();
           Console.WriteLine("Please Enter The problem data.");
           Console.WriteLine("Name: ");
           p.Name = Console.ReadLine();
           Console.WriteLine("Link: ");
           p.Link = Console.ReadLine();
           return p;
       }
  
   public void Do(ref List<DayAndProblems> all_solved, ref JsonFileHandler json, ref Streak curStreak)
   {
       problem p = new problem();
       TheDate d = new TheDate();
       OperationsOnDate op = new OperationsOnDate();
       p = InputProblemData();
       d = op.WhatIsToday();
       if (ProblemExistsAlready(p, all_solved))
       {
           Console.WriteLine("This problem is already solved");
           return;
       }


       if (NewDay(d, all_solved))
       {
           var newDay = new DayAndProblems();
           newDay.date = d;
           newDay.ProblemsSolvedToday = new List<problem>();
           newDay.ProblemsSolvedToday.Add(p);
           if (all_solved.Count == 0) newDay.Prefix = 1;
           else newDay.Prefix = all_solved[all_solved.Count - 1].Prefix + 1;
           all_solved.Add(newDay);
          
       }
       else
       {
           int lastIdx = all_solved.Count - 1;
           all_solved[lastIdx].ProblemsSolvedToday.Add(p);
           all_solved[lastIdx].Prefix++;
       }
       json.UpdateJson(ref all_solved);
       curStreak.Length++;
       curStreak.LastSolved = d;
   }


}
public class ProblemsSolvedAllTime: IReader
{
   public void Do(List<DayAndProblems> all_solved)
   {
       if (all_solved.Count == 0)
       {
           Console.WriteLine($"You Solved {0} problems till now.");
           return;
       }
       int num = all_solved[all_solved.Count - 1].Prefix;
       Console.WriteLine($"You Solved {num} problems till now.");
   }
}
public class ProblemsSolvedLast30 : IReader
{
   public void Do(List<DayAndProblems> allSolved)
   {
       if (allSolved.Count == 0)
       {
           Console.WriteLine("You didn't Solve Problems last 30 days");
           return;
       }
       TheDate theDay31Before = new TheDate();
       OperationsOnDate op = new OperationsOnDate();
       theDay31Before = op.WhatIsToday();
       theDay31Before.MinusFromDay(31);
       DayAndProblems theNeededDay = new DayAndProblems();
       for (int i = allSolved.Count - 1; i >= 0; i--)
       {
           if (theDay31Before.NewerThanOrEqualChecker(allSolved[i].date))
           {
               theNeededDay = allSolved[i];
           }
       }


       int ans = 0;
       if (theNeededDay == null) ans = allSolved[allSolved.Count - 1].Prefix;
       else  ans = allSolved[allSolved.Count - 1].Prefix - theNeededDay.Prefix;
       Console.WriteLine($"You solved {ans} problems last 30 days.");
   }
}
public class Streak
{
   private bool isStreak =false;
   private int length = 0;
   private TheDate lastSolved = new TheDate();


   public TheDate LastSolved
   { get => lastSolved; set => lastSolved = value; }
   public int Length
   { get => length; set => length = value; }
   public bool IsStreak
   { get => isStreak; set => isStreak = value; }
  
   private bool IsOnStreak(TheDate Today)
   {
       bool ans = false;
       OperationsOnDate obj = new OperationsOnDate();
       int dif = obj.GetDifferenceBetween2Days(LastSolved, Today);
       if (dif <= 1) ans = true;
       return ans;
   }


   public void YourStreak(List<DayAndProblems> all_solved)
   {
       if (all_solved.Count == 0)
       {
           Console.WriteLine("No Streak");
       }
       else
       {
           OperationsOnDate op = new OperationsOnDate();
           TheDate Today = new TheDate();
           Today = op.WhatIsToday();
           if (IsOnStreak(Today))
           {
               Console.WriteLine($"Your Streak is {length} days.");
           }
           else
           {
               length = 0;
               Console.WriteLine("No Streak");
           }
       }
   }
}
public class Initializer
{
   public void Init()
   {
       List<DayAndProblems> allSolved = new List<DayAndProblems>();
       JsonFileHandler json = new JsonFileHandler(allSolved);
       Streak curStreak = new Streak();  
       json.InitJsonSave(ref allSolved);
       
       while (true)
       {
           // initializing the interface
           Console.WriteLine("Hello, This is Your Problem Counter.\n\n");
           Console.WriteLine("You can do the following Operations.\n\n");
           Console.WriteLine("1 : Add a problem.\n");
           Console.WriteLine("2 : Check how many problems you solved all time.\n");
           Console.WriteLine("3 : Check how many problems you solved last 30 days.\n");
           Console.WriteLine("4 : Check your streak.\n");
           Console.WriteLine("Please enter the number of the operation you need to perform.\n");
           int n = int.Parse(Console.ReadLine());
           IWriter op;
           IReader op2;
           if (n == 1)
           {
               op = new AddProblem();
               op.Do(ref allSolved, ref json, ref curStreak);
           }
           else if (n == 2)
           {
               op2 = new ProblemsSolvedAllTime();
               op2.Do(allSolved);
           }
           else if (n == 3)
           {
               op2 = new ProblemsSolvedLast30();
               op2.Do(allSolved);
           }
           else
           {
               curStreak.YourStreak(allSolved);
           }


           json.UpdateJson(ref allSolved);
           Console.WriteLine("Do you want to make another Operation? If yes enter Y else enter N.\n\n");
           if (char.Parse(Console.ReadLine()) == 'N') break;
       }
   }
}
public class Program
{
   public static void Main(string[] args)
   {
       Initializer I = new Initializer();
       I.Init();
   }
}

