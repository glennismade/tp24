
using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using tp24;

namespace tests;

[TestFixture]
public class ReceivablesControllerTests
{
    private ReceivablesController _controller;

    [SetUp]
    public void Setup()
    {
        var receivables = new List<Receivable>
        {
            new Receivable
            {
                Reference = "ref1",
                CurrencyCode = "USD",
                IssueDate = DateTime.UtcNow.ToString(),
                DueDate = DateTime.UtcNow.AddDays(30).ToString(),
                DebtorName = "Debtor 1",
                DebtorReference = "debtor1-ref"
            },
            new Receivable
            {
                Reference = "ref2",
                CurrencyCode = "EUR",
                IssueDate = DateTime.UtcNow.ToString(),
                DueDate = DateTime.UtcNow.AddDays(30).ToString(),
                DebtorName = "Debtor 2",
                DebtorReference = "debtor2-ref"
            }
        };

        _controller = new ReceivablesController(receivables);
    }

    [Test]
    public void AddReceivableWithValidDataReturnsCreatedResult()
    {
        var receivableData = new List<Receivable>
        {
            new Receivable
            {
                Reference = "ref3",
                CurrencyCode = "GBP",
                IssueDate = DateTime.UtcNow.ToString(),
                DueDate = DateTime.UtcNow.AddDays(60).ToString(),
                DebtorName = "Debtor 3",
                DebtorReference = "debtor3-ref"
            }
        };

        var result = _controller.AddReceivable(receivableData) as CreatedResult;

        Assert.NotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(3, result.Value.GetType().GetProperties().Length);
    }

    [Test]
    public void AddReceivableWithDuplicateReferencesReturnsConflictResult()
    {
        var existingReceivable = new List<Receivable>
        {
            new Receivable
            {

            Reference = "ref1",
            CurrencyCode = "GBP",
            IssueDate = DateTime.UtcNow.ToString(),
            DueDate = DateTime.UtcNow.AddDays(60).ToString(),
            DebtorName = "Debtor 1",
            DebtorReference = "debtor1-ref"
            }
        };

        _controller.AddReceivable(existingReceivable);

        var receivableData = new List<Receivable>
        {
            new Receivable
            {
                Reference = "ref1", // this reference already exists in system.
                CurrencyCode = "GBP",
                IssueDate = DateTime.UtcNow.ToString(),
                DueDate = DateTime.UtcNow.AddDays(60).ToString(),
                DebtorName = "Debtor 4",
                DebtorReference = "debtor4-ref"
            }
        };

        var result = _controller.AddReceivable(receivableData);

        Assert.NotNull(result);
        Assert.IsInstanceOf<ConflictObjectResult>(result);
        var conflictResult = result as ConflictObjectResult;
        Assert.AreEqual(409, conflictResult.StatusCode);
        Assert.IsInstanceOf<AddReceivableResponse>(conflictResult.Value);

    }

    [Test]
    public void AddReceivableWithInvalidDataReturnsBadRequestResult()
    {
        var receivableData = new List<Receivable>
        {
            new Receivable
            {
                Reference = "ref5",
                CurrencyCode = "GBP",
                IssueDate = DateTime.UtcNow.ToString(),
                DueDate = DateTime.UtcNow.AddDays(60).ToString()
                // missing DebtorName and DebtorReference 
            }
        };

        var result = _controller.AddReceivable(receivableData) as BadRequestResult;

        Assert.NotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [Test]
    public void GetReceivablesSummaryReturnsSummaryObject()
    {
        var result = _controller.GetReceivablesSummary() as OkObjectResult;

        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(3, result.Value.GetType().GetProperties().Length);
    }
}
