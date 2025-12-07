using System;
using System.Collections.Generic;

namespace SMO.Application.Features.Performance.DTOs
{
    public class MacroeconomicIndicatorDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Unit { get; set; }
        public decimal? BaselineValue { get; set; }
        public DateTime? BaselineDate { get; set; }
        public decimal? TargetValue2030 { get; set; }
        public string CalculationFormula { get; set; }
        public string Polarity { get; set; }
        public string MeasurementFrequency { get; set; }
        public string ReferenceSource { get; set; }
        public string ComparisonCountries { get; set; }
        public decimal? CurrentValue { get; set; }
        public decimal? PerformanceScore { get; set; }
        public string Status { get; set; }
        public List<MacroeconomicIndicatorValueDto> RecentValues { get; set; }
    }

    public class MacroeconomicIndicatorValueDto
    {
        public int Id { get; set; }
        public int MacroeconomicIndicatorId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? ForecastValue { get; set; }
        public decimal? PerformanceScore { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class UpdateMEIValueRequest
    {
        public int IndicatorId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? ForecastValue { get; set; }
        public string Description { get; set; }
    }
}
