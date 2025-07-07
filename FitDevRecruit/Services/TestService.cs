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
                    // 주관식은 간단히 정답 문자열 비교 (실제론 더 정교한 평가 필요)
                    if (!string.IsNullOrEmpty(question.Answer) && answer.Trim().ToLower() == question.Answer.Trim().ToLower())
                        return 5; // 주관식 정답 시 5점
                    return 2; // 부분 점수
                
                case QuestionType.Personality:
                    if (int.TryParse(answer, out int pScore) && pScore >= 1 && pScore <= 5)
                        return question.PersonalityScore ?? pScore; // 인성 점수 반영
                    return 0;
                
                default:
                    return 0;
            }
        }
    }
} 