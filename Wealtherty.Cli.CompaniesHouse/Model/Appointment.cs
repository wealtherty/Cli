﻿using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Appointment : Relationship<Officer, Company>
{
    private readonly Company _company;
    private readonly global::CompaniesHouse.Response.Appointments.Appointment _appointment;

    public Appointment(Officer parent, Company child, global::CompaniesHouse.Response.Appointments.Appointment appointment) : base(parent, child)
    {
        _company = child;
        _appointment = appointment;

        From = _appointment.AppointedOn;
        To = _appointment.ResignedOn;
        Role = _appointment.OfficerRole.ToString();
        Occupation = _appointment.Occupation;
    }


    [JsonConverter(typeof(DateConverter))]
    public DateTime? From { get; set; }
        
    [JsonConverter(typeof(DateConverter))]
    public DateTime? To { get; set; }
    public string Role { get; set; }
    public string Occupation { get; set; }
    
    protected override string GetName()
    {
        if (_company.Status.Equals("Dissolved", StringComparison.OrdinalIgnoreCase))
        {
            return "WORKED_FOR";
        }

        return _appointment.ResignedOn.HasValue ? "WORKED_FOR" : "WORKS_FOR";
    }
}

