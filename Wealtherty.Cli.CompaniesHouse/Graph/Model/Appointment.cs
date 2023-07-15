using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Graph.Model;

public class Appointment : Relationship<Officer, Company>
{

    [JsonConverter(typeof(DateConverter))]
    public DateTime? From { get; set; }
        
    [JsonConverter(typeof(DateConverter))]
    public DateTime? To { get; set; }
    public string Role { get; set; }
    public string Occupation { get; set; }
    
    public bool IsCurrent { get; set; }
    
    [JsonIgnore]
    public global::CompaniesHouse.Response.Appointments.Appointment Resource { get; }

    public Appointment(Officer parent, Company child, global::CompaniesHouse.Response.Appointments.Appointment resource) : base(parent, child)
    {
        Resource = resource;
        
        From = resource.AppointedOn;
        To = resource.ResignedOn;
        Role = resource.OfficerRole.ToString();
        Occupation = resource.Occupation;
        
        if (child.Status.Equals("Dissolved", StringComparison.OrdinalIgnoreCase))
        {
            IsCurrent = false;
        }
        else
        {
            IsCurrent = !Resource.ResignedOn.HasValue;
        }
    }
    
    protected override string GetName() => IsCurrent ?  "WORKS_FOR" : "WORKED_FOR";
}

