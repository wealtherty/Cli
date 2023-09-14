﻿using CsvHelper.Configuration.Attributes;
using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.Cli.Bridge.Model.Csv;

public class ThinkTankAppointment
{
    public PoliticalWing ThinkTankPoliticalWing { get; set; }
        
    public string ThinkTankName { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime ThinkTankFoundedOn { get; set; }
        
        
    public string OfficerId { get; set; }
        
    public string OfficerName { get; set; }
        
    public string OfficerRole { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? OfficerAppointedOn { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? OfficerResignedOn { get; set; }

        
    public string CompanyNumber { get; set; }
        
    public string CompanyName { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? CompanyDateOfCreation { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? CompanyDateOfCessation { get; set; }
        
    public string CompanySicCode { get; set; }
        
        
    public string CompanySicCodeCategory { get; set; }
        
    public string CompanySicCodeDescription { get; set; }
        
        
}