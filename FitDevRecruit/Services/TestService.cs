using System;
using System.Collections.Generic;
using FitDevRecruit.Models;

namespace FitDevRecruit.Services
{
    public class TestService
    {
        private readonly QuestionService _questionService;

        public TestService(QuestionService questionService)
        {
            _questionService = questionService;
        }

        // 시험 진행: 맞춤형 문제 출제, 답안 입력, 채점
        public void ConductTest(Candidate candidate, int questionCount = 10)
        {
            // 새로운 맞춤형 출제 메서드 사용
            var questions = _questionService.GetCustomizedQuestions(candidate.Team, candidate.ExperienceLevel, questionCount);
            candidate.Answers = new Dictionary<int, string>();
            
            // 카테고리별 점수 초기화
            int technical = 0, personality = 0, problemSolving = 0, coding = 0, design = 0, leadership = 0;

            foreach (var q in questions)
            {
                Console.WriteLine($"\n문제 {q.Id} ({q.Category}): {q.Text}");
                if (q.Type == QuestionType.MultipleChoice || q.Type == QuestionType.Personality)
                {
                    for (int i = 0; i < q.Choices.Count; i++)
                        Console.WriteLine($"  {i + 1}. {q.Choices[i]}");
                }
                Console.Write("답변: ");
                var answer = Console.ReadLine();
                candidate.Answers[q.Id] = answer;

                // 카테고리별 자동 채점
                var score = CalculateScore(q, answer);
                
                switch (q.Category)
                {
                    case QuestionCategory.Technical:
                        technical += score;
                        break;
                    case QuestionCategory.Coding:
                        coding += score;
                        break;
                    case QuestionCategory.Design:
                        design += score;
                        break;
                    case QuestionCategory.Personality:
                        personality += score;
                        break;
                    case QuestionCategory.Leadership:
                        leadership += score;
                        break;
                    case QuestionCategory.ProblemSolving:
                        problemSolving += score;
                        break;
                }
            }
            
            // 종합 점수 계산
            candidate.TechnicalScore = technical + coding + design;
            candidate.PersonalityScore = personality + leadership;
            candidate.ProblemSolvingScore = problemSolving;
        }

        private int CalculateScore(Question question, string? answer)
        {
            if (string.IsNullOrEmpty(answer)) return 0;

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                    if (int.TryParse(answer, out int idx) && question.CorrectChoiceIndex == idx - 1)
                        return 5; // 객관식 정답 시 5점
                    return 0;
                
                case QuestionType.Subjective:
                    // 주관식 자동 채점 기능 활용
                    return _questionService.AutoScoreSubjectiveQuestion(question.Id, answer);
                
                case QuestionType.Personality:
                    if (int.TryParse(answer, out int pScore) && pScore >= 1 && pScore <= 5)
                        return question.PersonalityScore ?? pScore; // 인성 점수 반영
                    return 0;
                
                default:
                    return 0;
            }
        }

        // 상세 채점 결과 반환 (피드백 포함)
        public (int score, List<string> matchedKeywords, string feedback) GetDetailedScore(Question question, string? answer)
        {
            if (string.IsNullOrEmpty(answer)) 
                return (0, new List<string>(), "답변이 없습니다.");

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                    if (int.TryParse(answer, out int idx) && question.CorrectChoiceIndex == idx - 1)
                        return (5, new List<string>(), "정답입니다!");
                    return (0, new List<string>(), "틀렸습니다.");
                
                case QuestionType.Subjective:
                    return _questionService.GetDetailedScoringResult(question.Id, answer);
                
                case QuestionType.Personality:
                    if (int.TryParse(answer, out int pScore) && pScore >= 1 && pScore <= 5)
                    {
                        var expectedScore = question.PersonalityScore ?? pScore;
                        var feedback = pScore == expectedScore ? "적절한 답변입니다." : "다른 관점도 고려해보세요.";
                        return (expectedScore, new List<string>(), feedback);
                    }
                    return (0, new List<string>(), "유효하지 않은 답변입니다.");
                
                default:
                    return (0, new List<string>(), "채점 불가능한 문항입니다.");
            }
        }

        // 종합 점수 계산 (자동 채점 포함)
        public (int totalScore, int technicalScore, int personalityScore, int problemSolvingScore) CalculateTotalScore(Candidate candidate)
        {
            if (candidate.Answers == null || candidate.Answers.Count == 0)
                return (0, 0, 0, 0);

            int technical = 0, personality = 0, problemSolving = 0, coding = 0, design = 0, leadership = 0;

            foreach (var answer in candidate.Answers)
            {
                var question = _questionService.GetQuestionById(answer.Key);
                if (question == null) continue;

                var score = CalculateScore(question, answer.Value);
                
                switch (question.Category)
                {
                    case QuestionCategory.Technical:
                        technical += score;
                        break;
                    case QuestionCategory.Coding:
                        coding += score;
                        break;
                    case QuestionCategory.Design:
                        design += score;
                        break;
                    case QuestionCategory.Personality:
                        personality += score;
                        break;
                    case QuestionCategory.Leadership:
                        leadership += score;
                        break;
                    case QuestionCategory.ProblemSolving:
                        problemSolving += score;
                        break;
                }
            }
            
            var totalTechnical = technical + coding + design;
            var totalPersonality = personality + leadership;
            var totalScore = totalTechnical + totalPersonality + problemSolving;

            return (totalScore, totalTechnical, totalPersonality, problemSolving);
        }
    }
} 