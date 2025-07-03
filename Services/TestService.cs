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

        // 시험 진행: 문제 출제, 답안 입력, 채점
        public void ConductTest(Candidate candidate, int questionCount = 5)
        {
            var questions = _questionService.GetRandomQuestions(questionCount);
            candidate.Answers = new Dictionary<int, string>();
            int technical = 0, personality = 0, problemSolving = 0;

            foreach (var q in questions)
            {
                Console.WriteLine($"\n문제 {q.Id}: {q.Text}");
                if (q.Type == QuestionType.MultipleChoice || q.Type == QuestionType.Personality)
                {
                    for (int i = 0; i < q.Choices.Count; i++)
                        Console.WriteLine($"  {i + 1}. {q.Choices[i]}");
                }
                Console.Write("답변: ");
                var answer = Console.ReadLine();
                candidate.Answers[q.Id] = answer;

                // 자동 채점
                if (q.Type == QuestionType.MultipleChoice && int.TryParse(answer, out int idx))
                {
                    if (q.CorrectChoiceIndex == idx - 1) technical += 20; // 예시: 1문제 20점
                }
                else if (q.Type == QuestionType.Subjective)
                {
                    // 주관식은 간단히 정답 문자열 비교(실제론 평가 필요)
                    if (!string.IsNullOrEmpty(q.Answer) && answer?.Trim() == q.Answer.Trim())
                        problemSolving += 20;
                }
                else if (q.Type == QuestionType.Personality && int.TryParse(answer, out int pScore))
                {
                    personality += pScore; // 1~5점 등급 가정
                }
            }
            candidate.TechnicalScore = technical;
            candidate.PersonalityScore = personality;
            candidate.ProblemSolvingScore = problemSolving;
        }
    }
} 