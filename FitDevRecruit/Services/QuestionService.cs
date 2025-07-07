using System;
using System.Collections.Generic;
using System.Linq;
using FitDevRecruit.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            InitializeAutoScoringData();
        }

        private void InitializeQuestionRatios()
        {
            _questionRatios = new Dictionary<(TeamType, ExperienceLevel), Dictionary<QuestionCategory, double>>
            {
                // 백엔드 신입: 기술 50%, 코딩 20%, 인성 20%, 문제해결력 10%
                {(TeamType.Backend, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.5}, {QuestionCategory.Coding, 0.2}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 백엔드 경력: 기술 25%, 인성 20%, 문제해결력 20%, 설계 20%, 리더십 15%
                {(TeamType.Backend, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.25}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.2}, {QuestionCategory.Design, 0.2}, {QuestionCategory.Leadership, 0.15}}
                },
                
                // 프론트엔드 신입: 기술 45%, 코딩 20%, 인성 25%, 문제해결력 10%
                {(TeamType.Frontend, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.45}, {QuestionCategory.Coding, 0.2}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 프론트엔드 경력: 기술 25%, 인성 20%, 문제해결력 20%, 설계 20%, 리더십 15%
                {(TeamType.Frontend, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.25}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.2}, {QuestionCategory.Design, 0.2}, {QuestionCategory.Leadership, 0.15}}
                },
                
                // 데이터 신입: 기술 50%, 코딩 15%, 인성 25%, 문제해결력 10%
                {(TeamType.Data, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.5}, {QuestionCategory.Coding, 0.15}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 데이터 경력: 기술 25%, 인성 20%, 문제해결력 20%, 설계 20%, 리더십 15%
                {(TeamType.Data, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.25}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.2}, {QuestionCategory.Design, 0.2}, {QuestionCategory.Leadership, 0.15}}
                },
                
                // 인프라 신입: 기술 55%, 코딩 10%, 인성 25%, 문제해결력 10%
                {(TeamType.Infra, ExperienceLevel.Junior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.55}, {QuestionCategory.Coding, 0.1}, {QuestionCategory.Personality, 0.25}, {QuestionCategory.ProblemSolving, 0.1}}
                },
                
                // 인프라 경력: 기술 25%, 인성 20%, 문제해결력 20%, 설계 20%, 리더십 15%
                {(TeamType.Infra, ExperienceLevel.Senior), new Dictionary<QuestionCategory, double>
                    {{QuestionCategory.Technical, 0.25}, {QuestionCategory.Personality, 0.2}, {QuestionCategory.ProblemSolving, 0.2}, {QuestionCategory.Design, 0.2}, {QuestionCategory.Leadership, 0.15}}
                }
            };
        }

        private void InitializeQuestions()
        {
            // 여러 경로에서 questions.json 파일을 찾아봅니다
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "questions.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "questions.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "questions.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "questions.json")
            };

            string? jsonPath = null;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    jsonPath = path;
                    break;
                }
            }

            if (jsonPath == null)
                throw new FileNotFoundException($"문제 데이터 파일을 찾을 수 없습니다. 다음 경로들을 확인해주세요: {string.Join(", ", possiblePaths)}");

            var json = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());
            _questions = JsonSerializer.Deserialize<List<Question>>(json, options) ?? new List<Question>();
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
                if (codingQuestions.Count > 0)
                {
                    var selectedCodingQuestions = codingQuestions.OrderBy(x => _random.Next()).Take(codingCount);
                    questions.AddRange(selectedCodingQuestions);
                }
                
                // 나머지 카테고리 선택 (비율 조정)
                var remainingCategories = ratios.Where(kvp => kvp.Key != QuestionCategory.Coding).ToList();
                var remainingSlots = totalCount - questions.Count;
                var totalRemainingRatio = remainingCategories.Sum(kvp => kvp.Value);
                
                foreach (var (category, ratio) in remainingCategories)
                {
                    var categoryQuestions = GetQuestionsByCategory(category, team, level);
                    if (categoryQuestions.Count == 0) continue;
                    
                    var adjustedRatio = totalRemainingRatio > 0 ? ratio / totalRemainingRatio : 1.0 / remainingCategories.Count;
                    var count = Math.Max(1, (int)(remainingSlots * adjustedRatio));
                    count = Math.Min(count, categoryQuestions.Count); // 실제 문항 수를 초과하지 않도록
                    
                    var selectedQuestions = categoryQuestions.OrderBy(x => _random.Next()).Take(count);
                    questions.AddRange(selectedQuestions);
                }
            }
            else
            {
                // 경력 개발자는 기존 로직 사용하되 개선
                foreach (var (category, ratio) in ratios)
                {
                    var categoryQuestions = GetQuestionsByCategory(category, team, level);
                    if (categoryQuestions.Count == 0) continue;
                    
                    var count = Math.Max(1, (int)(totalCount * ratio));
                    count = Math.Min(count, categoryQuestions.Count); // 실제 문항 수를 초과하지 않도록
                    
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
            
            // 최종적으로 정확히 totalCount개 반환
            var finalQuestions = questions.OrderBy(x => _random.Next()).Take(totalCount).ToList();
            Console.WriteLine($"DEBUG: 최종 선택된 문항 수: {finalQuestions.Count}");
            
            // 여전히 부족한 경우 강제로 10문제 반환
            if (finalQuestions.Count < totalCount)
            {
                var allAvailableQuestions = _questions.Where(q => q.Team == team && q.TargetLevel == level).ToList();
                if (allAvailableQuestions.Count >= totalCount)
                {
                    finalQuestions = allAvailableQuestions.OrderBy(x => _random.Next()).Take(totalCount).ToList();
                    Console.WriteLine($"DEBUG: 강제로 {totalCount}문제 반환: {finalQuestions.Count}");
                }
                else
                {
                    // 모든 문항을 반환하고 부족한 만큼 다른 팀/레벨에서 보충
                    finalQuestions = allAvailableQuestions.ToList();
                    var otherQuestions = _questions.Where(q => q.Team != team || q.TargetLevel != level).ToList();
                    var needed = totalCount - finalQuestions.Count;
                    if (otherQuestions.Count > 0)
                    {
                        var additional = otherQuestions.OrderBy(x => _random.Next()).Take(needed);
                        finalQuestions.AddRange(additional);
                    }
                    Console.WriteLine($"DEBUG: 보충 문항 포함 {totalCount}문제 반환: {finalQuestions.Count}");
                }
            }
            
            return finalQuestions;
        }

        private List<Question> GetQuestionsByCategory(QuestionCategory category, TeamType team, ExperienceLevel level)
        {
            return _questions.Where(q => q.Category == category && q.Team == team && q.TargetLevel == level).ToList();
        }

        // 자동 채점을 위한 데이터 초기화
        private void InitializeAutoScoringData()
        {
            // 각 주관식 문항에 간단한 채점 기준 추가
            foreach (var question in _questions.Where(q => q.Type == QuestionType.Subjective))
            {
                switch (question.Id)
                {
                    case 1: // C#에서 string과 String의 차이는?
                        question.ModelAnswer = "string은 참조타입이고 String은 System.String의 별칭입니다";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "참조타입 또는 별칭 개념 언급 시 높은 점수";
                        break;
                    case 2: // HTTP의 GET과 POST의 차이는?
                        question.ModelAnswer = "GET은 안전하고 멱등하며 캐시 가능하고, POST는 안전하지 않고 멱등하지 않습니다";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "안전성, 멱등성, 캐시 가능성 중 하나라도 언급 시 점수";
                        break;
                    case 3: // RESTful API란 무엇인가요?
                        question.ModelAnswer = "REST는 HTTP 기반의 웹 서비스 아키텍처로 리소스 중심 설계입니다";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "HTTP, 리소스, 아키텍처 중 하나라도 언급 시 점수";
                        break;
                    case 6: // 1부터 100까지의 합을 구하는 C# 코드를 작성하세요.
                        question.ModelAnswer = "for문 또는 LINQ를 사용한 반복문";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "반복문 사용 시 높은 점수, LINQ 사용 시 추가 점수";
                        break;
                    case 7: // 배열에서 중복을 제거하는 C# 코드를 작성하세요.
                        question.ModelAnswer = "Distinct 메서드 또는 HashSet 사용";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "Distinct 또는 HashSet 사용 시 높은 점수";
                        break;
                    case 10: // 예상치 못한 버그가 발생했을 때 어떻게 접근합니까?
                        question.ModelAnswer = "로그 확인, 디버깅, 분석 과정 포함";
                        question.MaxScore = 5;
                        question.ScoringCriteria = "로그, 디버깅, 분석 중 하나라도 언급 시 점수";
                        break;
                    default:
                        // 기본 채점 기준 설정
                        question.MaxScore = 5;
                        question.ScoringCriteria = "답변 내용에 따라 점수 부여";
                        break;
                }
            }
        }

        // 개선된 조건 기반 자동 채점
        public int AutoScoreSubjectiveQuestion(int questionId, string answer)
        {
            var question = _questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null || question.Type != QuestionType.Subjective)
                return 0;

            if (string.IsNullOrWhiteSpace(answer))
                return 0;

            // 문항별 상세 채점 기준 적용 (answers.txt 기준)
            switch (question.Id)
            {
                case 1: // C#에서 string과 String의 차이는?
                    if (answer.Contains("참조타입") || answer.Contains("별칭"))
                        return 5;
                    else if (answer.Contains("string") || answer.Contains("String"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("타입") || answer.Contains("클래스")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 2: // HTTP의 GET과 POST의 차이는?
                    if (answer.Contains("안전") || answer.Contains("멱등") || answer.Contains("캐시"))
                        return 5;
                    else if (answer.Contains("GET") || answer.Contains("POST"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("HTTP") || answer.Contains("메서드")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 3: // RESTful API란 무엇인가요?
                    if (answer.Contains("HTTP") || answer.Contains("리소스") || answer.Contains("아키텍처"))
                        return 5;
                    else if (answer.Contains("REST") || answer.Contains("API"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("웹") || answer.Contains("서비스")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 4: // SQL에서 JOIN의 종류를 설명하세요.
                    if (answer.Contains("JOIN") || answer.Contains("SQL") || answer.Contains("테이블"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 6: // 1부터 100까지의 합을 구하는 C# 코드를 작성하세요.
                    if (answer.Contains("for") || answer.Contains("while") || answer.Contains("LINQ"))
                        return 5;
                    else if (answer.Contains("반복") || answer.Contains("루프"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("코드") || answer.Contains("구현")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 7: // 배열에서 중복을 제거하는 C# 코드를 작성하세요.
                    if (answer.Contains("Distinct") || answer.Contains("HashSet"))
                        return 5;
                    else if (answer.Contains("중복") || answer.Contains("제거"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("배열") || answer.Contains("코드")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 10: // 예상치 못한 버그가 발생했을 때 어떻게 접근합니까?
                    if (answer.Contains("로그") || answer.Contains("디버깅") || answer.Contains("분석"))
                        return 5;
                    else if (answer.Contains("확인") || answer.Contains("검사"))
                        return 3;
                    else if (answer.Length > 20 && (answer.Contains("버그") || answer.Contains("문제")))
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 11: // 마이크로서비스 아키텍처의 장단점을 설명하세요.
                    if (answer.Contains("마이크로서비스") || answer.Contains("아키텍처") || answer.Contains("서비스"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 12: // 트랜잭션의 ACID 원칙을 설명하세요.
                    if (answer.Contains("ACID") || answer.Contains("트랜잭션") || answer.Contains("원자성") || 
                        answer.Contains("일관성") || answer.Contains("고립성") || answer.Contains("지속성"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 14: // 대규모 트래픽 처리 경험을 설명하세요.
                    if (answer.Contains("트래픽") || answer.Contains("처리") || answer.Contains("성능") || answer.Contains("최적화"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 15: // 신규 서비스 아키텍처를 설계할 때 가장 먼저 고려할 점은?
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 16: // 대규모 트래픽 처리 방안을 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 18: // 팀 빌딩 과정에서 중요하게 생각하는 점은?
                    if (answer.Contains("팀") || answer.Contains("빌딩") || answer.Contains("협업") || answer.Contains("소통"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 19: // 예상치 못한 버그가 발생했을 때 어떻게 접근합니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 20: // 시스템 장애 발생 시 문제 원인 분석 과정을 설명하세요.
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 22: // CSS에서 flex와 grid의 차이점은?
                    if (answer.Contains("flex") || answer.Contains("grid") || answer.Contains("CSS") || answer.Contains("레이아웃"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 23: // 자바스크립트에서 let과 var의 차이는?
                    if (answer.Contains("let") || answer.Contains("var") || answer.Contains("스코프") || answer.Contains("호이스팅"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 26: // 배열의 모든 요소를 2배로 만드는 JS 코드를 작성하세요.
                    if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 27: // 문자열을 뒤집는 JS 코드를 작성하세요.
                    if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 29: // 학습에 대한 태도는 어떻게 되시나요?
                    if (answer.Contains("학습") || answer.Contains("태도") || answer.Contains("개발") || answer.Contains("성장"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 30: // 예상치 못한 요구사항 변경에 어떻게 대응합니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 31: // SPA(Single Page Application)의 장점은?
                    if (answer.Contains("SPA") || answer.Contains("싱글페이지") || answer.Contains("애플리케이션"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 33: // 대규모 프로젝트 협업 경험을 설명하세요.
                    if (answer.Contains("프로젝트") || answer.Contains("협업") || answer.Contains("경험") || answer.Contains("팀"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 34: // 대규모 프론트엔드 프로젝트 구조를 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 35: // 컴포넌트 설계 원칙은?
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 36: // 상태 관리 아키텍처를 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 37: // 갈등 상황에서의 리더십을 발휘한 경험을 서술하세요.
                    if (answer.Contains("리더십") || answer.Contains("갈등") || answer.Contains("경험") || answer.Contains("해결"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 39: // 예상치 못한 요구사항 변경에 어떻게 대응합니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 40: // 사용자 경험 개선을 위한 문제 해결 방법은?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 42: // 파이썬에서 리스트와 튜플의 차이는?
                    if (answer.Contains("리스트") || answer.Contains("튜플") || answer.Contains("파이썬") || answer.Contains("변경가능"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 45: // 데이터 시각화의 중요성은?
                    if (answer.Contains("데이터") || answer.Contains("시각화") || answer.Contains("중요성") || answer.Contains("분석"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 46: // 리스트에서 중복을 제거하는 파이썬 코드를 작성하세요.
                    if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 47: // 데이터프레임에서 결측값을 처리하는 파이썬 코드를 작성하세요.
                    if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 48: // 성장 마인드란 무엇이라고 생각합니까?
                    if (answer.Contains("성장") || answer.Contains("마인드") || answer.Contains("학습") || answer.Contains("개발"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 50: // 예상치 못한 데이터 오류가 발생했을 때 어떻게 접근합니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 52: // 데이터 품질 관리의 중요성은?
                    if (answer.Contains("데이터") || answer.Contains("품질") || answer.Contains("관리") || answer.Contains("중요성"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 53: // 머신러닝 모델 평가 지표는?
                    if (answer.Contains("머신러닝") || answer.Contains("모델") || answer.Contains("평가") || answer.Contains("지표"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 54: // 데이터 파이프라인을 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 55: // 데이터 웨어하우스 설계 원칙은?
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 56: // 머신러닝 파이프라인을 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 58: // 멘토링 경험을 설명하세요.
                    if (answer.Contains("멘토링") || answer.Contains("경험") || answer.Contains("지도") || answer.Contains("교육"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 59: // 데이터 품질 문제 해결 경험을 설명하세요.
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 64: // 네트워크 계층 구조를 설명하세요.
                    if (answer.Contains("네트워크") || answer.Contains("계층") || answer.Contains("구조") || answer.Contains("OSI"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 66: // 서버 모니터링의 중요성은?
                    if (answer.Contains("서버") || answer.Contains("모니터링") || answer.Contains("중요성") || answer.Contains("관리"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 67: // 리눅스에서 파일 개수를 세는 명령어를 작성하세요.
                    if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 68: // 책임감이란 무엇이라고 생각합니까?
                    if (answer.Contains("책임감") || answer.Contains("의무") || answer.Contains("책임") || answer.Contains("성실"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 69: // 스트레스 관리 방법은?
                    if (answer.Contains("스트레스") || answer.Contains("관리") || answer.Contains("해소") || answer.Contains("대처"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 70: // 예상치 못한 장애가 발생했을 때 어떻게 접근합니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 71: // CI/CD 파이프라인 구축 시 고려사항은?
                    if (answer.Contains("CI/CD") || answer.Contains("파이프라인") || answer.Contains("구축") || answer.Contains("고려사항"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 72: // 클라우드 인프라의 장단점은?
                    if (answer.Contains("클라우드") || answer.Contains("인프라") || answer.Contains("장단점") || answer.Contains("장점"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 73: // 컨테이너 기술의 장단점은?
                    if (answer.Contains("컨테이너") || answer.Contains("기술") || answer.Contains("장단점") || answer.Contains("Docker"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 74: // 대규모 인프라 시스템을 설계해보세요.
                    if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조") || answer.Contains("인프라"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 75: // 고가용성 시스템 설계 원칙은?
                    if (answer.Contains("고가용성") || answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("이중화"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 76: // 보안 아키텍처를 설계해보세요.
                    if (answer.Contains("보안") || answer.Contains("아키텍처") || answer.Contains("설계") || answer.Contains("구조"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 77: // 협업 과정에서 가장 중요하게 생각하는 가치는?
                    if (answer.Contains("협업") || answer.Contains("가치") || answer.Contains("소통") || answer.Contains("신뢰"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 78: // 조직 문화 개선 경험을 설명하세요.
                    if (answer.Contains("조직") || answer.Contains("문화") || answer.Contains("개선") || answer.Contains("경험"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 79: // 프로젝트 마감 직전 큰 오류가 발생했다면 어떻게 하시겠습니까?
                    if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근") || answer.Contains("오류"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                case 80: // 위기 상황에서의 리더십을 발휘한 경험은?
                    if (answer.Contains("위기") || answer.Contains("리더십") || answer.Contains("경험") || answer.Contains("해결"))
                        return 3;
                    else if (answer.Length > 20)
                        return 2;
                    else if (answer.Length > 10)
                        return 1;
                    return 0;

                default:
                    // 개선된 기본 채점 로직: 키워드 + 길이 조합
                    var score = 0;
                    
                    // 카테고리별 기본 키워드 체크
                    if (question.Category == QuestionCategory.Technical)
                    {
                        if (answer.Contains("기술") || answer.Contains("개념") || answer.Contains("원리"))
                            score = 3;
                        else if (answer.Length > 20)
                            score = 2;
                        else if (answer.Length > 10)
                            score = 1;
                    }
                    else if (question.Category == QuestionCategory.Coding)
                    {
                        if (answer.Contains("코드") || answer.Contains("구현") || answer.Contains("알고리즘"))
                            score = 3;
                        else if (answer.Length > 20)
                            score = 2;
                        else if (answer.Length > 10)
                            score = 1;
                    }
                    else if (question.Category == QuestionCategory.Design)
                    {
                        if (answer.Contains("설계") || answer.Contains("아키텍처") || answer.Contains("구조"))
                            score = 3;
                        else if (answer.Length > 20)
                            score = 2;
                        else if (answer.Length > 10)
                            score = 1;
                    }
                    else if (question.Category == QuestionCategory.ProblemSolving)
                    {
                        if (answer.Contains("분석") || answer.Contains("해결") || answer.Contains("접근"))
                            score = 3;
                        else if (answer.Length > 20)
                            score = 2;
                        else if (answer.Length > 10)
                            score = 1;
                    }
                    else
                    {
                        // 기타 카테고리
                        if (answer.Length >= 20)
                            score = 3;
                        else if (answer.Length >= 10)
                            score = 2;
                        else if (answer.Length >= 5)
                            score = 1;
                    }
                    
                    return score;
            }
        }

        // AI 기반 자동 채점 (GPT API 활용 가능)
        public async Task<int> AutoScoreWithAI(int questionId, string answer)
        {
            // 간단한 조건 기반 채점으로 대체
            return AutoScoreSubjectiveQuestion(questionId, answer);
        }

        // 종합 자동 채점 (간단한 방식)
        public async Task<int> ComprehensiveAutoScore(int questionId, string answer)
        {
            // 간단한 조건 기반 채점만 사용
            return AutoScoreSubjectiveQuestion(questionId, answer);
        }

        // 채점 결과 상세 정보 반환 (간단한 피드백)
        public (int score, List<string> matchedKeywords, string feedback) GetDetailedScoringResult(int questionId, string answer)
        {
            var question = _questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null || question.Type != QuestionType.Subjective)
                return (0, new List<string>(), "채점 불가능한 문항입니다.");

            var score = AutoScoreSubjectiveQuestion(questionId, answer);
            var feedback = GenerateSimpleFeedback(score, question.MaxScore);

            return (score, new List<string>(), feedback);
        }

        // 간단한 피드백 생성
        private string GenerateSimpleFeedback(int score, int maxScore)
        {
            if (score >= maxScore * 0.8)
                return "훌륭한 답변입니다!";
            else if (score >= maxScore * 0.6)
                return "좋은 답변입니다.";
            else if (score >= maxScore * 0.4)
                return "보통 수준의 답변입니다.";
            else if (score > 0)
                return "개선이 필요한 답변입니다.";
            else
                return "답변이 부족합니다.";
        }
    }
} 