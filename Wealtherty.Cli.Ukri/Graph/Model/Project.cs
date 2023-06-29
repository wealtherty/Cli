using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Ukri.Graph.Model;

public class Project : Node
{
    public string ProjectId { get; set; }
    
    public string Title { get; set; }
    
    public string Status { get; set; }
    
    public string GrantCategory { get; set; }
    
    public string LeadFunder { get; set; }
    
    public string LeadOrganisationDepartment { get; set; }

}