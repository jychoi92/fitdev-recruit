using System;
using System.Collections.Generic;
using System.Linq;
using FitDevRecruit.Models;

namespace FitDevRecruit.Services
{
    public class QuestionService
    {
        private List<Question> _questions;
        private Random _random = new Random();

        public QuestionService()
        {
            // 하드코딩된 예시 문제 (실제 구현 시 JSON 등에서 불러올 수도 있음)
            _questions = new List<Question>
            {
                // 신입용 문제
                new Question { Id = 1, Text = "C#에서 string과 String의 차이는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior },
                new Question { Id = 2, Text = "Java의 기본 자료형이 아닌 것은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Choices = new List<string>{"int", "float", "String", "char"}, CorrectChoiceIndex = 2 },
                new Question { Id = 3, Text = "협업 시 가장 중요한 태도는?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Choices = new List<string>{"소통", "속도", "완벽함", "독립성"}, PersonalityScore = 1 },
                new Question { Id = 4, Text = "HTML에서 가장 기본적인 태그는?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Choices = new List<string>{"<div>", "<html>", "<body>", "<head>"}, CorrectChoiceIndex = 1 },
                new Question { Id = 5, Text = "개발자로서 성장하기 위해 가장 중요한 것은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Choices = new List<string>{"학습", "경험", "네트워킹", "모든 것"}, PersonalityScore = 4 },
                
                // 경력용 문제
                new Question { Id = 6, Text = "마이크로서비스 아키텍처의 장단점을 설명하세요.", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior },
                new Question { Id = 7, Text = "대용량 데이터 처리 시 성능 최적화 방법은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Choices = new List<string>{"인덱싱", "캐싱", "파티셔닝", "모든 것"}, CorrectChoiceIndex = 3 },
                new Question { Id = 8, Text = "팀 리더로서 갈등 해결 방법은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Senior, Choices = new List<string>{"직접 개입", "중재", "무시", "회피"}, PersonalityScore = 1 },
                new Question { Id = 9, Text = "CI/CD 파이프라인 구축 시 고려사항은?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior },
                new Question { Id = 10, Text = "시스템 설계 시 확장성을 고려한 패턴은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Choices = new List<string>{"싱글톤", "팩토리", "옵저버", "모든 것"}, CorrectChoiceIndex = 3 },
            };
        }

        public List<Question> GetAllQuestions()
        {
            return _questions;
        }

        public List<Question> GetRandomQuestions(int count)
        {
            return _questions.OrderBy(x => _random.Next()).Take(count).ToList();
        }

        public Question? GetQuestionById(int id)
        {
            return _questions.FirstOrDefault(q => q.Id == id);
        }

        public List<Question> GetQuestionsByLevel(ExperienceLevel level, int count = 5)
        {
            return _questions.Where(q => q.TargetLevel == level)
                           .OrderBy(x => _random.Next())
                           .Take(count)
                           .ToList();
        }
    }
} 