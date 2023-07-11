using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission.Graph.Model;

public class Appointment : Relationship<Charity, Trustee>
{
    public bool IsChair { get; set; }

    public DateTime? AppointedOn { get; set; }
    
    public Appointment(Charity parent, Trustee child, Api.Model.Trustee trustee) : base(parent, child)
    {
        AppointedOn = trustee.AppointedOn;
        IsChair = trustee.IsChair;
    }
    
    protected override string GetName() => "HAS_TRUSTEE";
}

