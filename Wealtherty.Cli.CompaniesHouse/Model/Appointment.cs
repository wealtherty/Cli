using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Appointment : Relationship<Officer, Company>
{
    private readonly Company _company;
    private readonly global::CompaniesHouse.Response.Appointments.Appointment _appointment;

    public Appointment(Officer parent, Company child, global::CompaniesHouse.Response.Appointments.Appointment appointment) : base(parent, child)
    {
        _company = child;
        _appointment = appointment;

        From = _appointment.AppointedOn.ToNeo4jDate();
        To = _appointment.ResignedOn.ToNeo4jDate();
        Role = _appointment.OfficerRole.ToString();
    }
    
    public string From { get; set; }
        
    public string To { get; set; }
    
    public string Role { get; set; }


    protected override string GetName()
    {
        if (_company.Status.Equals("Dissolved", StringComparison.OrdinalIgnoreCase))
        {
            return "WORKED_FOR";
        }

        return _appointment.ResignedOn.HasValue ? "WORKED_FOR" : "WORKS_FOR";
    }
}

