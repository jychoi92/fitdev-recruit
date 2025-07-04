using System;

namespace FitDev.Models
{
    public class Report
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public DateTime TestDate { get; set; }
        public int TechnicalScore { get; set; }
        public int PersonalityScore { get; set; }
        public int ProblemSolvingScore { get; set; }
        public string Summary { get; set; } // 종합 평가 요약
    }
}
