using System.Collections.Generic;

namespace FitDevRecruit.Models
{
    public enum QuestionType
    {
        MultipleChoice,
        Subjective,
        Personality
    }

    public enum QuestionCategory
    {
        Technical,      // 기술
        Coding,         // 코딩테스트
        Design,         // 설계/아키텍처
        Personality,    // 인성/성향
        Leadership,     // 리더십
        ProblemSolving  // 문제해결력
    }

    public enum TeamType
    {
        Backend,
        Frontend,
        Data,
        Infra
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; // 문제 내용
        public QuestionType Type { get; set; } // 문제 유형
        public QuestionCategory Category { get; set; } // 문제 카테고리
        public ExperienceLevel TargetLevel { get; set; } = ExperienceLevel.Junior; // 대상 경력 레벨
        public List<string> Choices { get; set; } = new List<string>(); // 객관식 선택지 (주관식/인성은 null)
        public int? CorrectChoiceIndex { get; set; } // 객관식 정답 인덱스 (주관식/인성은 null)
        public string? Answer { get; set; } // 주관식 정답 (객관식/인성은 null)
        public int? PersonalityScore { get; set; } // 인성 문제 점수(1~5 등, 기타는 null)
        public TeamType Team { get; set; } = TeamType.Backend;
        
        // 자동 채점을 위한 간단한 필드들
        public string? ModelAnswer { get; set; } // 모범 답안
        public int MaxScore { get; set; } = 5; // 최대 점수 (기본값 5점)
        public string? ScoringCriteria { get; set; } // 채점 기준 설명
    }
} 