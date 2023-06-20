﻿namespace Wealtherty.Cli.Ukri.Api.Model;

public class Project
{
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    public string Status { get; set; }
    
    public string GrantCategory { get; set; }
    
    public string leadFunder { get; set; }
    
    public string LeadOrganisationDepartment { get; set; }
    
    public string AbstractText { get; set; }
}