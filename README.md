# Wealtherty

## Think Tanks
### SIC Code Analysis 

**SIC Codes**

*TODO*

**Think Tanks**

- List of Think Tanks were created from [On Think Tanks (OTT)](https://onthinktanks.org/).  See [OTT full dataset](https://airtable.com/app5Tu5McTOQC3pYw/shrnWdKAQxofzjZg4/tblr8Lc3OsoygJ7og).
- Political Wing was assigned to each Think Tank
- See [ThinkTanks](Wealtherty.ThinkTanks/Resources/ThinkTanks.csv)

**Companies**

*TODO*

- See [Companies](Wealtherty.ThinkTanks/Resources/Companies.csv)

**Appointments**

*TODO*

- See [Appointments](Wealtherty.ThinkTanks/Resources/Appointments.csv)

**Notes**
- While the Fabian Society is included in the list of Think Tanks, it is exlucded from the list of Companies because, as an [unincorporated membership](https://fabians.org.uk/about-us/accountability/), it isn't registered at Companies House.
- Companies without any SIC Codes were ignored.
- Officers who are third-party service providers, typically Nominee Directors and Secretaries, were ignored.
- Unrecognised SIC Codes were ignored, eg 7260, 7499 and 8514.  We can only assume that these codes were once valid but company records weren't updated when they become invalid.
- Companies that are now dormant are moved to SIC Code 99999.  Unfortunately, we're unable to determine what their original values were.
- Progress Britain doesn't have a date_founded within OTT.
- Companies House doesn't have Appointed-On dates for some Appointments.  See John Hedley Greenborough (https://find-and-update.company-information.service.gov.uk/officers/XkeESSrDDictj_iY0NlXVcXH0tY/appointments).  
- Many companies don't have SIC Codes, for various reasons.  For example, a dissolved company might not have any codes.  And, it isn't necessary for a LLP to have any codes.  Rather than ignore appointments of companies without any codes (or with unrecognised codes), I've assigned categorised them as Unknown.  This will skew the data and so Unknown warrents separate analysis