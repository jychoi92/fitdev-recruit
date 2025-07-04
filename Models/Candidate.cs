using System.Collections.Generic;

namespace FitDev.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Dictionary<int, string> Answers { get; set; } // <문제ID, 답변>
        public int TechnicalScore { get; set; } // 기술 평가 점수
        public int PersonalityScore { get; set; } // 인성 평가 점수
        public int ProblemSolvingScore { get; set; } // 문제 해결력 점수
    }
}
