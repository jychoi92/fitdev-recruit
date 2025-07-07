using System;
using FitDevRecruit.Models;

namespace FitDevRecruit.Services
{
    public class ReportService
    {
        public Report GenerateReport(Candidate candidate)
        {
            // 실제 출제된 문제 개수와 각 문항별 최대점수로 만점 계산
            int technicalMax = candidate.TechnicalMaxScore > 0 ? candidate.TechnicalMaxScore : 0;
            int personalityMax = candidate.PersonalityMaxScore > 0 ? candidate.PersonalityMaxScore : 0;
            int problemSolvingMax = candidate.ProblemSolvingMaxScore > 0 ? candidate.ProblemSolvingMaxScore : 0;

            // 100점 만점 환산 (문항이 없으면 0점)
            int technicalScore100 = technicalMax > 0 ? (int)Math.Round(candidate.TechnicalScore * 100.0 / technicalMax) : 0;
            int personalityScore100 = personalityMax > 0 ? (int)Math.Round(candidate.PersonalityScore * 100.0 / personalityMax) : 0;
            int problemSolvingScore100 = problemSolvingMax > 0 ? (int)Math.Round(candidate.ProblemSolvingScore * 100.0 / problemSolvingMax) : 0;

            string summary = $"기술 점수: {technicalScore100}/100  인성 점수: {personalityScore100}/100  문제해결력 점수: {problemSolvingScore100}/100\n";

            // 종합 평가 예시
            if (technicalScore100 >= 60 && personalityScore100 >= 60 && problemSolvingScore100 >= 60)
                summary += "\n종합 평가: 우수 지원자입니다.";
            else
                summary += "\n종합 평가: 추가 검토가 필요합니다.";

            return new Report
            {
                CandidateId = candidate.Id,
                CandidateName = candidate.Name,
                TestDate = DateTime.Now,
                TechnicalScore = technicalScore100,
                PersonalityScore = personalityScore100,
                ProblemSolvingScore = problemSolvingScore100,
                Summary = summary,
                CandidateEmail = candidate.Email,
                CandidateLevel = candidate.ExperienceLevel.ToString(),
                CandidateTeam = candidate.Team.ToString()
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