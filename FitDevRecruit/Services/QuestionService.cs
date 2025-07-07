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

        // 팀/경력별 출제 비율 정의
        private Dictionary<(TeamType, ExperienceLevel), Dictionary<QuestionCategory, double>> _questionRatios;

        public QuestionService()
        {
            InitializeQuestionRatios();
            InitializeQuestions();
        }

        private void InitializeQuestionRatios()
        {
            _questionRatios = new Dictionary<(TeamType, ExperienceLevel), Dictionary<QuestionCategory, double>>
            {
                // 백엔드 신입: 기술 50%, 코딩 20%, 인성 20%, 문제해결력 10%
                {(TeamType.Backend, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.5}, {QuestionCategory.Coding, 0.2}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 백엔드 경력: 기술 30%, 설계 30%, 리더십 25%, 문제해결력 15%
                {(TeamType.Backend, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.3}, {QuestionCategory.Design, 0.3}, {QuestionCategory.Leadership, 0.25}, {QuestionCategory.ProblemSolving, 0.15}}
                },
                
                // 프론트엔드 신입: 기술 45%, 코딩 20%, 인성 25%, 문제해결력 10%
                {(TeamType.Frontend, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.45}, {QuestionCategory.Coding, 0.2}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 프론트엔드 경력: 기술 25%, 설계 30%, 리더십 25%, 문제해결력 20%
                {(TeamType.Frontend, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.25}, {QuestionCategory.Design, 0.3}, {QuestionCategory.Leadership, 0.25}, {QuestionCategory.ProblemSolving, 0.2}}
                },
                
                // 데이터 신입: 기술 50%, 코딩 15%, 인성 25%, 문제해결력 10%
                {(TeamType.Data, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.5}, {QuestionCategory.Coding, 0.15}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 데이터 경력: 기술 30%, 설계 30%, 리더십 25%, 문제해결력 15%
                {(TeamType.Data, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.3}, {QuestionCategory.Design, 0.3}, {QuestionCategory.Leadership, 0.25}, {QuestionCategory.ProblemSolving, 0.15}}
                },
                
                // 인프라 신입: 기술 55%, 코딩 10%, 인성 25%, 문제해결력 10%
                {(TeamType.Infra, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.55}, {QuestionCategory.Coding, 0.1}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 인프라 경력: 기술 30%, 설계 30%, 리더십 25%, 문제해결력 15%
                {(TeamType.Infra, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.3}, {QuestionCategory.Design, 0.3}, {QuestionCategory.Leadership, 0.25}, {QuestionCategory.ProblemSolving, 0.15}}
                }
            };
        }

        private void InitializeQuestions()
        {
            _questions = new List<Question>
            {
                // ===== 기술 문항 (Technical) =====
                // 백엔드 기술
                new Question { Id = 1, Text = "C#에서 string과 String의 차이는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 2, Text = "HTTP의 GET과 POST의 차이는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 3, Text = "RESTful API란 무엇인가요?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 4, Text = "SQL에서 JOIN의 종류를 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 5, Text = "Java의 기본 자료형이 아닌 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"int", "float", "String", "char"}, CorrectChoiceIndex = 2 },
                new Question { Id = 6, Text = "LINQ의 장점은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"간결성", "성능", "확장성", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 7, Text = "다음 중 C#의 컬렉션이 아닌 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"List", "Dictionary", "Set", "ArrayList"}, CorrectChoiceIndex = 2 },
                new Question { Id = 8, Text = "코드가 예상대로 동작하지 않을 때 가장 먼저 확인할 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"오타", "로직", "환경설정", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 9, Text = "마이크로서비스 아키텍처의 장단점을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 10, Text = "트랜잭션의 ACID 원칙을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 11, Text = "운영 중인 서비스에서 장애가 발생했을 때 가장 먼저 할 일은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend, Choices = new List<string>{"로그 확인", "서비스 재시작", "팀원 호출", "사용자 공지"}, CorrectChoiceIndex = 0 },
                new Question { Id = 12, Text = "대규모 트래픽 처리 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 13, Text = "RESTful API 설계 시 고려사항은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 14, Text = "데이터베이스 인덱싱 전략을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 15, Text = "캐싱 전략의 종류와 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                
                // 프론트엔드 기술
                new Question { Id = 16, Text = "HTML에서 가장 기본적인 태그는?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"<div>", "<html>", "<body>", "<head>"}, CorrectChoiceIndex = 1 },
                new Question { Id = 17, Text = "CSS에서 flex와 grid의 차이점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 18, Text = "자바스크립트에서 let과 var의 차이는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 19, Text = "React의 주요 특징은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"컴포넌트 기반", "MVC", "템플릿 엔진", "서버 사이드 렌더링"}, CorrectChoiceIndex = 0 },
                new Question { Id = 20, Text = "프론트엔드 개발에서 가장 중요한 역량은?", Type = QuestionType.Personality, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"UI/UX", "성능", "협업", "트렌드"}, PersonalityScore = 3 },
                new Question { Id = 21, Text = "SPA(Single Page Application)의 장점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 22, Text = "프론트엔드 성능 최적화 방법은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend, Choices = new List<string>{"코드 스플리팅", "이미지 최적화", "지연 로딩", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 23, Text = "대규모 프로젝트 협업 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 24, Text = "코드 리뷰 문화에 대해 어떻게 생각합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 25, Text = "웹 접근성의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 26, Text = "크로스 브라우징 이슈 해결 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 27, Text = "모바일 반응형 디자인 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                
                // 데이터 기술
                new Question { Id = 28, Text = "SQL에서 데이터를 조회하는 명령어는?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"SELECT", "INSERT", "UPDATE", "DELETE"}, CorrectChoiceIndex = 0 },
                new Question { Id = 29, Text = "파이썬에서 리스트와 튜플의 차이는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 30, Text = "데이터 시각화 도구로 적합하지 않은 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"Tableau", "PowerBI", "Excel", "Notepad"}, CorrectChoiceIndex = 3 },
                new Question { Id = 31, Text = "데이터 분석에서 가장 중요한 역량은?", Type = QuestionType.Personality, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"논리적 사고", "도구 활용", "협업", "호기심"}, PersonalityScore = 1 },
                new Question { Id = 32, Text = "데이터 시각화의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 33, Text = "대용량 데이터 처리 시 성능 최적화 방법은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data, Choices = new List<string>{"인덱싱", "캐싱", "파티셔닝", "모든 것"}, CorrectChoiceIndex = 3 },
                new Question { Id = 34, Text = "데이터 품질 관리의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 35, Text = "머신러닝 모델 평가 지표는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 36, Text = "데이터 전처리의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 37, Text = "빅데이터 처리 기술의 종류는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 38, Text = "데이터 보안과 개인정보 보호 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                
                // 인프라 기술
                new Question { Id = 39, Text = "리눅스에서 파일 권한을 변경하는 명령어는?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"chmod", "chown", "ls", "cd"}, CorrectChoiceIndex = 0 },
                new Question { Id = 40, Text = "서버 장애 발생 시 가장 먼저 확인할 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"로그", "네트워크", "하드웨어", "모두"}, CorrectChoiceIndex = 3 },
                new Question { Id = 41, Text = "인프라 엔지니어에게 중요한 역량은?", Type = QuestionType.Personality, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"책임감", "신속함", "협업", "문서화"}, PersonalityScore = 1 },
                new Question { Id = 42, Text = "네트워크 계층 구조를 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 43, Text = "서버 모니터링 도구로 적합하지 않은 것은?", Type = QuestionType.MultipleChoice, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra, Choices = new List<string>{"Nagios", "Zabbix", "Grafana", "Notepad"}, CorrectChoiceIndex = 3 },
                new Question { Id = 44, Text = "서버 모니터링의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 45, Text = "CI/CD 파이프라인 구축 시 고려사항은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 46, Text = "클라우드 인프라의 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 47, Text = "컨테이너 기술의 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 48, Text = "로드 밸런싱 전략의 종류는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 49, Text = "백업 및 재해 복구 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 50, Text = "보안 정책 수립 시 고려사항은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },

                // ===== 코딩테스트 문항 (Coding) =====
                new Question { Id = 51, Text = "1부터 100까지의 합을 구하는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 52, Text = "배열에서 중복을 제거하는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 53, Text = "팩토리얼을 계산하는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 54, Text = "피보나치 수열을 구하는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 55, Text = "문자열을 뒤집는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 56, Text = "배열의 모든 요소를 2배로 만드는 JS 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 57, Text = "문자열을 뒤집는 JS 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 58, Text = "리스트에서 중복을 제거하는 파이썬 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 59, Text = "리눅스에서 파일 개수를 세는 명령어를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 60, Text = "정렬 알고리즘을 구현해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 61, Text = "이진 탐색 알고리즘을 구현해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 62, Text = "스택과 큐의 차이점을 코드로 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },

                // ===== 설계/아키텍처 문항 (Design) =====
                new Question { Id = 63, Text = "신규 서비스 아키텍처를 설계할 때 가장 먼저 고려할 점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 64, Text = "대규모 트래픽 처리 방안을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 65, Text = "마이크로서비스 아키텍처 설계 시 고려사항은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 66, Text = "데이터베이스 샤딩 전략을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 67, Text = "대규모 프론트엔드 프로젝트 구조를 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 68, Text = "컴포넌트 설계 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 69, Text = "상태 관리 아키텍처를 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 70, Text = "데이터 파이프라인을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 71, Text = "데이터 웨어하우스 설계 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 72, Text = "머신러닝 파이프라인을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 73, Text = "대규모 인프라 시스템을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 74, Text = "고가용성 시스템 설계 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 75, Text = "보안 아키텍처를 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },

                // ===== 인성/성향 문항 (Personality) =====
                new Question { Id = 76, Text = "협업 시 가장 중요한 태도는?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"소통", "속도", "완벽함", "독립성"}, PersonalityScore = 1 },
                new Question { Id = 77, Text = "팀 프로젝트에서 의견 충돌이 생기면 어떻게 할 것인가?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"타협", "주장", "무시", "회피"}, PersonalityScore = 1 },
                new Question { Id = 78, Text = "성장 마인드란 무엇이라고 생각합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 79, Text = "책임감이란 무엇이라고 생각합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 80, Text = "동료가 실수했을 때 어떻게 대처하나요?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"도움", "지적", "무시", "리더에게 보고"}, PersonalityScore = 1 },
                new Question { Id = 81, Text = "팀 내에서 소통이 잘 안 될 때 어떻게 하시겠습니까?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"직접 대화", "문서화", "무시", "리더에게 보고"}, PersonalityScore = 1 },
                new Question { Id = 82, Text = "학습에 대한 태도는 어떻게 되시나요?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 83, Text = "업무에 대한 열정은 어느 정도인가요?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data, Choices = new List<string>{"매우 높음", "높음", "보통", "낮음"}, PersonalityScore = 2 },
                new Question { Id = 84, Text = "스트레스 관리 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 85, Text = "팀 내 갈등이 발생했을 때 어떻게 해결할 것인가?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend, Choices = new List<string>{"대화", "무시", "타협", "리더에게 보고"}, PersonalityScore = 1 },
                new Question { Id = 86, Text = "새로운 기술을 배우는 데 얼마나 시간을 투자하시나요?", Type = QuestionType.Personality, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend, Choices = new List<string>{"매우 많음", "많음", "보통", "적음"}, PersonalityScore = 2 },
                new Question { Id = 87, Text = "실패했을 때 어떻게 대처하시나요?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 88, Text = "업무 우선순위를 어떻게 정하시나요?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },

                // ===== 리더십 문항 (Leadership) =====
                new Question { Id = 89, Text = "리더로서 가장 중요한 덕목은?", Type = QuestionType.Personality, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend, Choices = new List<string>{"소통", "결단력", "공정성", "책임감"}, PersonalityScore = 4 },
                new Question { Id = 90, Text = "갈등 상황에서의 리더십을 발휘한 경험을 서술하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 91, Text = "팀원이 마감 기한을 지키지 못할 때 어떻게 하시겠습니까?", Type = QuestionType.Personality, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data, Choices = new List<string>{"이유 파악", "질책", "무시", "직접 도와줌"}, PersonalityScore = 1 },
                new Question { Id = 92, Text = "협업 과정에서 가장 중요하게 생각하는 가치는?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 93, Text = "리더로서의 역할은 무엇이라고 생각합니까?", Type = QuestionType.Personality, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend, Choices = new List<string>{"소통", "결단력", "공정성", "책임감"}, PersonalityScore = 4 },
                new Question { Id = 94, Text = "신기술 도입 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 95, Text = "팀 빌딩 과정에서 중요하게 생각하는 점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 96, Text = "멘토링 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 97, Text = "조직 문화 개선 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 98, Text = "위기 상황에서의 리더십을 발휘한 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 99, Text = "팀 성과 향상을 위한 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 100, Text = "인재 육성 방안에 대해 어떻게 생각하시나요?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },

                // ===== 문제해결력 문항 (ProblemSolving) =====
                new Question { Id = 101, Text = "예상치 못한 버그가 발생했을 때 어떻게 접근합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 102, Text = "시스템 장애 발생 시 문제 원인 분석 과정을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 103, Text = "예상치 못한 요구사항 변경에 어떻게 대응합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 104, Text = "팀 내 기술적 의견 충돌이 발생했을 때 어떻게 해결합니까?", Type = QuestionType.Personality, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data, Choices = new List<string>{"토론", "다수결", "리더 결정", "외부 자문"}, PersonalityScore = 1 },
                new Question { Id = 105, Text = "프로젝트 마감 직전 큰 오류가 발생했다면 어떻게 하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 106, Text = "문제 상황에서의 대처 방법을 서술하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 107, Text = "예상치 못한 데이터 오류가 발생했을 때 어떻게 접근합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 108, Text = "예상치 못한 장애가 발생했을 때 어떻게 접근합니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 109, Text = "성능 최적화 문제 해결 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 110, Text = "사용자 경험 개선을 위한 문제 해결 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 111, Text = "데이터 품질 문제 해결 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 112, Text = "인프라 장애 대응 및 복구 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 113, Text = "보안 취약점 발견 시 대응 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Backend },
                new Question { Id = 114, Text = "사용자 피드백을 통한 문제 해결 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 115, Text = "데이터 분석을 통한 비즈니스 문제 해결 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 116, Text = "시스템 확장성 문제 해결 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },

                // ===== 백엔드 Junior 추가 문항 =====
                new Question { Id = 117, Text = "C#에서 async/await의 사용법을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 118, Text = "Entity Framework의 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 119, Text = "API 버전 관리 방법을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 120, Text = "데이터베이스 정규화의 목적은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 121, Text = "배열을 정렬하는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 122, Text = "문자열에서 특정 문자를 찾는 C# 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 123, Text = "업무 중 새로운 기술을 배워야 할 때 어떻게 하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 124, Text = "코드 리뷰를 받을 때 어떤 태도를 가지시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 125, Text = "데이터베이스 연결 오류가 발생했을 때 어떻게 해결하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },
                new Question { Id = 126, Text = "메모리 누수 문제를 어떻게 진단하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Backend },

                // ===== 프론트엔드 Junior 추가 문항 =====
                new Question { Id = 127, Text = "CSS에서 반응형 디자인의 핵심은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 128, Text = "JavaScript에서 클로저란 무엇인가요?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 129, Text = "Vue.js와 React의 차이점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 130, Text = "웹 성능 최적화의 기본 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 131, Text = "배열의 모든 요소를 필터링하는 JS 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 132, Text = "이벤트 리스너를 추가하는 JS 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 133, Text = "프론트엔드 개발자로서 가장 중요하게 생각하는 역량은?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 134, Text = "디자이너와 협업할 때 어떤 태도를 가지시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 135, Text = "브라우저 호환성 문제를 어떻게 해결하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },
                new Question { Id = 136, Text = "페이지 로딩 속도가 느릴 때 어떻게 개선하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Frontend },

                // ===== 프론트엔드 Senior 추가 문항 =====
                new Question { Id = 137, Text = "대규모 프론트엔드 프로젝트의 상태 관리 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 138, Text = "마이크로 프론트엔드 아키텍처의 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 139, Text = "프론트엔드 테스트 전략을 설계해보세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 140, Text = "프론트엔드 팀 리더로서의 역할은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 141, Text = "주니어 개발자 멘토링 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 142, Text = "프론트엔드 기술 스택 도입 결정 과정은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 143, Text = "복잡한 UI 컴포넌트 설계 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },
                new Question { Id = 144, Text = "프론트엔드 보안 이슈 해결 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Frontend },

                // ===== 데이터 Junior 추가 문항 =====
                new Question { Id = 145, Text = "데이터 분석 프로세스의 단계를 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 146, Text = "SQL에서 GROUP BY의 역할은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 147, Text = "데이터 품질 검증 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 148, Text = "데이터 시각화의 기본 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 149, Text = "데이터프레임을 필터링하는 파이썬 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 150, Text = "데이터 정렬하는 파이썬 코드를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 151, Text = "데이터 분석가로서의 직업관은?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 152, Text = "데이터 기반 의사결정의 중요성은?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 153, Text = "이상치 데이터를 어떻게 처리하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },
                new Question { Id = 154, Text = "데이터 누락 문제를 어떻게 해결하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Data },

                // ===== 데이터 Senior 추가 문항 =====
                new Question { Id = 155, Text = "데이터 아키텍처 설계 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 156, Text = "실시간 데이터 처리 시스템 설계는?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 157, Text = "데이터 거버넌스 체계 구축 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 158, Text = "데이터 팀 관리 경험을 설명하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 159, Text = "데이터 전략 수립 과정은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 160, Text = "데이터 품질 관리 체계 구축 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },
                new Question { Id = 161, Text = "대용량 데이터 처리 성능 최적화 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Data },

                // ===== 인프라 Junior 추가 문항 =====
                new Question { Id = 162, Text = "리눅스 시스템 모니터링 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 163, Text = "네트워크 보안의 기본 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 164, Text = "서버 백업 전략의 종류는?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 165, Text = "클라우드 서비스의 장단점은?", Type = QuestionType.Subjective, Category = QuestionCategory.Technical, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 166, Text = "서버 로그를 분석하는 명령어를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 167, Text = "네트워크 연결을 확인하는 명령어를 작성하세요.", Type = QuestionType.Subjective, Category = QuestionCategory.Coding, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 168, Text = "인프라 엔지니어로서의 책임감은?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 169, Text = "24시간 운영 시스템에 대한 태도는?", Type = QuestionType.Subjective, Category = QuestionCategory.Personality, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 170, Text = "서버 다운 상황을 어떻게 대처하시겠습니까?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },
                new Question { Id = 171, Text = "네트워크 장애 원인 분석 방법은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Junior, Team = TeamType.Infra },

                // ===== 인프라 Senior 추가 문항 =====
                new Question { Id = 172, Text = "대규모 인프라 아키텍처 설계 원칙은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 173, Text = "멀티 클라우드 전략 설계는?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 174, Text = "인프라 자동화 체계 구축 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.Design, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 175, Text = "인프라 팀 리더로서의 역할은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 176, Text = "인프라 보안 정책 수립 경험은?", Type = QuestionType.Subjective, Category = QuestionCategory.Leadership, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 177, Text = "대규모 장애 대응 및 복구 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra },
                new Question { Id = 178, Text = "인프라 비용 최적화 전략은?", Type = QuestionType.Subjective, Category = QuestionCategory.ProblemSolving, TargetLevel = ExperienceLevel.Senior, Team = TeamType.Infra }
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

                // 새로운 맞춤형 출제 메서드
        public List<Question> GetCustomizedQuestions(TeamType team, ExperienceLevel level, int totalCount = 10)
        {
            var key = (team, level);
            if (!_questionRatios.ContainsKey(key))
            {
                // 기본 비율로 폴백
                return GetQuestionsByLevelAndTeam(level, team, totalCount);
            }

            // 디버깅: 해당 팀/레벨의 전체 문항 수 확인
            var allQuestionsForTeam = _questions.Where(q => q.Team == team && q.TargetLevel == level).ToList();
            Console.WriteLine($"DEBUG: {team} {level} 전체 문항 수: {allQuestionsForTeam.Count}");
            
            foreach (var category in Enum.GetValues<QuestionCategory>())
            {
                var categoryQuestions = allQuestionsForTeam.Where(q => q.Category == category).ToList();
                Console.WriteLine($"DEBUG: {category} 카테고리 문항 수: {categoryQuestions.Count}");
            }

            var ratios = _questionRatios[key];
            var questions = new List<Question>();
            
            // 신입 개발자의 경우 코딩테스트는 최대 2문제로 제한
            if (level == ExperienceLevel.Junior)
            {
                // 코딩테스트 먼저 선택 (1-2문제)
                var codingQuestions = GetQuestionsByCategory(QuestionCategory.Coding, team, level);
                var codingCount = Math.Min(2, Math.Max(1, (int)(totalCount * ratios[QuestionCategory.Coding])));
                var selectedCodingQuestions = codingQuestions.OrderBy(x => _random.Next()).Take(codingCount);
                questions.AddRange(selectedCodingQuestions);
                
                // 나머지 카테고리 선택
                foreach (var (category, ratio) in ratios)
                {
                    if (category == QuestionCategory.Coding) continue; // 이미 처리됨
                    
                    var categoryQuestions = GetQuestionsByCategory(category, team, level);
                    var remainingSlots = totalCount - questions.Count;
                    var categoryRatio = ratio / (ratios.Values.Sum() - ratios[QuestionCategory.Coding]);
                    var count = Math.Max(1, (int)(remainingSlots * categoryRatio));
                    
                    var selectedQuestions = categoryQuestions.OrderBy(x => _random.Next()).Take(count);
                    questions.AddRange(selectedQuestions);
                }
            }
            else
            {
                // 경력 개발자는 기존 로직 사용
                foreach (var (category, ratio) in ratios)
                {
                    var categoryQuestions = GetQuestionsByCategory(category, team, level);
                    var count = Math.Max(1, (int)(totalCount * ratio));
                    
                    var selectedQuestions = categoryQuestions.OrderBy(x => _random.Next()).Take(count);
                    questions.AddRange(selectedQuestions);
                }
            }
            
            // 부족한 경우 랜덤으로 보충
            while (questions.Count < totalCount)
            {
                var remainingQuestions = _questions.Where(q => q.TargetLevel == level && q.Team == team && !questions.Contains(q)).ToList();
                if (!remainingQuestions.Any()) break;
                
                questions.Add(remainingQuestions[_random.Next(remainingQuestions.Count)]);
            }
            
            var finalQuestions = questions.OrderBy(x => _random.Next()).Take(totalCount).ToList();
            Console.WriteLine($"DEBUG: 최종 선택된 문항 수: {finalQuestions.Count}");
            
            // 임시: 10문제가 안 나오면 강제로 10문제 반환
            if (finalQuestions.Count < totalCount)
            {
                var allAvailableQuestions = _questions.Where(q => q.Team == team && q.TargetLevel == level).ToList();
                finalQuestions = allAvailableQuestions.OrderBy(x => _random.Next()).Take(totalCount).ToList();
                Console.WriteLine($"DEBUG: 강제로 10문제 반환: {finalQuestions.Count}");
            }
            
            return finalQuestions;
        }

        private List<Question> GetQuestionsByCategory(QuestionCategory category, TeamType team, ExperienceLevel level)
        {
            return _questions.Where(q => q.Category == category && q.Team == team && q.TargetLevel == level).ToList();
        }
    }
} 