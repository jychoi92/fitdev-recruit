using System.Collections.Generic;

namespace FitDevRecruit.Models
{
    public enum ExperienceLevel
    {
        Junior,     // 신입
        Senior      // 경력
    }

    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ExperienceLevel ExperienceLevel { get; set; } = ExperienceLevel.Junior; // 경력 구분
        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>(); // <문제ID, 답변>
        public int TechnicalScore { get; set; } // 기술 평가 점수
        public int PersonalityScore { get; set; } // 인성 평가 점수
        public int ProblemSolvingScore { get; set; } // 문제 해결력 점수
    }
} 