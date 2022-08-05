namespace WalkthroughApp_API.Entities;

public class Walkthrough
{
    public int Id { get; set; }
    public string WalkthroughName { get; set; }
    public JobTitle EmployeeRole { get; set; }
}