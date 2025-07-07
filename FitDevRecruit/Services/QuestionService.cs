using System;
using System.Collections.Generic;
using System.Linq;
using FitDevRecruit.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    }
} 