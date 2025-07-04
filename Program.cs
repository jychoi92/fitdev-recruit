using System;
using FitDev.Models;
using FitDev.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== 인적성 평가 시스템 ===\n");

        // 지원자 정보 입력
        var candidate = new Candidate();
        Console.Write("이름: ");
        candidate.Name = Console.ReadLine();
        Console.Write("이메일: ");
        candidate.Email = Console.ReadLine();
        candidate.Id = new Random().Next(1000, 9999); // 간단한 ID 부여

        // 서비스 초기화
        var questionService = new QuestionService();
        var testService = new TestService(questionService);
        var reportService = new ReportService();

        // 시험 진행
        testService.ConductTest(candidate, 5); // 5문제 출제

        // 리포트 생성 및 출력
        var report = reportService.GenerateReport(candidate);
        reportService.PrintReport(report);

        Console.WriteLine("프로그램을 종료하려면 아무 키나 누르세요...");
        Console.ReadKey();
    }
}
