using System;
using FitDevRecruit.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitDevRecruit.Services
{
    public class ReportService
    {
        private static readonly string ResultsFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "results.json");

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

        // 평가 결과를 파일에 저장
        public void SaveReportToFile(Report report)
        {
            List<Report> reports = new List<Report>();
            if (File.Exists(ResultsFilePath))
            {
                var json = File.ReadAllText(ResultsFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                    reports = JsonSerializer.Deserialize<List<Report>>(json) ?? new List<Report>();
            }
            reports.Add(report);
            File.WriteAllText(ResultsFilePath, JsonSerializer.Serialize(reports, new JsonSerializerOptions { WriteIndented = true }));
        }

        // 전체 평가 결과 불러오기
        public List<Report> LoadAllReports()
        {
            if (!File.Exists(ResultsFilePath)) return new List<Report>();
            var json = File.ReadAllText(ResultsFilePath);
            if (string.IsNullOrWhiteSpace(json)) return new List<Report>();
            return JsonSerializer.Deserialize<List<Report>>(json) ?? new List<Report>();
        }
    }
} 