﻿using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Company : Node
{
    public string Number { get; set; }
    
    public string Name { get; set; }
    
    public string Status { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    [JsonConverter(typeof(DateConverter))]
    public DateTime? CreatedOn { get; set; }

    [JsonConverter(typeof(DateConverter))]
    public DateTime? StoppedTradingOn { get; set; }

    protected override object GetMatchObject() => new { Number };
}