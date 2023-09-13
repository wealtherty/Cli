﻿using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Graph.Model;

public class SicCode : Node
{
    public string Code { get; set; }
    
    public string Category { get; set; }
    
    public string Description { get; set; }
}