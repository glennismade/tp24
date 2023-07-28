using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Serilog;

namespace tp24
{

    [Route("api/receivables")]
    [ApiController]
    public class ReceivablesController : ControllerBase
    {

        private static List<Receivable> receivables = new List<Receivable>();
        private readonly List<Receivable> _receivables;

        public ReceivablesController(List<Receivable> receivables)
        {
            _receivables = receivables;
        }

        [HttpPost]
        public IActionResult AddReceivable([FromBody] List<Receivable> receivableData)
        {
            if (receivableData == null || receivableData.Count == 0)
            {
                return new BadRequestResult();
            }

            var duplicateReferences = receivableData
                .Where(r => r.Reference != null && r.CurrencyCode != null && r.IssueDate != null && r.DueDate != null && r.DebtorName != null && r.DebtorReference != null)
                .Select(r => r.Reference)
                .Intersect(receivables.Select(r => r.Reference))
                .ToList();

            if (duplicateReferences.Any())
            {
                Log.Warning("Received duplicate references: {References}", string.Join(", ", duplicateReferences));

                var duplicateReferenceResponse = new AddReceivableResponse
                {
                    Reference = string.Join(",", duplicateReferences),
                    CurrentDate = DateTime.UtcNow,
                    Value = GetOpeningValueOrDefault(receivableData)
                };

                return Conflict(duplicateReferenceResponse);
            }

            var invalidReceivables = receivableData
                .Where(r => r.Reference == null || r.CurrencyCode == null || r.IssueDate == null || r.DueDate == null || r.DebtorName == null || r.DebtorReference == null)
                .ToList();

            if (invalidReceivables.Any())
            {
                Log.Warning("Invalid receivables. Payload is missing required fields.");
                return new BadRequestResult();
            }

            receivables.AddRange(receivableData);
            Log.Information("Data successfully added");

            var responseData = new AddReceivableResponse
            {
                Reference = receivableData.FirstOrDefault()?.Reference,
                CurrentDate = DateTime.UtcNow,
                Value = GetOpeningValueOrDefault(receivableData)
            };

            return Created("", responseData);
        }

        [HttpGet("summary")]
        public IActionResult GetReceivablesSummary()
        {

            try
            {

                double totalOpenInvoices = receivables.Where(r => r.ClosedDate == null && !r.Cancelled).Sum(r => r.OpeningValue);
                double totalClosedInvoices = receivables.Where(r => r.ClosedDate != null && !r.Cancelled).Sum(r => r.PaidValue);

                Log.Information("Summary - Total Open: {TotalOpenInvoices}, Total Closed: {TotalClosedInvoices}", totalOpenInvoices, totalClosedInvoices);

                var summary = new
                {
                    TotalOpenInvoices = totalOpenInvoices,
                    TotalClosedInvoices = totalClosedInvoices,
                    CurrentTime = DateTime.Now
                };

                return Ok(summary);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred while fetching data");
                return StatusCode(500, "An error occurred while fetching data");
            }
        }

        private double GetOpeningValueOrDefault(List<Receivable> receivables)
        {
            double totalOpeningValue = 0.0;
            if (receivables != null && receivables.Count > 0)
            {
                foreach (var receivable in receivables)
                {
                    totalOpeningValue += receivable.OpeningValue;
                }
            }
            return totalOpeningValue;
        }
    }
}