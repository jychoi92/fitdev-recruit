using Microsoft.AspNetCore.Mvc;
using FitDevRecruit.Models;
using FitDevRecruit.Services;

namespace FitDevRecruit.Controllers
{
    public class RecruitController : Controller
    {
        private readonly QuestionService _questionService;
        private readonly TestService _testService;
        private readonly ReportService _reportService;

        public RecruitController(QuestionService questionService, TestService testService, ReportService reportService)
        {
            _questionService = questionService;
            _testService = testService;
            _reportService = reportService;
        }

        // 지원자 정보 입력 페이지
        public IActionResult Index()
        {
            return View();
        }

        // 지원자 정보 입력 처리
        [HttpPost]
        public IActionResult StartTest(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                candidate.Id = new Random().Next(1000, 9999);
                
                // 세션에 지원자 정보 저장
                HttpContext.Session.SetString("CandidateName", candidate.Name);
                HttpContext.Session.SetString("CandidateEmail", candidate.Email);
                HttpContext.Session.SetString("ExperienceLevel", candidate.ExperienceLevel.ToString());
                HttpContext.Session.SetString("TeamType", candidate.Team.ToString());
                HttpContext.Session.SetInt32("CandidateId", candidate.Id);
                
                return RedirectToAction("Test", new { candidateId = candidate.Id });
            }
            return View("Index", candidate);
        }

        // 문제 풀이 페이지
        public IActionResult Test(int candidateId)
        {
            var candidateName = HttpContext.Session.GetString("CandidateName");
            var candidateEmail = HttpContext.Session.GetString("CandidateEmail");
            var experienceLevel = HttpContext.Session.GetString("ExperienceLevel");
            var teamType = HttpContext.Session.GetString("TeamType");
            
            ExperienceLevel level = ExperienceLevel.Junior;
            TeamType team = TeamType.Backend;
            
            if (Enum.TryParse<ExperienceLevel>(experienceLevel, out var parsedLevel))
            {
                level = parsedLevel;
            }
            
            if (Enum.TryParse<TeamType>(teamType, out var parsedTeam))
            {
                team = parsedTeam;
            }
            
            // 새로운 맞춤형 출제 메서드 사용
            var questions = _questionService.GetCustomizedQuestions(team, level, 10);
            ViewBag.CandidateId = candidateId;
            ViewBag.ExperienceLevel = level;
            ViewBag.TeamType = team;
            return View(questions);
        }

        // 답안 제출 처리
        [HttpPost]
        public IActionResult SubmitTest(int candidateId, Dictionary<int, string> answers)
        {
            var candidate = new Candidate
            {
                Id = candidateId,
                Name = HttpContext.Session.GetString("CandidateName") ?? "",
                Email = HttpContext.Session.GetString("CandidateEmail") ?? "",
                ExperienceLevel = Enum.Parse<ExperienceLevel>(HttpContext.Session.GetString("ExperienceLevel") ?? "Junior"),
                Team = Enum.Parse<TeamType>(HttpContext.Session.GetString("TeamType") ?? "Backend"),
                Answers = answers
            };

            // 새로운 카테고리별 채점 로직
            var questions = _questionService.GetAllQuestions();
            int technical = 0, personality = 0, problemSolving = 0, coding = 0, design = 0, leadership = 0;

            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.Key);
                if (question != null)
                {
                    var score = CalculateScore(question, answer.Value);
                    
                    switch (question.Category)
                    {
                        case QuestionCategory.Technical:
                            technical += score;
                            break;
                        case QuestionCategory.Coding:
                            coding += score;
                            break;
                        case QuestionCategory.Design:
                            design += score;
                            break;
                        case QuestionCategory.Personality:
                            personality += score;
                            break;
                        case QuestionCategory.Leadership:
                            leadership += score;
                            break;
                        case QuestionCategory.ProblemSolving:
                            problemSolving += score;
                            break;
                    }
                }
            }

            // 종합 점수 계산
            candidate.TechnicalScore = technical + coding + design;
            candidate.PersonalityScore = personality + leadership;
            candidate.ProblemSolvingScore = problemSolving;

            var report = _reportService.GenerateReport(candidate);
            return View("Result", report);
        }

        private int CalculateScore(Question question, string? answer)
        {
            if (string.IsNullOrEmpty(answer)) return 0;

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                    if (int.TryParse(answer, out int idx) && question.CorrectChoiceIndex == idx - 1)
                        return 5; // 객관식 정답 시 5점
                    return 0;
                
                case QuestionType.Subjective:
                    // 주관식은 간단히 정답 문자열 비교 (실제론 더 정교한 평가 필요)
                    if (!string.IsNullOrEmpty(question.Answer) && answer.Trim().ToLower() == question.Answer.Trim().ToLower())
                        return 5; // 주관식 정답 시 5점
                    return 2; // 부분 점수
                
                case QuestionType.Personality:
                    if (int.TryParse(answer, out int pScore) && pScore >= 1 && pScore <= 5)
                        return question.PersonalityScore ?? pScore; // 인성 점수 반영
                    return 0;
                
                default:
                    return 0;
            }
        }

        // 결과 페이지
        public IActionResult Result()
        {
            return View();
        }
    }
} 