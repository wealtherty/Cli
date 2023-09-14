using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Model.Graph;

public class Appointment : Relationship<Company, Officer>
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

    public Appointment(Company company, Officer officer, global::CompaniesHouse.Response.Appointments.Appointment resource) : base(company, officer)
    {
        Resource = resource;
        
        From = resource.AppointedOn;
        To = resource.ResignedOn;
        Role = resource.OfficerRole.ToString();
        Occupation = resource.Occupation;
        
        if (company.Status.Equals("Dissolved", StringComparison.OrdinalIgnoreCase))
        {
            IsCurrent = false;
        }
        else
        {
            IsCurrent = !Resource.ResignedOn.HasValue;
        }
    }
    
    protected override string GetName() => IsCurrent ?  "HAS_OFFICER" : "HAD_OFFICER";
}

