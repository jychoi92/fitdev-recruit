using System;
using System.Collections.Generic;
using System.Linq;
using FitDev.Models;

namespace FitDev.Services
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
                new Question { Id = 1, Text = "C#에서 string과 String의 차이는?", Type = QuestionType.Subjective },
                new Question { Id = 2, Text = "Java의 기본 자료형이 아닌 것은?", Type = QuestionType.MultipleChoice, Choices = new List<string>{"int", "float", "String", "char"}, CorrectChoiceIndex = 2 },
                new Question { Id = 3, Text = "협업 시 가장 중요한 태도는?", Type = QuestionType.Personality, Choices = new List<string>{"소통", "속도", "완벽함", "독립성"}, PersonalityScore = 1 },
                // 추가 문제...
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
    }
}
