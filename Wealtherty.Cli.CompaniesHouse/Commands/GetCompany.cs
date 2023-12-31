﻿using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:company-get")]
public class GetCompany : Command
{
    [Option('n', "number", Required = true)]
    public string Number { get; set; }

    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var facade = serviceProvider.GetService<Facade>();

        await facade.CreateOfficersAndCompaniesAsync(Number, new CancellationToken());

    }
}