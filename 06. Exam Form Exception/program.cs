Assignment 06: Exam Form Submission with Custom Exception
-Throws custom exception if submitted after the deadline

using System;
using System.Text;

namespace ExamFormSubmission;

public sealed class DeadlineExpiredException : Exception
{
    public DateTime Deadline { get; }
    public DateTime SubmittedAt { get; }

    public DeadlineExpiredException(DateTime deadline, DateTime submittedAt)
        : base($"Exam form submission FAILED. Deadline was {deadline:dd-MM-yyyy HH:mm}, " +
               $"but submission attempted at {submittedAt:dd-MM-yyyy HH:mm}.")
    {
        Deadline = deadline;
        SubmittedAt = submittedAt;
    }
}

public sealed class ExamForm
{
    public string StudentName { get; }
    public string Subject { get; }
    public DateTime SubmittedAt { get; private set; }
    public bool IsSubmitted { get; private set; }

    public ExamForm(string student, string subject) =>
        (StudentName, Subject) = (student, subject);

    public string Submit(DateTime deadline)
    {
        DateTime now = DateTime.Now;
        SubmittedAt = now;

        if (now > deadline)
            throw new DeadlineExpiredException(deadline, now);

        IsSubmitted = true;
        return $"[OK] {StudentName}'s exam form for '{Subject}' submitted at {now:dd-MM-yyyy HH:mm}.";
    }
}

public static class Program
{
    public static void Main()
    {
        var output = new StringBuilder();

        // Case 1 : Valid submission (deadline in the future)
        var form1 = new ExamForm("Arjun", "Data Structures");
        try
        {
            output.AppendLine(form1.Submit(DateTime.Now.AddDays(3)));
        }
        catch (DeadlineExpiredException ex)
        {
            output.AppendLine("ERROR: " + ex.Message);
        }

        // Case 2 : Late submission (deadline already passed)
        var form2 = new ExamForm("Neha", "Operating Systems");
        try
        {
            output.AppendLine(form2.Submit(DateTime.Now.AddDays(-2)));
        }
        catch (DeadlineExpiredException ex)
        {
            output.AppendLine("ERROR: " + ex.Message);
            output.AppendLine($"  Late by: {(ex.SubmittedAt - ex.Deadline).TotalHours:F1} hours");
        }

        // Case 3 : Deadline is exactly now (1 second ago)
        var form3 = new ExamForm("Kabir", "DBMS");
        try
        {
            output.AppendLine(form3.Submit(DateTime.Now.AddSeconds(-1)));
        }
        catch (DeadlineExpiredException ex)
        {
            output.AppendLine("ERROR: " + ex.Message);
        }

        Console.Write(output.ToString());
    }
}
