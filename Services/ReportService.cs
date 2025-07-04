using System;
using FitDev.Models;

namespace FitDev.Services
{
    public class ReportService
    {
        public Report GenerateReport(Candidate candidate)
        {
            string summary = $"기술 점수: {candidate.TechnicalScore}\n" +
                             $"인성 점수: {candidate.PersonalityScore}\n" +
                             $"문제해결력 점수: {candidate.ProblemSolvingScore}\n";

            // 간단한 종합 평가 예시
            if (candidate.TechnicalScore >= 60 && candidate.PersonalityScore >= 10 && candidate.ProblemSolvingScore >= 20)
                summary += "\n종합 평가: 우수 지원자입니다.";
            else
                summary += "\n종합 평가: 추가 검토가 필요합니다.";

            return new Report
            {
                CandidateId = candidate.Id,
                CandidateName = candidate.Name,
                TestDate = DateTime.Now,
                TechnicalScore = candidate.TechnicalScore,
                PersonalityScore = candidate.PersonalityScore,
                ProblemSolvingScore = candidate.ProblemSolvingScore,
                Summary = summary
            };
        }

        public void PrintReport(Report report)
        {
            Console.WriteLine("\n===== 평가 결과 리포트 =====");
            Console.WriteLine($"지원자: {report.CandidateName}");
            Console.WriteLine($"응시일: {report.TestDate}");
            Console.WriteLine($"기술 점수: {report.TechnicalScore}");
            Console.WriteLine($"인성 점수: {report.PersonalityScore}");
            Console.WriteLine($"문제해결력 점수: {report.ProblemSolvingScore}");
            Console.WriteLine($"\n{report.Summary}");
            Console.WriteLine("========================\n");
        }
    }
}
