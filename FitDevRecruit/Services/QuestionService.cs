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
                // 신입 - 백엔드
                new Question { Id = 1, Text = "C#에서 string과 String의 차이는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 2, Text = "Java의 기본 자료형이 아닌 것은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"int", "float", "String", "char"}, CorrectChoiceIndex = 2 },
                new Question { Id = 3, Text = "협업 시 가장 중요한 태도는?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"소통", "속도", "완벽함", "독립성"}, PersonalityScore = 1 },
                new Question { Id = 4, Text = "HTTP의 GET과 POST의 차이는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 5, Text = "코드가 예상대로 동작하지 않을 때 가장 먼저 확인할 것은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"오타", "로직", "환경설정", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 6, Text = "팀 프로젝트에서 의견 충돌이 생기면 어떻게 할 것인가?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"타협", "주장", "무시", "회피"}, PersonalityScore = 1 },
                // 신입 - 프론트엔드
                new Question { Id = 7, Text = "HTML에서 가장 기본적인 태그는?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"<div>", "<html>", "<body>", "<head>"}, CorrectChoiceIndex = 1 },
                new Question { Id = 8, Text = "CSS에서 flex와 grid의 차이점은?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 9, Text = "프론트엔드 개발에서 가장 중요한 역량은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"UI/UX", "성능", "협업", "트렌드"}, PersonalityScore = 3 },
                new Question { Id = 10, Text = "자바스크립트에서 let과 var의 차이는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 11, Text = "React의 주요 특징은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"컴포넌트 기반", "MVC", "템플릿 엔진", "서버 사이드 렌더링"}, CorrectChoiceIndex = 0 },
                // 신입 - 데이터
                new Question { Id = 12, Text = "SQL에서 데이터를 조회하는 명령어는?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"SELECT", "INSERT", "UPDATE", "DELETE"}, CorrectChoiceIndex = 0 },
                new Question { Id = 13, Text = "데이터 분석에서 가장 중요한 역량은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"논리적 사고", "도구 활용", "협업", "호기심"}, PersonalityScore = 1 },
                new Question { Id = 14, Text = "파이썬에서 리스트와 튜플의 차이는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 15, Text = "데이터 시각화 도구로 적합하지 않은 것은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"Tableau", "PowerBI", "Excel", "Notepad"}, CorrectChoiceIndex = 3 },
                // 신입 - 인프라
                new Question { Id = 16, Text = "리눅스에서 파일 권한을 변경하는 명령어는?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"chmod", "chown", "ls", "cd"}, CorrectChoiceIndex = 0 },
                new Question { Id = 17, Text = "서버 장애 발생 시 가장 먼저 확인할 것은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"로그", "네트워크", "하드웨어", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 18, Text = "인프라 엔지니어에게 중요한 역량은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"책임감", "신속함", "협업", "문서화"}, PersonalityScore = 1 },
                // 경력 - 백엔드
                new Question { Id = 19, Text = "마이크로서비스 아키텍처의 장단점을 설명하세요.", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 20, Text = "트랜잭션의 ACID 원칙 중 'I'는 무엇을 의미하는가?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend, Choices = new List<string>{"Isolation", "Integrity", "Index", "Instance"}, CorrectChoiceIndex = 0 },
                new Question { Id = 21, Text = "팀원이 마감 기한을 지키지 못할 때 어떻게 하시겠습니까?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend, Choices = new List<string>{"이유 파악", "질책", "무시", "직접 도와줌"}, PersonalityScore = 1 },
                new Question { Id = 22, Text = "운영 중인 서비스에서 장애가 발생했을 때 가장 먼저 할 일은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend, Choices = new List<string>{"로그 확인", "서비스 재시작", "팀원 호출", "사용자 공지"}, CorrectChoiceIndex = 0 },
                // 경력 - 프론트엔드
                new Question { Id = 23, Text = "SPA(Single Page Application)의 장점은?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 24, Text = "프론트엔드 성능 최적화 방법은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend, Choices = new List<string>{"코드 스플리팅", "이미지 최적화", "지연 로딩", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 25, Text = "리더로서 가장 중요한 덕목은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend, Choices = new List<string>{"소통", "결단력", "공정성", "책임감"}, PersonalityScore = 4 },
                // 경력 - 데이터
                new Question { Id = 26, Text = "대용량 데이터 처리 시 성능 최적화 방법은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data, Choices = new List<string>{"인덱싱", "캐싱", "파티셔닝", "모든 것"}, CorrectChoiceIndex = 3 },
                new Question { Id = 27, Text = "데이터 분석 프로젝트에서 가장 중요한 단계는?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 28, Text = "데이터 엔지니어에게 필요한 역량은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data, Choices = new List<string>{"분석력", "도구 활용", "협업", "문제 해결력"}, PersonalityScore = 4 },
                // 경력 - 인프라
                new Question { Id = 29, Text = "CI/CD 파이프라인 구축 시 고려사항은?", Type = QuestionType.Subjective, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 30, Text = "시스템 설계 시 확장성을 고려한 패턴은?", Type = QuestionType.MultipleChoice, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra, Choices = new List<string>{"싱글톤", "팩토리", "옵저버", "모든 것"}, CorrectChoiceIndex = 3 },
                new Question { Id = 31, Text = "인프라 리더로서 중요한 역량은?", Type = QuestionType.Personality, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra, Choices = new List<string>{"책임감", "신속함", "협업", "문서화"}, PersonalityScore = 4 },
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

        public List<Question> GetQuestionsByLevelAndTeam(ExperienceLevel level, TeamType team, int count = 5)
        {
            return _questions.Where(q => q.TargetLevel == level && q.Team == team)
                           .OrderBy(x => _random.Next())
                           .Take(count)
                           .ToList();
        }
    }
} 