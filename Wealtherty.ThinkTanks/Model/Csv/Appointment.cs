﻿using CsvHelper.Configuration.Attributes;
using Wealtherty.ThinkTanks.Model.Graph;

namespace Wealtherty.ThinkTanks.Model.Csv;

public class Appointment
{

    public string ThinkTankId { get; set; }
    
    public PoliticalWing ThinkTankPoliticalWing { get; set; }
        
    public string ThinkTankName { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? ThinkTankFoundedOn { get; set; }
        
        
    public string OfficerId { get; set; }
        
    public string OfficerName { get; set; }
        
    public string OfficerRole { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? OfficerAppointedOn { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? OfficerResignedOn { get; set; }

        
    public string CompanyNumber { get; set; }
        
    public string CompanyName { get; set; }
    
    public string CompanyType { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? CompanyDateOfCreation { get; set; }
        
    [Format("dd/MM/yyyy")]
    public DateTime? CompanyDateOfCessation { get; set; }
        
    public string CompanySicCode { get; set; }
        
        
    public string CompanySicCodeCategory { get; set; }
        
    public string CompanySicCodeDescription { get; set; }
    
}