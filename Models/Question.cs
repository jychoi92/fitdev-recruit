using System.Collections.Generic;

namespace FitDev.Models
{
    public enum QuestionType
    {
        MultipleChoice,
        Subjective,
        Personality
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } // 문제 내용
        public QuestionType Type { get; set; } // 문제 유형
        public List<string> Choices { get; set; } // 객관식 선택지 (주관식/인성은 null)
        public int? CorrectChoiceIndex { get; set; } // 객관식 정답 인덱스 (주관식/인성은 null)
        public string? Answer { get; set; } // 주관식 정답 (객관식/인성은 null)
        public int? PersonalityScore { get; set; } // 인성 문제 점수(1~5 등, 기타는 null)
    }
}
