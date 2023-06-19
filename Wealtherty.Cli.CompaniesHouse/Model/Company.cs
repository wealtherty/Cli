namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Company
{
    public string Type { get; set; }

    public string CompanyName { get; set; }

    public string CompanyNumber { get; set; }

    public string CompanyStatus { get; set; }

    public DateTime? DateOfCreation { get; set; }

    public DateTime? DateOfCessation { get; set; }

    public bool? HasBeenLiquidated { get; set; }

    public bool? HasCharges { get; set; }

    public bool? HasInsolvencyHistory { get; set; }

    public bool? IsCommunityInterestCompany { get; set; }
}